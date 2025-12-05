using System.Net.Mime;
using System.Text.Json;
using AidManager.API.Shared.Domain.Entities;
using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous] 

public class AuthorizeController(IGoogleAuthorization googleAuthorization, AppDBContext context): ControllerBase
{
    [HttpGet]
    public IActionResult Authorize() => Ok(googleAuthorization.GetAuthorizationUrl());

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {
        try
        {
            // Decodifica explícitamente el código
            var decodedCode = System.Web.HttpUtility.UrlDecode(code);
        
            Console.WriteLine($"Original code: {code}");
            Console.WriteLine($"Decoded code: {decodedCode}");
        
            var userCredential = await googleAuthorization.ExchangeCodeForToken(decodedCode);
        
            var _credential = await context.Credentials
                .FirstOrDefaultAsync(c => c.AccessToken == userCredential.Token.AccessToken);
            
            if (_credential == null)
            {
                return BadRequest("No se encontró el usuario para el token proporcionado.");
            }
        
            return Redirect($"http://localhost:8080/connect/{_credential.UserId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en callback: {ex.Message}");
            return BadRequest($"Error al procesar la autorización: {ex.Message}");
        }
    }


    [HttpGet("token/{userId}")]
    public async Task<IActionResult> GetAccessToken(string userId)
    {
        Guid _userId = Guid.Empty;
        try
        {
            _userId = Guid.Parse(userId);
        }
        catch { return Unauthorized(); }

        var credential = await context.Credentials.FirstOrDefaultAsync(c => c.UserId == _userId);
        return Ok(JsonSerializer.Serialize
            (new Token(credential.AccessToken, credential.UserId.ToString())));
    }
}
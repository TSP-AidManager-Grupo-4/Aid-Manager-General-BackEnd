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
        
            // Use new method that creates/finds user and links credential
            var (credential, user, googleData) = await googleAuthorization.ExchangeCodeForTokenWithUserData(decodedCode);
            
            if (credential == null || user == null)
            {
                return BadRequest("Error creando/encontrando el usuario de OAuth.");
            }
        
            // Return user data to frontend via redirect with query params
            var responseData = new {
                success = true,
                user = new { id = user.Id, email = user.Email, firstName = user.FirstName, lastName = user.LastName, profileImg = user.ProfileImg, companyId = user.CompanyId },
                credential = new { credentialId = credential.UserId, accessToken = credential.AccessToken, expiresIn = credential.ExpiresInSeconds }
            };
            
            var jsonData = JsonSerializer.Serialize(responseData);
            var encodedData = System.Web.HttpUtility.UrlEncode(jsonData);
            
            return Redirect($"http://localhost:8080/auth/callback?data={encodedData}");
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
        int _userId = 0;
        try
        {
            _userId = int.Parse(userId);
        }
        catch { return Unauthorized(); }

        var credential = await context.Credentials.FirstOrDefaultAsync(c => c.UserId == _userId);
        return Ok(JsonSerializer.Serialize
            (new Token(credential.AccessToken, credential.UserId.ToString())));
    }
}
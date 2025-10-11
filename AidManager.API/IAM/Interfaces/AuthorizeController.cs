using System.Net.Mime;
using System.Text.Json;
using AidManager.API.Shared.Domain.Entities;
using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]

public class AuthorizeController(IGoogleAuthorization googleAuthorization, AppDBContext context): ControllerBase
{
    [HttpGet]
    public IActionResult Authorize() => Ok(googleAuthorization.GetAuthorizationUrl());

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {
        var userCredential = await googleAuthorization.ExchangeCodeForToken(code);
        var _credential = await context.Credentials
            .FirstOrDefaultAsync(c=>c.AccessToken == userCredential.Token.AccessToken);
        if (_credential == null)
        {
            return BadRequest("No se encontró el usuario para el token proporcionado.");
        }
        return Redirect($"https://localhost:5082/connect/{_credential.UserId}");
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
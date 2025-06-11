using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Microsoft.Extensions.Configuration;

namespace Nop.Web.Controllers.Api;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ICustomerRegistrationService _customerRegistrationService;
    private readonly IConfiguration _configuration;

    public AuthController(
        ICustomerRegistrationService customerRegistrationService, IConfiguration configuration)
    {
        _customerRegistrationService = customerRegistrationService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest model)
    {
        var loginResult = await _customerRegistrationService.ValidateCustomerAsync(model.Email, model.Password);

        if (loginResult != CustomerLoginResults.Successful)
            return Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]?? string.Empty);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, model.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { token = tokenString });
    }
}

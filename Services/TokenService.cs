using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OpenWhistle.Services;

public interface ITokenService
{
    public string GenerateToken(Guid reportId,
        int expirationMinutes);

    public bool ValidateToken(string token, Guid reportId);
}

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(Guid reportId, int expirationMinutes)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT config missing");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, reportId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(jwtSettings["Issuer"],
                                         jwtSettings["Audience"],
                                         claims,
                                         expires: DateTime.Now.AddMinutes(expirationMinutes),
                                         signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token, Guid reportId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            {
                return false;
            }

            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            return claim != null && claim.Value == reportId.ToString();
        }
        catch
        {
            return false;
        }
    }
}
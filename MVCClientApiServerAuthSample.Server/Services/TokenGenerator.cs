using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using MVCClientApiServerAuthSample.Server.Models;

namespace MVCClientApiServerAuthSample.Server.Services;

// TODO: Extract the abstraction! In this case, it is simplified for the sake of example
internal class TokenGenerator
{
    private readonly IOptions<JwtOptions> _options;

    public TokenGenerator(IOptions<JwtOptions> options)
        => _options = options;

    public string GetAccessToken(string name)
    {
        JwtSecurityTokenHandler tokenHandler = new();

        var tokenDescriptor = new SecurityTokenDescriptor
                              {
                                  Subject = new ClaimsIdentity(new Claim[]
                                                               {
                                                                   new(ClaimTypes.Name, name)
                                                               }),
                                  Expires = DateTime.UtcNow.Add(_options.Value.Lifetime),
                                  Issuer = _options.Value.Issuer,
                                  Audience = _options.Value.Audience,
                                  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Value.SecurityKey)),
                                                                              SecurityAlgorithms.HmacSha256Signature)
                              };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}
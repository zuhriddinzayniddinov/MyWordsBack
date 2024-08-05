using System.IdentityModel.Tokens.Jwt;
using Entity.Models;
using Entity.Models.Auth;

namespace AuthenticationBroker.TokenHandler;

public interface IJwtTokenHandler
{
    public (string refreshToken, DateTime expireDate) GenerateRefreshToken();
    JwtSecurityToken GenerateAccessToken(User user);
}
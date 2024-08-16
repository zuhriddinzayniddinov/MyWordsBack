using System.IdentityModel.Tokens.Jwt;
using AuthenticationBroker.TokenHandler;
using DatabaseBroker.Repositories.Auth;
using Entity.DataTransferObjects.Authentication;
using Entity.Enum;
using Entity.Exeptions;
using Entity.Extensions;
using Entity.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services;

public class AuthService(
    IJwtTokenHandler jwtTokenHandler,
    IUserRepository userRepository,
    ITokenRepository tokenRepository,
    ISignMethodsRepository signMethodsRepository,
    IStructureRepository structureRepository)
    : IAuthService
{
    public async ValueTask<bool> RegisterAsync(UserRegisterDto userRegisterDto)
    {
        var hasStoredUser = await signMethodsRepository.OfType<PasswordSignMethod>()
            .AnyAsync(x => x.PhoneNumber == userRegisterDto.phoneNumber);
        if (hasStoredUser)
            throw new AlreadyExistsException("Phone number already exists");

        var newUser = new User()
        {
            FirstName = userRegisterDto.firstName,
            LastName = userRegisterDto.lastName,
            NativeLanguage = userRegisterDto.nativeLanguage,
            SignMethods = new List<SignMethod>(),
            StructureId = (await structureRepository.FirstOrDefaultAsync(x => x.IsDefault))?.Id,
        };

        var storedUser = await userRepository.AddAsync(newUser);
        await signMethodsRepository.AddAsync(new PasswordSignMethod()
        {
            PhoneNumber = userRegisterDto.phoneNumber,
            PasswordHash = PasswordHelper.Encrypt(userRegisterDto.password),
            UserId = storedUser.Id
        });
        return true;
    }

    public async ValueTask<TokenDto> SignByPasswordAsync(AuthenticationDto authenticationDto)
    {
        var signMethod = await signMethodsRepository.OfType<PasswordSignMethod>()
            .FirstOrDefaultAsync(x => x.PhoneNumber == authenticationDto.phoneNumber);

        if (signMethod is null)
            throw new NotFoundException("That credentials not found");

        if (!PasswordHelper.Verify(signMethod.PasswordHash, authenticationDto.password))
            throw new NotFoundException("User not found");

        var user = signMethod.User;

        var refreshToken = jwtTokenHandler.GenerateRefreshToken();

        var token = new TokenModel()
        {
            UserId = user.Id,
            TokenType = TokenTypes.Password,
            AccessToken = new JwtSecurityTokenHandler()
                .WriteToken(jwtTokenHandler.GenerateAccessToken(user)),
            RefreshToken = refreshToken.refreshToken,
            ExpireRefreshToken = refreshToken.expireDate
        };

        token = await tokenRepository.AddAsync(token);

        var tokenDto = new TokenDto(
            token.AccessToken,
            token.RefreshToken,
            token.ExpireRefreshToken);

        return tokenDto;
    }

    public async ValueTask<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
    {
        var token = await tokenRepository
            .GetAllAsQueryable()
            .FirstOrDefaultAsync(t => t.AccessToken == tokenDto.accessToken
                && t.RefreshToken == tokenDto.refreshToken)
            ?? throw new NotFoundException("Not Found Token");

        if (token.ExpireRefreshToken < DateTime.UtcNow)
        {
            var deleteToken = await tokenRepository.RemoveAsync(token);
            throw new AlreadyExistsException("Refresh token timed out");
        }

        token.User = await userRepository.GetByIdAsync(token.UserId)
            ?? throw new NotFoundException($"User not found by id: {token.UserId}");

        token.AccessToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtTokenHandler.GenerateAccessToken(token.User));
        token.UpdatedAt = DateTime.UtcNow;

        token = await tokenRepository.UpdateAsync(token);

        var newTokenDto = new TokenDto(
            token.AccessToken,
            token.RefreshToken,
            token.ExpireRefreshToken);

        return newTokenDto;
    }

    public async ValueTask<bool> DeleteTokenAsync(TokenDto tokenDto)
    {
        var token = await tokenRepository
                        .GetAllAsQueryable()
                        .FirstOrDefaultAsync(t => t.AccessToken == tokenDto.accessToken
                                                  && t.RefreshToken == tokenDto.refreshToken)
                    ?? throw new NotFoundException("Not Found Token");

        var deleteToken = await tokenRepository.RemoveAsync(token);

        return deleteToken.Id == token.Id;
    }
}
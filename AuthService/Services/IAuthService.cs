using Entity.DataTransferObjects.Authentication;

namespace AuthService.Services;

public interface IAuthService
{
    ValueTask<bool> RegisterAsync(UserRegisterDto userRegisterDto);
    ValueTask<TokenDto> SignByPasswordAsync(AuthenticationDto authenticationDto);
    ValueTask<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
    ValueTask<bool> DeleteTokenAsync(TokenDto tokenDto);
}
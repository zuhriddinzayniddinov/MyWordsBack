using AuthService.Services;
using Entity.DataTransferObjects.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebCore.Controllers;
using WebCore.Models;

namespace TemplateApi.Controllers.Auth;
[Route("api-auth/[controller]/[action]")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost]
    public async Task<ResponseModel> Sign([FromBody] AuthenticationDto authenticationDto)
        => ResponseModel.ResultFromContent(await authService.SignByPasswordAsync(authenticationDto));

    [HttpPost]
    public async Task<ResponseModel> Register([FromBody] UserRegisterDto userRegisterDto)
        => ResponseModel.ResultFromContent(await authService.RegisterAsync(userRegisterDto));
    
    [HttpPost]
    public async Task<ResponseModel> RefreshToken([FromBody]TokenDto tokenDto)
        => ResponseModel.ResultFromContent(await authService.RefreshTokenAsync(tokenDto));
    
    [HttpDelete]
    public async Task<ResponseModel> LogOut([FromBody]TokenDto tokenDto)
        => ResponseModel.ResultFromContent(await authService.DeleteTokenAsync(tokenDto));
}
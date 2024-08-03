using AuthService.Services;
using Entity.DataTransferObjects.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebCore.Controllers;
using WebCore.Models;

namespace AuthApi.Controllers;
public class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost]
    public async Task<ResponseModel> Sign(
        [FromBody] AuthenticationDto authenticationDto)
    {
        return ResponseModel
            .ResultFromContent(
                await authService.SignByPassword(authenticationDto));
    }

    [HttpPost]
    public async Task<ResponseModel> Register(
        [FromBody] UserRegisterDto userRegisterDto)
        => ResponseModel.ResultFromContent(await authService.Register(userRegisterDto));
    
    [HttpPost]
    public async Task<ResponseModel> RefreshToken([FromBody]TokenDto tokenDto)
        => ResponseModel.ResultFromContent(await authService.RefreshTokenAsync(tokenDto));
    
    [HttpDelete]
    public async Task<ResponseModel> LogOut([FromBody]TokenDto tokenDto)
        => ResponseModel.ResultFromContent(await authService.DeleteTokenAsync(tokenDto));
}
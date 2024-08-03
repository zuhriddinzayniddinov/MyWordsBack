using Entity.DataTransferObjects.StaticFiles;
using Microsoft.AspNetCore.Mvc;
using StaticFileService.Service;
using WebCore.Models;

namespace AssetApi.Controllers;

[RequestFormLimits(MultipartBodyLengthLimit = (long)1024 * 1024 * 1024 * 1024)]
public class StaticFileController(IStaticFileService staticFileService) : ControllerBase
{
    [HttpPost]
    public async Task<ResponseModel> Add([FromForm]FileDto fileDto)
        => ResponseModel.ResultFromContent(await staticFileService.AddFileAsync(fileDto));

    [HttpDelete]
    public async Task<ResponseModel> Remove([FromBody]RemoveFileDto removeFileDto)
        => ResponseModel.ResultFromContent(await staticFileService.RemoveAsync(removeFileDto));
}
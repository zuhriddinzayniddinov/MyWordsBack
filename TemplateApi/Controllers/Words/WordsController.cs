using Entity.DataTransferObjects.Words;
using Entity.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using WebCore.Controllers;
using WebCore.Models;
using WordsService.WordService;

namespace TemplateApi.Controllers.Words;
[Route("api-word/[controller]/[action]")]
public class WordsController(IWordService wordService) : ApiControllerBase
{
    [HttpPost]
    public async Task<ResponseModel> Create([FromBody] WordCreatedDto wordCreatedDto)
        => ResponseModel.ResultFromContent(await wordService.CreateWordAsync(wordCreatedDto,UserId));
    
    [HttpGet]
    public async Task<ResponseModel> GetById([FromRoute] long id)
        => ResponseModel.ResultFromContent(await wordService.GetWordByIdAsync(id));

    [HttpGet]
    public async Task<ResponseModel> GetAll([FromQuery] MetaQueryModel queryModel)
    {
        var result = await wordService.GetWordsAsync(queryModel, UserId);

        return new ResponseModel()
        {
            Content = result.Item1,
            Total = result.Item2
        };
    }
}
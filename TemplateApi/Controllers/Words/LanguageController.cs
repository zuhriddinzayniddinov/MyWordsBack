using Entity.DataTransferObjects.Words;
using Entity.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using WebCore.Controllers;
using WebCore.Models;
using WordsService.LanguageService;

namespace TemplateApi.Controllers.Words;
[Route("api-word/[controller]/[action]")]
public class LanguageController(ILanguageService languageService) : ApiControllerBase
{
    [HttpPost]
    public async Task<ResponseModel> Create([FromBody] LanguageCreatedDto languageCreatedDto)
        => ResponseModel.ResultFromContent(await languageService.CreateLanguageAsync(languageCreatedDto));
    
    [HttpGet]
    public async Task<ResponseModel> GetById([FromRoute] long id)
        => ResponseModel.ResultFromContent(await languageService.GetLanguageByIdAsync(id));
    
    [HttpGet]
    public async Task<ResponseModel> GetByCode([FromRoute] string code)
        => ResponseModel.ResultFromContent(await languageService.GetLanguageByCodeAsync(code));
    
    [HttpGet]
    public async Task<ResponseModel> GetAll([FromQuery]MetaQueryModel queryModel)
    {
        var result = await languageService.GetLanguagesAsync(queryModel);

        return new ResponseModel()
        {
            Content = result.Item1,
            Total = result.Item2
        };
    }
}
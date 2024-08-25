using Entity.DataTransferObjects.Words;
using Entity.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using WebCore.Controllers;
using WebCore.Models;
using WordsService.GroupService;

namespace TemplateApi.Controllers.Words;
[Route("api-word/[controller]/[action]")]
public class GroupController(IGroupService groupService) : ApiControllerBase
{
    [HttpPost]
    public async Task<ResponseModel> Create([FromBody] GroupCreatedDto groupCreatedDto)
        => ResponseModel.ResultFromContent(await groupService.CreateGroupAsync(groupCreatedDto,UserId));
    
    [HttpGet]
    public async Task<ResponseModel> GetById([FromRoute] long id)
        => ResponseModel.ResultFromContent(await groupService.GetGroupByIdAsync(id));
    
    [HttpGet]
    public async Task<ResponseModel> GetAll([FromQuery]MetaQueryModel queryModel)
    {
        var result = await groupService.GetGroupsAsync(queryModel, UserId);

        return new ResponseModel()
        {
            Content = result.Item1,
            Total = result.Item2
        };
    }
}
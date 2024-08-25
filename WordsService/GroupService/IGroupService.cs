using Entity.DataTransferObjects.Words;
using Entity.Models.ApiModels;

namespace WordsService.GroupService;

public interface IGroupService
{
    Task<GroupDto> GetGroupByIdAsync(long id);
    Task<(List<GroupDto> result, int count)> GetGroupsAsync(MetaQueryModel metaQuery,long? userId);
    Task<GroupDto> CreateGroupAsync(GroupCreatedDto groupCreatedDto,long userId);
}
using DatabaseBroker.Repositories.Auth;
using DatabaseBroker.Repositories.Words;
using Entity.DataTransferObjects.Words;
using Entity.Exeptions;
using Entity.Models.ApiModels;
using Entity.Models.Words;
using Microsoft.EntityFrameworkCore;

namespace WordsService.GroupService;

public class GroupService(IGroupRepository groupRepository,
    IUserRepository userRepository) : IGroupService
{
    public async Task<GroupDto> GetGroupByIdAsync(long id)
    {
        var group = await groupRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Not found group by id: {id}");
        
        return new GroupDto(
            group.Id,
            group.Name,
            group.ToLanguageId,
            group.ToLanguage.Name,
            group.FromLanguageId,
            group.FromLanguage.Name,
            group.Words.Count);
    }

    public async Task<(List<GroupDto> result, int count)> GetGroupsAsync(MetaQueryModel metaQuery,long? userId)
    {
        var query = groupRepository.GetAllAsQueryable();
        
        if(userId is not null or 0)
            query = query.Where(g => g.UserId == userId);
        
        return (await query
            .Skip(metaQuery.Skip)
            .Take(metaQuery.Take)
            .Select(group => new GroupDto(
                group.Id,
                group.Name,
                group.ToLanguageId,
                group.ToLanguage.Name,
                group.FromLanguageId,
                group.FromLanguage.Name,
                group.Words.Count))
            .ToListAsync(),
                query.Count());
    }

    public async Task<GroupDto> CreateGroupAsync(GroupCreatedDto groupCreatedDto, long userId)
    {
        var group = await groupRepository.GetAllAsQueryable()
            .Where(g => g.UserId == userId)
            .Where(g => g.ToLanguageId == groupCreatedDto.toLanguageId)
            .Where(g => g.FromLanguageId == groupCreatedDto.fromLanguageId)
            .FirstOrDefaultAsync();

        if (group == null)
        {
            var fromLanguageId = groupCreatedDto.fromLanguageId ??
                                 userRepository.GetAllAsQueryable()
                                     .Where(u => u.Id == userId)
                                     .Select(u => u.NativeLanguageId)
                                     .FirstOrDefault() ?? 0;
                
            group = await groupRepository.AddAsync(new Group()
            {
                ToLanguageId = groupCreatedDto.toLanguageId,
                FromLanguageId = fromLanguageId,
                UserId = userId
            });
        }
        
        return new GroupDto(
            group.Id,
            group.Name,
            group.ToLanguageId,
            group.ToLanguage.Name,
            group.FromLanguageId,
            group.FromLanguage.Name,
            group.Words.Count);
    }
}
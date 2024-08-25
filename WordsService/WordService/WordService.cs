using DatabaseBroker.Repositories.Words;
using Entity.DataTransferObjects.Words;
using Entity.Exeptions;
using Entity.Models.ApiModels;
using Entity.Models.Words;
using Microsoft.EntityFrameworkCore;

namespace WordsService.WordService;

public class WordService(IWordRepository wordRepository) : IWordService
{
    public async Task<WordDto> CreateWordAsync(WordCreatedDto wordCreatedDto, long userId)
    {
        var word = await wordRepository.AddAsync(new Word()
                       {
                           GroupId = wordCreatedDto.groupId,
                           Text = wordCreatedDto.text,
                           Translation = wordCreatedDto.translate,
                           UserId = userId
                       });
        
        return new WordDto(
            word.Id,
            word.Text,
            word.Translation,
            word.GroupId);
    }

    public async Task<WordDto> GetWordByIdAsync(long id)
    {
        var word = await wordRepository.GetAllAsQueryable()
                           .FirstOrDefaultAsync(w => w.Id == id)
                       ?? throw new NotFoundException($"Not found word by id: {id}");
        
        return new WordDto(
            word.Id,
            word.Text,
            word.Translation,
            word.GroupId);
    }

    public async Task<(List<WordDto>,int)> GetWordsAsync(MetaQueryModel queryModel, long? userId)
    {
        var query = wordRepository.GetAllAsQueryable();
        
        if(userId is not null or 0)
            query = query.Where(g => g.UserId == userId);
        
        return (await query
                .Skip(queryModel.Skip)
                .Take(queryModel.Take)
                .Select(w => new WordDto(
                    w.Id,
                    w.Text,
                    w.Translation,
                    w.GroupId))
                .ToListAsync(),
            query.Count());
    }
}
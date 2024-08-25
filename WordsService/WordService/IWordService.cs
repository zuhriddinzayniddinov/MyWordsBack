using Entity.DataTransferObjects.Words;
using Entity.Models.ApiModels;

namespace WordsService.WordService;

public interface IWordService
{
    Task<WordDto> CreateWordAsync(WordCreatedDto wordCreatedDto, long userId);
    Task<WordDto> GetWordByIdAsync(long id);
    Task<(List<WordDto>,int)> GetWordsAsync(MetaQueryModel queryModel, long? userId);
}
using Entity.DataTransferObjects.Words;
using Entity.Models.ApiModels;

namespace WordsService.LanguageService;

public interface ILanguageService
{
    Task<LanguageDto> GetLanguageByCodeAsync(string languageCode);
    Task<LanguageDto> GetLanguageByIdAsync(long languageId);
    Task<(List<LanguageDto> result, int count)> GetLanguagesAsync(MetaQueryModel metaQuery);
    Task<LanguageDto> CreateLanguageAsync(LanguageCreatedDto language);
}
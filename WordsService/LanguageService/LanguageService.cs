using DatabaseBroker.Repositories.Words;
using Entity.DataTransferObjects.Words;
using Entity.Exeptions;
using Entity.Models.ApiModels;
using Entity.Models.Words;
using Microsoft.EntityFrameworkCore;

namespace WordsService.LanguageService;

public class LanguageService
    (ILanguageRepository languageRepository) : ILanguageService
{
    public async Task<LanguageDto> GetLanguageByCodeAsync(string languageCode)
    {
        var language = await languageRepository.GetAllAsQueryable()
            .FirstOrDefaultAsync(l => l.Code == languageCode)
            ?? throw new NotFoundException($"Not found language by code: {languageCode}");
        
        return new LanguageDto(
            language.Id,
            language.Code,
            language.Name,
            language.IsDefault);
    }

    public async Task<LanguageDto> GetLanguageByIdAsync(long languageId)
    {
        var language = await languageRepository.GetAllAsQueryable()
                           .FirstOrDefaultAsync(l => l.Id == languageId)
                       ?? throw new NotFoundException($"Not found language by id: {languageId}");
        
        return new LanguageDto(
            language.Id,
            language.Code,
            language.Name,
            language.IsDefault);
    }

    public async Task<(List<LanguageDto> result, int count)> GetLanguagesAsync(MetaQueryModel metaQuery)
    {
        var query = languageRepository.GetAllAsQueryable();
        
        return (await query
            .Skip(metaQuery.Skip)
            .Take(metaQuery.Take)
            .Select(l =>
                new LanguageDto(l.Id, l.Code, l.Name, l.IsDefault))
            .ToListAsync(),
            query.Count());
    }

    public async Task<LanguageDto> CreateLanguageAsync(LanguageCreatedDto languageCreatedDto)
    {
        var language = await languageRepository.GetAllAsQueryable()
                           .FirstOrDefaultAsync(l => l.Code == languageCreatedDto.code);

        if (language != null)
            return new LanguageDto(
                language.Id,
                language.Code,
                language.Name,
                language.IsDefault);
        language = new Language()
        {
            Code = languageCreatedDto.code,
            Name = languageCreatedDto.name,
            IsDefault = languageCreatedDto.isDefault
        };

        language = await languageRepository.AddAsync(language);

        return new LanguageDto(
            language.Id,
            language.Code,
            language.Name,
            language.IsDefault);
    }
}
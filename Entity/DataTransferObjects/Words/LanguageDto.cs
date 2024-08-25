namespace Entity.DataTransferObjects.Words;

public record LanguageDto(
    long id,
    string code,
    string name,
    bool isDefault);
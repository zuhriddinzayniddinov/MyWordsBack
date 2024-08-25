namespace Entity.DataTransferObjects.Words;

public record LanguageCreatedDto(
    string code,
    string name,
    bool isDefault = false);
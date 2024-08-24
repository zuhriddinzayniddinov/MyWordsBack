namespace Entity.DataTransferObjects.Words;

public record GroupDto(
    long id,
    string name,
    long toLanguageId,
    string toLanguageName,
    long fromLanguageId,
    string fromLanguageName,
    int wordsCount);
namespace Entity.DataTransferObjects.Words;

public record GroupCreatedDto(
    string name,
    long toLanguageId,
    long? fromLanguageId);
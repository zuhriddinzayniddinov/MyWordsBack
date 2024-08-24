namespace Entity.DataTransferObjects.Words;

public record WordDto(
    long id,
    string text,
    string translate,
    long groupId);
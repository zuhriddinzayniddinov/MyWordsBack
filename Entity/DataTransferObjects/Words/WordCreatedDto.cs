namespace Entity.DataTransferObjects.Words;

public record WordCreatedDto(
    long groupId,
    string text,
    string translate);
namespace Entity.DataTransferObjects.Authentication;

public record UserRegisterDto(
    string firstName,
    string lastName,
    long nativeLanguageId,
    string phoneNumber,
    string password);
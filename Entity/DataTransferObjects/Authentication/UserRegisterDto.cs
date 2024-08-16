using Entity.Enum;

namespace Entity.DataTransferObjects.Authentication;

public record UserRegisterDto(
    string firstName,
    string lastName,
    Language nativeLanguage,
    string phoneNumber,
    string password);
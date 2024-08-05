namespace Entity.DataTransferObjects.Authentication;

public record UserRegisterDto(
    string firstName,
    string lastName,
    string phoneNumber,
    string password);
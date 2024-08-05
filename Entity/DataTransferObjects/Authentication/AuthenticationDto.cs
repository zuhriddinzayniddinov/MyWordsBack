namespace Entity.DataTransferObjects.Authentication;

public record AuthenticationDto(
    string phoneNumber,
    string password);
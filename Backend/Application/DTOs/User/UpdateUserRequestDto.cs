namespace Application.DTOs.User;

public record UpdateUserRequestDto
{
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}

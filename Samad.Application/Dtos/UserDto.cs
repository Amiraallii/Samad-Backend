namespace Samad.Application.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string NationalCode { get; set; }
        public string PasswordHash { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public record RegisterDto(string firstName,
        string lastName,
        string phoneNumber,
        string email,
        string nationalCode,
        string password,
        DateTime birthDate);
    public record LoginDto(string email,
        string password);

    public sealed record LoginResultDto(
    string AccessToken,
    string RefreshToken,
    int UserId,
    string FirstName,
    string LastName,
    string Email,
    string Role);
}

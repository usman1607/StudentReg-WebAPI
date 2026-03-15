namespace Application.Dtos.ResponseDto
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = default!;
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserType { get; set; } = default!;
        public List<string> Roles { get; set; } = new();
        public string? Delegation { get; set; }
    }
}

namespace SchoolApis.Model
{
    public class RefreshTokenRequestDto
    {
        public Guid Id { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}

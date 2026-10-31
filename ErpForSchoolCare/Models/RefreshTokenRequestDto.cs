namespace ErpForSchoolCare.Models
{
    public class RefreshTokenRequestDto
    {
        public Guid Id { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}

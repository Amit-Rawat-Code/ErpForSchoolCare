namespace ErpForSchoolCare.Models
{
    public class LoginResponseDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ErpForSchoolCare.Models
{
    public class User
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [PasswordValidate]
        [DataType(dataType:DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password",ErrorMessage ="Password not matched")]
        [DataType(dataType: DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;        
    }
}

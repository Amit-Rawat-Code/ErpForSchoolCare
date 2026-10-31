using System.ComponentModel.DataAnnotations;

namespace ErpForSchoolCare.Models
{
    public class MenuDto : Common
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Url { get; set; } = string.Empty;
        [Required]
        public string Icon { get; set; } = string.Empty;
        [Required]
        public int Order { get; set; }
    }
}

namespace SchoolApis.Model
{
    public class MenuDto : Common
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int? Order { get; set; }
    }

}

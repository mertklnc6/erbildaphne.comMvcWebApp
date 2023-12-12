namespace erbildaphne.comMvcWebApp.Models
{
    public class GNewsSourceViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string Name { get; set; }
        public SourceSide Side { get; set; }
        public enum SourceSide
        {
            Left,
            Right,
            Center
        }
        public string LogoUrl { get; set; }
        public string SiteUrl { get; set; }
    }
}

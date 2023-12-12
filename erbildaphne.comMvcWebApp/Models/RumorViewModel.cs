namespace erbildaphne.comMvcWebApp.Models
{
    public class RumorViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string Title { get; set; }

        public string Description { get; set; }
        public string Content { get; set; }

        public string PictureUrl { get; set; }

        public bool IsPublished { get; set; } = false;
    }
}

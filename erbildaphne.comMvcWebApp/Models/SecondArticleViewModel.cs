namespace erbildaphne.comMvcWebApp.Models
{
    public class SecondArticleViewModel
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string PictureUrl { get; set; }
        public int AuthorId { get; set; }
        public bool IsPublished { get; set; } = false;
        public bool IsBoosted { get; set; } = false;
        public bool IsChosen { get; set; } = false;
    }
}

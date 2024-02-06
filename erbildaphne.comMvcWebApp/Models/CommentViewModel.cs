namespace erbildaphne.comMvcWebApp.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public string Title { get; set; }

        public string Content { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsVerified { get; set; } = false;
    }
}

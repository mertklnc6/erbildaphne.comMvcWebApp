namespace erbildaphne.comMvcWebApp.Models
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Bio { get; set; }

        public string ProfilePicUrl { get; set; }

    }
}

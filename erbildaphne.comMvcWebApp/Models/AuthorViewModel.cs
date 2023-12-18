namespace erbildaphne.comMvcWebApp.Models
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string Name { get; set; }
        public string Description { get; set; }
        public string Bio { get; set; }
        public string Instagram { get; set; }
        public string Website { get; set; }
        public string Linkedin { get; set; }
        public string Facebook { get; set; }

        public string PictureUrl { get; set; }


    }
}

namespace erbildaphne.comMvcWebApp.Models
{
    public class GNewsViewModel
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }       

        public string PictureUrl { get; set; }

        public bool IsPublished { get; set; } = false;

        public int TotalSource { get; set; } 
        public int LeaningLeft { get; set; } 
        public int LeaningRight { get; set; } 
        public int LeaningCenter { get; set; } 

        public string LeftPercentage => CalculatePercentage(LeaningLeft);
        public string RightPercentage => CalculatePercentage(LeaningRight);
        public string CenterPercentage => CalculatePercentage(LeaningCenter);

        private string CalculatePercentage(int count)
        {
            if (TotalSource == 0) return "%0";
            var percentage = (double)count / TotalSource * 100;
            return $"%{percentage:N2}"; // N2, sayıyı yüzdelik formatta iki ondalık basamağa yuvarlar
        }

        
    }
}

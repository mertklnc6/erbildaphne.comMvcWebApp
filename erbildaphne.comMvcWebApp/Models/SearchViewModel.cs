namespace erbildaphne.comMvcWebApp.Models
{
    public class SearchViewModel
    {
        public List<ArticleViewModel> Articles { get; set; }
        public List<GNewsViewModel> GNews { get; set; }
        public List<RumorViewModel> Rumors { get; set; }
    }
}

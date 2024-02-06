using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace erbildaphne.comMvcWebApp.ViewComponents
{
    public class SearchViewComponent : ViewComponent
    {
        private IHttpClientFactory _httpClientFactory;

        public SearchViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string searchTerm)
        {
            if (searchTerm != null)
            {
                var viewModel = new SearchViewModel();
                var http = _httpClientFactory.CreateClient("Client");

                // Tüm Articles çek
                var articleResponse = await http.GetAsync("article");
                if (articleResponse.IsSuccessStatusCode)
                {
                    var articleData = await articleResponse.Content.ReadAsStringAsync();
                    viewModel.Articles = JsonConvert.DeserializeObject<List<ArticleViewModel>>(articleData);
                }

                // Tüm GNews çek
                var gNewsResponse = await http.GetAsync("gnews");
                if (gNewsResponse.IsSuccessStatusCode)
                {
                    var gNewsData = await gNewsResponse.Content.ReadAsStringAsync();
                    viewModel.GNews = JsonConvert.DeserializeObject<List<GNewsViewModel>>(gNewsData);
                }

                // Tüm Rumors çek
                var rumorResponse = await http.GetAsync("rumor");
                if (rumorResponse.IsSuccessStatusCode)
                {
                    var rumorData = await rumorResponse.Content.ReadAsStringAsync();
                    viewModel.Rumors = JsonConvert.DeserializeObject<List<RumorViewModel>>(rumorData);
                }

                // Arama terimine göre filtreleme
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    viewModel.Articles = viewModel.Articles?
                        .Where(a => a.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    viewModel.GNews = viewModel.GNews?
                        .Where(g => g.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    viewModel.Rumors = viewModel.Rumors?
                        .Where(r => r.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return View(viewModel);
            }
            return View();
        }


    }
}


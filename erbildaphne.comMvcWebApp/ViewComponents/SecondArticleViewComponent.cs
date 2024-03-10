using erbildaphne.comMvcWebApp.Models;
using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace erbildaphne.comMvcWebApp.ViewComponents
{
    public class SecondArticleViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SecondArticleViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var http = _httpClientFactory.CreateClient("Client");

            var result = await http.GetAsync("secondArticle/get/");
            var authorResult = await http.GetAsync("author/get/");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<SecondArticleViewModel>>(jsonData);
                var jsonAuthor = await authorResult.Content.ReadAsStringAsync();
                var authors = JsonConvert.DeserializeObject<List<AuthorViewModel>>(jsonAuthor);
                ViewBag.Authors = authors;
                //Json Serialize işlemleri için NewtonSoft paketini yüklüyoruz.
                return View(data);
            }
            else
            {
                // API'den başarısız bir yanıt geldiğinde hata sayfasını göster
                return View("Error");
            }

        }
    }
}

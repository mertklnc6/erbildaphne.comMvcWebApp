using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace erbildaphne.comMvcWebApp.ViewComponents
{
    public class MustReadViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MustReadViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var http = _httpClientFactory.CreateClient("Client");

            var result = await http.GetAsync("write");
            var authorResult = await http.GetAsync("author");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<WriteViewModel>>(jsonData);
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

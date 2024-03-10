using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace erbildaphne.comMvcWebApp.ViewComponents
{
    public class SourceViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SourceViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var http = _httpClientFactory.CreateClient("Client");

            var result = await http.GetAsync("source/get/");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<GNewsSourceViewModel>>(jsonData);                
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

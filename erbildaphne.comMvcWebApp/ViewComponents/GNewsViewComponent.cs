﻿using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace erbildaphne.comMvcWebApp.ViewComponents
{
    public class GNewsViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GNewsViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var http = _httpClientFactory.CreateClient("Client");

            var result = await http.GetAsync("gNews/get/");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<GNewsViewModel>>(jsonData);                
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

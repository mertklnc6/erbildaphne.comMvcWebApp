
using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace erbildaphne.comMvcWebApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            
            var http = _httpClientFactory.CreateClient("Client");
           
            var result = await http.GetAsync("author");            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<AuthorViewModel>>(jsonData);               
                
                
                return View(data);
            }
            else
            {                
                return View("Error");
            }


        }    
       

        

        public async Task<IActionResult> Details(int id)
        {
            var http = _httpClientFactory.CreateClient("Client");
            var result = await http.GetAsync("author" +"/" + id.ToString());
            var writeResult = await http.GetAsync("write");


            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<AuthorViewModel>(jsonData);
                var jsonWrites = await writeResult.Content.ReadAsStringAsync();
                var writes = JsonConvert.DeserializeObject<List<WriteViewModel>>(jsonWrites);
                ViewBag.Writes = writes;
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


using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using erbildaphne.comMvcWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace erbildaphne.comMvcWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var http = _httpClientFactory.CreateClient("Client");
            var jsonContent = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var result = await http.PostAsync("account/register/", content);
            var response = result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var token = await result.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JWTToken", token);
                // Kayıt başarılı, giriş sayfasına yönlendir
                return RedirectToAction("Login");
            }

            // Hata mesajlarını ModelState'e ekle
            ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var http = _httpClientFactory.CreateClient("Client");
            var jsonContent = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var result = await http.PostAsync("account/login/", content);
            var response = result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var token = await result.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JWTToken", token);
                return RedirectToAction("Index", "Admin");
                // Giriş başarılı, anasayfaya yönlendir               
            }

            ModelState.AddModelError("", "Giriş başarısız.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var http = _httpClientFactory.CreateClient("Client");
            await http.PostAsync("account/logout", null);

            // Çıkış başarılı, giriş sayfasına yönlendir
            return RedirectToAction("Login");
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Json;
using erbildaphne.comMvcWebApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace erbildaphne.comMvcWebApp.Controllers
{
    [Authorize(Roles ="admin")]
    public class RolesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RolesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Tüm rolleri görüntüle
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var http = _httpClientFactory.CreateClient("Client");
            var response = await http.GetAsync("roles/list");

            if (response.IsSuccessStatusCode)
            {
                var roles = await response.Content.ReadFromJsonAsync<List<RoleViewModel>>();
                return View(roles);
            }

            return View("Error");
        }

        // Rol oluşturma sayfası
        public IActionResult Create()
        {
            return View();
        }

        // Rol oluşturma işlemi
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            var jsonContent = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var result = await http.PostAsync("roles/create/", content);
            var response = result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        // Rol atama sayfası
        public IActionResult Assign()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            return View();
        }

        // Rol atama işlemi
        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            var jsonContent = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var result = await http.PostAsync("roles/assign/", content);
            var response = result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}

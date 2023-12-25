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
using System.Reflection;
using System.Security.Claims;

namespace erbildaphne.comMvcWebApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RoleController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Tüm rolleri görüntüle
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var roleClaims = User.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            if (roleClaims != null)
            {
                var response = await http.GetAsync("role/list");

                if (response.IsSuccessStatusCode)
                {
                    var roles = await response.Content.ReadFromJsonAsync<List<RoleViewModel>>();
                    return View(roles);
                }

                return View("Error");
            }


            return RedirectToAction("Login", "Account");
            
        }

        // Rol oluşturma sayfası
        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var roleClaims = User.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            if (roleClaims != null)
            {
                return View();
            }


            return RedirectToAction("Login", "Account");
        }

        // Rol oluşturma işlemi
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var roleClaims = User.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            if (roleClaims != null)
            {
                var jsonContent = JsonConvert.SerializeObject(model);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var result = await http.PostAsync("role/create/", content);
                var response = result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                return View(model);
            }


            return RedirectToAction("Login", "Account");

            
        }

        // Rol atama sayfası
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var roleClaims = User.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            if (roleClaims != null)
            {
                var result = await http.GetAsync("role" + "/" + id);
                var response = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    var roles = JsonConvert.DeserializeObject<UsersInOrOutViewModel>(response);
                    return View(roles);
                }

                return View("Error");
            }


            return RedirectToAction("Login", "Account");


            
            
        }

        // Rol atama işlemi
        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var roleClaims = User.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            if (roleClaims != null)
            {
                var jsonContent = JsonConvert.SerializeObject(model);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var result = await http.PutAsync("role/edit/", content);
                var response = result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                return View(model);
            }


            return RedirectToAction("Login", "Account");

            
        }
    }
}

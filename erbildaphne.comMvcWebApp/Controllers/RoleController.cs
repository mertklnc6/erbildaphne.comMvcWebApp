using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using System.Security.Claims;
using erbildaphne.comMvcWebApp.Models;

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

            var response = await http.GetAsync("role/get/");

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
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            return View();

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

            var jsonContent = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var result = await http.PostAsync("role/create/", content);
            var response = result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);


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

            var result = await http.GetAsync("role/users/" + id.ToString());
            var response = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var roles = JsonConvert.DeserializeObject<UsersInOrOutViewModel>(response);
                return View(roles);
            }

            return View("Error");





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

            var jsonContent = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");

            var result = await http.PutAsync("role/edit/", content);
            var response = result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);



        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var result = await http.DeleteAsync("role/delete/" + id.ToString());

            return RedirectToAction("Index");
        }
    }
}

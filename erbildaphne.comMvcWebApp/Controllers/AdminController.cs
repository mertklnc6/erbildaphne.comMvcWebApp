using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace erbildaphne.comMvcWebApp.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        //[Authorize(Roles = "admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            return View();
        }
    }
}

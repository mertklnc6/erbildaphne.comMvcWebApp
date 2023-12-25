using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace erbildaphne.comMvcWebApp.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public IActionResult Index()
        {
            
                var token = HttpContext.Session.GetString("JWTToken");
                
                if (token == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                
                var http = _httpClientFactory.CreateClient("Client");
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
                var roleClaims = User.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Editor");

                if(roleClaims != null)
                {
                    return View();
                }
                       
            
            return RedirectToAction("Login","Account");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using erbildaphne.comMvcWebApp.Models;

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

            var jsonToken = HttpContext.Session.GetString("JWTToken");
            if (jsonToken == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var http = _httpClientFactory.CreateClient("Client");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jsonToken);
            var token = JsonConvert.DeserializeObject<TokenViewModel>(jsonToken);
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token.Token);

            var roleClaims = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == "Editor");

            if (roleClaims != null)
            {
                return View();
            }


            return RedirectToAction("Login", "Account");
        }
    }
}

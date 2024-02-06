using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace erbildaphne.comMvcWebApp.Controllers
{
    public class RumorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RumorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            //Account controller Login ekranından alınan Username ve Password bilgileri, /api/Auth metoduna POST isteği olarak gönderilecek.
            //API gelen user bilgilerini doğruladıktan sonra ürettiği token bilgisini buraya (Mvc) gönderecek. Bu token bilgisi her istek öncesi aşağıdaki gibi request.Header içine eklenerek api'den listeler çekilebilecek.
            //var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIm5iZiI6MTY5NzI3NjY2OSwiZXhwIjoxNjk3Mjc2OTY5LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdCIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0In0.3QCXuBm4zjCJnGIteSsMtY33jUNpnl_MdAJVBLPeCPQ";
            var http = _httpClientFactory.CreateClient("Client");
            //http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var result = await http.GetAsync("rumor");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<RumorViewModel>>(jsonData);
                //Json Serialize işlemleri için NewtonSoft paketini yüklüyoruz.
                return View(data);
            }
            else
            {
                // API'den başarısız bir yanıt geldiğinde hata sayfasını göster
                return View("Error");
            }


        }
        [HttpGet]
        public async Task<IActionResult> List()
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
                var result = await http.GetAsync("rumor");
                var error = result.Content.ReadAsStringAsync();
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonData = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<RumorViewModel>>(jsonData);
                    //Json Serialize işlemleri için NewtonSoft paketini yüklüyoruz.
                    return View(data);
                }
                else
                {
                    // API'den başarısız bir yanıt geldiğinde hata sayfasını göster
                    return View("Error");
                }
            }


            return RedirectToAction("Login", "Account");


        }

        [HttpGet]
        
        public async Task<IActionResult> Create()
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
                return View(new RumorViewModel()); // Burada RumorViewModel nesnesi geçiriliyor.
            }


            return RedirectToAction("Login", "Account");


        }


        [HttpPost]

        public async Task<IActionResult> Create(RumorViewModel model)
        {
            // Resim yükleme



            // API'ye model verisini gönderme
            try
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
                    var jsonContent = JsonConvert.SerializeObject(model);

                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var result = await http.PostAsync("rumor", content);
                    var error = result.Content.ReadAsStringAsync();

                    if (result.IsSuccessStatusCode)
                    {
                        // Başarılı POST işleminden sonra yapılacak işlemler
                        return RedirectToAction("List");
                    }
                    else
                    {
                        // API'den başarısız bir yanıt geldiğinde hata sayfasını göster
                        return RedirectToAction("List");
                    }
                }


                return RedirectToAction("Login", "Account");


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "API iletişiminde bir hata oluştu: " + ex.Message);
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var http = _httpClientFactory.CreateClient("Client");
            var result = await http.GetAsync("rumor" + "/" + id.ToString());


            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<RumorViewModel>(jsonData);
                //Json Serialize işlemleri için NewtonSoft paketini yüklüyoruz.
                return View(data);
            }
            else
            {
                // API'den başarısız bir yanıt geldiğinde hata sayfasını göster
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Publish(int id)
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
                var rumorResult = await http.GetAsync("rumor/" + id);

                if (rumorResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var rumorJsonData = await rumorResult.Content.ReadAsStringAsync();
                    var rumorData = JsonConvert.DeserializeObject<RumorViewModel>(rumorJsonData);

                    // Yayın durumunu değiştir
                    rumorData.IsPublished = !rumorData.IsPublished;

                    // Güncellenmiş veriyi JSON'a dönüştür
                    var jsonContent = JsonConvert.SerializeObject(rumorData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // API'ye güncellenmiş veriyi gönder
                    var updateResult = await http.PutAsync("rumor/" + id.ToString(), content);
                    var error = updateResult.Content.ReadAsStringAsync();

                    return RedirectToAction("List");

                }

                // Hata durumunda
                return View("Error");
            }


            return RedirectToAction("Login", "Account");


        }

        [HttpGet]        
        public async Task<IActionResult> Edit(int id)
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
                var rumorResult = await http.GetAsync("rumor/" + id);


                if (rumorResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var rumorJsonData = await rumorResult.Content.ReadAsStringAsync();
                    var rumorData = JsonConvert.DeserializeObject<RumorViewModel>(rumorJsonData);

                    return View(rumorData);
                }
                else
                {
                    return View("Error");
                }
            }


            return RedirectToAction("Login", "Account");


        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, RumorViewModel model)
        {

            if (id == model.Id)
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
                    var rumorResult = await http.GetAsync("rumor/" + id);
                    var existing = await rumorResult.Content.ReadAsStringAsync();
                    var existingData = JsonConvert.DeserializeObject<RumorViewModel>(existing);
                    existingData.Title = model.Title;
                    existingData.Description = model.Description;
                    existingData.Content = model.Content;
                    var jsonContent = JsonConvert.SerializeObject(existingData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result = await http.PutAsync("rumor/" + id.ToString(), content);
                    var error = result.Content.ReadAsStringAsync();
                    return RedirectToAction("List");
                }


                return RedirectToAction("Login", "Account");






            }
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
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
                var rumorResult = await http.GetAsync("rumor/" + id);
                var existing = await rumorResult.Content.ReadAsStringAsync();
                var existingData = JsonConvert.DeserializeObject<RumorViewModel>(existing);
                if (existingData.IsDeleted)
                {
                    var result = await http.DeleteAsync("rumor/" + id.ToString());
                    var error = result.Content.ReadAsStringAsync();
                    return RedirectToAction("List");
                }
                else
                {

                    existingData.IsDeleted = true;
                    var jsonContent = JsonConvert.SerializeObject(existingData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result2 = await http.PutAsync("rumor/" + id.ToString(), content);
                    var error = result2.Content.ReadAsStringAsync();
                    return RedirectToAction("List");
                }
            }


            return RedirectToAction("Login", "Account");



        }

        [HttpGet]
        public async Task<IActionResult> RemoveDelete(int id)
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
                var rumorResult = await http.GetAsync("rumor/" + id);
                var existing = await rumorResult.Content.ReadAsStringAsync();
                var existingData = JsonConvert.DeserializeObject<RumorViewModel>(existing);
                if (existingData.IsDeleted)
                {
                    existingData.IsDeleted = false;
                    var jsonContent = JsonConvert.SerializeObject(existingData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result2 = await http.PutAsync("rumor/" + id.ToString(), content);
                    var error = result2.Content.ReadAsStringAsync();
                    return RedirectToAction("List");
                }
                return RedirectToAction("List");
            }


            return RedirectToAction("Login", "Account");


        }


    }
}

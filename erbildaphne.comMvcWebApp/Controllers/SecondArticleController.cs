using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
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
    public class SecondArticleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SecondArticleController(IHttpClientFactory httpClientFactory)
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
            var result = await http.GetAsync("secondArticle");
            var AuthorResult = await http.GetAsync("Author");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<SecondArticleViewModel>>(jsonData);
                var jsonAuthor = await AuthorResult.Content.ReadAsStringAsync();
                var Authors = JsonConvert.DeserializeObject<List<AuthorViewModel>>(jsonAuthor);
                ViewBag.Authors = Authors;
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
                var result = await http.GetAsync("secondArticle");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonData = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<SecondArticleViewModel>>(jsonData);
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
        public async Task<IActionResult> Create(int id)
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
                var result = await http.GetAsync("Author");

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonData = await result.Content.ReadAsStringAsync();
                    var Authors = JsonConvert.DeserializeObject<List<AuthorViewModel>>(jsonData);
                    ViewBag.Authors = new SelectList(Authors, "Id", "Name");
                    return View(new SecondArticleViewModel()); // Burada SecondArticleViewModel nesnesi geçiriliyor.
                }
                else
                {
                    return View("Error");
                }
            }


            return RedirectToAction("Login", "Account");

            



        }


        [HttpPost]
        
        public async Task<IActionResult> Create(SecondArticleViewModel model, IFormFile image)
        {
            // Resim yükleme
            try
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\secondArticles");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string imageFileExtension = Path.GetExtension(image.FileName);
                string uniqueImageFileName = $"{Guid.NewGuid()}{imageFileExtension}";
                var imagePath = Path.Combine(uploadPath, uniqueImageFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                model.PictureUrl = "/uploads/secondArticles/" + uniqueImageFileName;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                return View(model);
            }

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



                    var result = await http.PostAsync("secondArticle", content);

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


        public async Task<IActionResult> Details(int id)
        {
            var http = _httpClientFactory.CreateClient("Client");
            var result = await http.GetAsync("secondArticle" + "/" + id.ToString());
            var AuthorResult = await http.GetAsync("Author");


            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<SecondArticleViewModel>(jsonData);
                var jsonAuthor = await AuthorResult.Content.ReadAsStringAsync();
                var Authors = JsonConvert.DeserializeObject<List<AuthorViewModel>>(jsonAuthor);
                ViewBag.Authors = Authors;
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
                var secondArticleResult = await http.GetAsync("secondArticle/" + id);
                var AuthorResult = await http.GetAsync("Author");

                if (secondArticleResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var secondArticleJsonData = await secondArticleResult.Content.ReadAsStringAsync();
                    var secondArticleData = JsonConvert.DeserializeObject<SecondArticleViewModel>(secondArticleJsonData);

                    var AuthorJsonData = await AuthorResult.Content.ReadAsStringAsync();
                    var Authors = JsonConvert.DeserializeObject<List<AuthorViewModel>>(AuthorJsonData);

                    ViewBag.Authors = new SelectList(Authors, "Id", "Name", secondArticleData.AuthorId);
                    return View(secondArticleData);
                }
                else
                {
                    return View("Error");
                }
            }


            return RedirectToAction("Login", "Account");

            
        }        

        [HttpPost]

        public async Task<IActionResult> Edit(int id, SecondArticleViewModel model, IFormFile? image)
        {

            if (id == model.Id)
            {
                if (image != null)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\secondArticle");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string imageFileExtension = Path.GetExtension(image.FileName);
                    string uniqueImageFileName = $"{Guid.NewGuid()}{imageFileExtension}";
                    var imagePath = Path.Combine(uploadPath, uniqueImageFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    model.PictureUrl = "/uploads/secondArticle/" + uniqueImageFileName;
                }


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
                    var secondArticleResult = await http.GetAsync("secondArticle/" + id);
                    var existing = await secondArticleResult.Content.ReadAsStringAsync();
                    var existingData = JsonConvert.DeserializeObject<SecondArticleViewModel>(existing);
                    
                    existingData.AuthorId = model.AuthorId;
                    existingData.IsPublished = model.IsPublished;
                    existingData.IsDeleted = model.IsDeleted;
                    existingData.Content = model.Content;                    
                    existingData.CreatedDate = model.CreatedDate;
                    existingData.Description = model.Description;
                    existingData.IsBoosted = model.IsBoosted;
                    existingData.IsChosen = model.IsChosen;
                    existingData.Title = model.Title;



                    var jsonContent = JsonConvert.SerializeObject(existingData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result = await http.PutAsync("secondArticle/" + id, content);
                    var error = result.Content.ReadAsStringAsync();
                    return RedirectToAction("List");
                }


                return RedirectToAction("Login", "Account");






            }
            return RedirectToAction("List");
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
                var secondArticleResult = await http.GetAsync("secondArticle/" + id);

                if (secondArticleResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var secondArticleJsonData = await secondArticleResult.Content.ReadAsStringAsync();
                    var secondArticleData = JsonConvert.DeserializeObject<SecondArticleViewModel>(secondArticleJsonData);

                    // Yayın durumunu değiştir
                    secondArticleData.IsPublished = !secondArticleData.IsPublished;

                    // Güncellenmiş veriyi JSON'a dönüştür
                    var jsonContent = JsonConvert.SerializeObject(secondArticleData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // API'ye güncellenmiş veriyi gönder
                    var updateResult = await http.PutAsync("secondArticle/" + id, content);


                    return RedirectToAction("List");

                }


                // Hata durumunda
                return View("Error");
            }


            return RedirectToAction("Login", "Account");

            
        }

        public async Task<IActionResult> Boost(int id)
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
                var secondArticleResult = await http.GetAsync("secondArticle/" + id);

                if (secondArticleResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var secondArticleJsonData = await secondArticleResult.Content.ReadAsStringAsync();
                    var secondArticleData = JsonConvert.DeserializeObject<SecondArticleViewModel>(secondArticleJsonData);

                    // Yayın durumunu değiştir
                    secondArticleData.IsBoosted = !secondArticleData.IsBoosted;

                    // Güncellenmiş veriyi JSON'a dönüştür
                    var jsonContent = JsonConvert.SerializeObject(secondArticleData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // API'ye güncellenmiş veriyi gönder
                    var updateResult = await http.PutAsync("secondArticle/" + id, content);


                    return RedirectToAction("List");

                }


                // Hata durumunda
                return View("Error");
            }


            return RedirectToAction("Login", "Account");

           
        }
        public async Task<IActionResult> Chose(int id)
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
                var secondArticleResult = await http.GetAsync("secondArticle/" + id);

                if (secondArticleResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var secondArticleJsonData = await secondArticleResult.Content.ReadAsStringAsync();
                    var secondArticleData = JsonConvert.DeserializeObject<SecondArticleViewModel>(secondArticleJsonData);

                    // Yayın durumunu değiştir
                    secondArticleData.IsChosen = !secondArticleData.IsChosen;

                    // Güncellenmiş veriyi JSON'a dönüştür
                    var jsonContent = JsonConvert.SerializeObject(secondArticleData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // API'ye güncellenmiş veriyi gönder
                    var updateResult = await http.PutAsync("secondArticle/" + id, content);


                    return RedirectToAction("List");

                }


                // Hata durumunda
                return View("Error");
            }


            return RedirectToAction("Login", "Account");

           
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
                var secondArticleResult = await http.GetAsync("secondArticle/" + id);
                var existing = await secondArticleResult.Content.ReadAsStringAsync();
                var existingData = JsonConvert.DeserializeObject<SecondArticleViewModel>(existing);
                if (existingData.IsDeleted)
                {
                    var result = await http.DeleteAsync("secondArticle/" + id.ToString());
                    var error = result.Content.ReadAsStringAsync();
                    return RedirectToAction("List");
                }
                else
                {

                    existingData.IsDeleted = true;
                    var jsonContent = JsonConvert.SerializeObject(existingData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result2 = await http.PutAsync("secondArticle/" + id, content);
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
                var secondArticleResult = await http.GetAsync("secondArticle/" + id);
                var existing = await secondArticleResult.Content.ReadAsStringAsync();
                var existingData = JsonConvert.DeserializeObject<SecondArticleViewModel>(existing);
                if (existingData.IsDeleted)
                {
                    existingData.IsDeleted = false;
                    var jsonContent = JsonConvert.SerializeObject(existingData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result2 = await http.PutAsync("secondArticle/" + id.ToString(), content);
                    var error = result2.Content.ReadAsStringAsync();
                    return RedirectToAction("List");
                }
                return RedirectToAction("List");
            }


            return RedirectToAction("Login", "Account");


        }






    }
}

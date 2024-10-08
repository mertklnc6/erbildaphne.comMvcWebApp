﻿using erbildaphne.comMvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;

namespace erbildaphne.comMvcWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var http = _httpClientFactory.CreateClient("Client");
            //http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var result = await http.GetAsync("mainNews");            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<MainNewsViewModel>>(jsonData);               
                //Json Serialize işlemleri için NewtonSoft paketini yüklüyoruz.
                return View(data);
            }
            else
            {
                // API'den başarısız bir yanıt geldiğinde hata sayfasını göster
                return View("Error");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
        {            
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(model.Email);
                    mail.To.Add("edkusbakisi@gmail.com");
                    mail.Subject = model.Subject;
                    mail.Body = $"Name: {model.Name}\nEmail: {model.Email}\nMessage: {model.Message}";

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("edkusbakisi@gmail.com", ""); // Gmail kullanıcı adı ve şifresi
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }

                    ViewBag.Message = "Mesaj başarıyla gönderildi.";
                }
                catch (Exception ex)
                {
                    // Hata yönetimi
                    ViewBag.Error = "Mesaj gönderilirken bir hata oluştu: " + ex.Message;
                }
            }
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Search(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;
            // Diğer işlemler
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
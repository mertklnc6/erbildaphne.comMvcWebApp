using System.ComponentModel.DataAnnotations;

namespace erbildaphne.comMvcWebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email boş geçilemez")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre boş geçilemez")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        //public string ReturnUrl { get; set; }
    }
}

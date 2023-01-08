using System.ComponentModel.DataAnnotations;

namespace CompanyApp.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Почта не указана")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Пароль не указан")]       
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace _WebApp.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="Enter Full name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="Password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

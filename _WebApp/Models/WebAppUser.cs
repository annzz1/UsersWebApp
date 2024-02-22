using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _WebApp.Models
{
    public class WebAppUser:IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Name {  get; set; }
        public DateTime? LastLoginTime { get; set; } = DateTime.Now;
        public DateTime? RegisterTime { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}

using _WebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _WebApp.ApplicationDbContext
{
    public class AppDbContext:IdentityDbContext<WebAppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
            
        }
    }
}

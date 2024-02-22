using _WebApp.ViewModels;
using _WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using _WebApp.ApplicationDbContext;

namespace CustomIdentity.Controllers;

public class AccountController: Controller
{
    private readonly SignInManager<WebAppUser> signInManager;
    private readonly UserManager<WebAppUser> userManager;
    private readonly AppDbContext _context;
    public AccountController(SignInManager<WebAppUser> signInManager, UserManager<WebAppUser> userManager,AppDbContext context)
    {
        this.userManager = userManager;
        this.signInManager=signInManager;
        _context = context;
    }
    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View(await userManager.Users.ToListAsync());
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model)
    {
        if (ModelState.IsValid)
        {
            
            var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);
            var user = await userManager.FindByEmailAsync(model.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "You do not have an account! Please register first!");
            }
            else if (user.IsActive == false)
            {
                ModelState.AddModelError("", "Your Account is currently Blocked!");
            }
            else if (result.Succeeded)
            {
                user.LastLoginTime = DateTime.Now;
                await userManager.UpdateAsync(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(model);
        }
        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (ModelState.IsValid)
        {
            WebAppUser user = new()
            {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email,
               
            };

            var result = await userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(string userIds)
    {
        List<string> UserIds = userIds.Split(',').ToList();
        if (UserIds.IsNullOrEmpty())
        {
            return BadRequest();
        }
        var currentUser = await userManager.GetUserAsync(User);
        foreach (var id in UserIds)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
           
        }
        if (currentUser != null && UserIds.Contains(currentUser.Id))
        {
           
            await signInManager.SignOutAsync();
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Block(string userIds)
    {
        var UserIds = userIds.Split(',').ToList();
        if (UserIds == null || UserIds.Count == 0)
        {
            return BadRequest("No users selected for blocking.");
        }
       var currentUser = await userManager.GetUserAsync(User);
        foreach (var userId in UserIds)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = false;
                await userManager.UpdateAsync(user);
            }
        }
        if(currentUser!=null && UserIds.Contains(currentUser.Id))
        {
            currentUser.IsActive = false;
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        
       
        return RedirectToAction("Index"); 
    }

    [HttpPost]
    public async Task<IActionResult> unBlock(string userIds)
    {
        List<string> UserIds = userIds.Split(',').ToList();
        if (UserIds == null || UserIds.Count == 0)
        {
            return BadRequest("No users selected for blocking.");
        }
       
        foreach (var userId in UserIds)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = true;
                await userManager.UpdateAsync(user);
            }
        }
        
        return RedirectToAction("Index"); 
    }
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
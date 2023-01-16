using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        UserManager<AppUser> _userManager { get; }
        SignInManager<AppUser> _signInManager { get; }

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVM regVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = new AppUser()
            {
                Name = regVM.Name,
                Surname = regVM.Surname,
                UserName = regVM.Username,
                Email = regVM.Email
            };
            IdentityResult result = await _userManager.CreateAsync(user, regVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Index","Home");
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVM loginVM)
        {
            if (!ModelState.IsValid) return Json(ModelState);

            AppUser appUser = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);

            if(appUser is null)
            {
                appUser = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
                if(appUser is null)
                {
                    ModelState.AddModelError("Other", "Username or Password is wrong");
                    return Json(ModelState);
                }
            }

            var result = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.IsPresistance,true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("Other", "Username or Password is wrong");
                return Json(ModelState);
            }
            return Content("Ok");
        }
    }
}

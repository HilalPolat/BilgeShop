using System.Security.Claims;
using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BilgeShop.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public string? CookieAuthenticationDefault { get; private set; }

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("kayit-ol")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Route("kayit-ol")]
        public IActionResult Register(RegisterViewModel formData)
        {
            if(!ModelState.IsValid)
            {
                return View(formData);
            }

            var userDto = new UserDto()
            {
                FirstName = formData.FirstName.Trim(),
                LastName = formData.LastName.Trim(),
                Email = formData.Email.Trim(),
                Password = formData.Password.Trim(),
            };

           var response= _userService.AddUser(userDto);
            if (response.IsSucceed)
            {
                return RedirectToAction("Index", "home");
            }
            else
            {
                ViewBag.ErrorMessage = response.Message;
                return View(formData);
            }
            
        }

        public  async Task <IActionResult> Login(LoginViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginMessage"] = "Kullanıcı Adı ve Şifre alanını doldurunuz.";
            }
            var loginDto = new LoginDto()
            {
                Email = formData.Email.Trim(),
                Password = formData.Password.Trim(),
            };

            var user=_userService.Login(loginDto);
            if(user is null)
            {
                TempData["LoginMessage"] = "Kullanıcı adı veya şifreyi hatalı girdiniz.";
                return RedirectToAction("index", "home");
            }
            var claims = new List<Claim>();
            claims.Add(new Claim("id", user.Id.ToString()));    
            claims.Add(new Claim("email", user.Email));
            claims.Add(new Claim("firstName", user.FirstName));
            claims.Add(new Claim("lastName", user.LastName));
            claims.Add(new Claim("userType", user.UserType.ToString()));

            claims.Add(new Claim(ClaimTypes.Role, user.UserType.ToString()));

            var claimIdentity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            var autProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(48)),
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), autProperties);
            return RedirectToAction("index", "home");

        }
        public async Task<IActionResult> Logout()
		{
            await HttpContext.SignOutAsync();
            return RedirectToAction("index", "home");
		}
    }
}

using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Extensions;
using BilgeShop.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BilgeShop.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [Route("Hesabim")]
        public IActionResult Index()
        {
            var viewModel = new AccountViewModel()
            {
                FirstName = User.GetUserFirstName(),
                LastName = User.GetUserLastName(),
                Email = User.GetUserEmail(),
                EmailConfirm=User.GetUserEmail()
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Update(AccountViewModel formData )
        {
            var loggedUserId = User.GetUserId();
            if (!ModelState.IsValid)
            {
                return View("index", formData);
            }

            var userProfileEditDto = new UserProfileEditDto()
            {
                FirstName = formData.FirstName.Trim(),
                LastName = formData.LastName.Trim(),
                Email = formData.Email.Trim(),
                Id = User.GetUserId()
            };

            _userService.UpdateUser(userProfileEditDto);

            return RedirectToAction("index", "home");
        }
    }
}

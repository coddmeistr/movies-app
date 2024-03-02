using FilmLibrary.Business.Interfaces;
using FilmLibrary.Models.Models;
using FilmLibrary.UI.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FilmLibrary.UI.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IAuthentificationService _authentificationService;
        private readonly IUserService _userService;

        public UserController(IAuthentificationService authentificationService, IUserService userService, ILogger<UserController> logger)
        {
            _authentificationService = authentificationService ?? throw new ArgumentNullException(nameof(authentificationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(CredentialsModel model)
        {
            if (ModelState.IsValid)
            {
                var userModel = _authentificationService.Authenticate(model);
                if (string.IsNullOrEmpty(userModel.Name))
                {
                    ModelState.AddModelError(String.Empty, ModelErrorMessages.IncorrectLoginCredentialsMessage);
                    return View(model);
                }

                string userName = userModel.Name;
                string userRole = userModel.UserRole;

                Authenticate(ref userName, ref userRole);

                return RedirectToAction("LaunchStartPage", "Home");
            }

            ModelState.AddModelError(String.Empty, ModelErrorMessages.IncorrectLoginCredentialsMessage);
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(CredentialsModel model)
        {   
            if (ModelState.IsValid)
            {
                var userModel = _userService.CreateUser(model);

                if (string.IsNullOrEmpty(userModel.Name))
                {
                    ModelState.AddModelError(String.Empty, ModelErrorMessages.UserExistsMessage);
                    return View("User exists");
                }

                string userName = userModel.Name;
                string userRole = userModel.UserRole;

                Authenticate(ref userName, ref userRole);

                return RedirectToAction("LaunchStartPage", "Home");
            }

            ModelState.AddModelError(String.Empty, ModelErrorMessages.IncorrectLoginDetailsMessage);
            return View("Model state is not valid");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");

            return RedirectToAction("Login", "User");
        }

        private void Authenticate(ref string userName, ref string userRole)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            try
            {
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                _logger.LogInformation("User logged in");
            }
            catch (NullReferenceException) { _logger.LogError(string.Empty, "Cannot create ClaimsPrincipal instance"); }
        }
    }
}
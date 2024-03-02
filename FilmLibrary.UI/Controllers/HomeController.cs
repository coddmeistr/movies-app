using FilmLibrary.Business.Interfaces;
using FilmLibrary.Core.Constants;
using FilmLibrary.Core.Enums;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using FilmLibrary.UI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmLibrary.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IMovieService _movieService;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IMovieService movieService,
            IUserRepository userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public IActionResult LaunchStartPage(UserViewModel collectionModel)
        {
            if (!string.IsNullOrEmpty(collectionModel.UserName))
            {
                string userName = collectionModel.UserName;
                collectionModel = _movieService.GetCommonMovieCollection(UserRole.customer.ToString(), collectionModel.UserName);
                collectionModel.UserName = userName;

                return View(collectionModel);
            }
            else
            {
                if (User.Identity is null)
                {
                    ModelState.AddModelError(String.Empty, ModelErrorMessages.NoSuchUserMessage);
                    _logger.LogError(LogMessages.UserNotFoundMessage);

                    return View();
                }

                var roleClaim = User.Claims.ToList()[1].ToString();
                string role = _userService.GetUserRole(roleClaim);

                var userName = User.Identity.Name;

                if (string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError(String.Empty, ModelErrorMessages.NoSuchUserMessage);
                    _logger.LogError(LogMessages.UserNotFoundMessage);
                    return View("User name is empty");
                }
                
                UserViewModel model = _movieService.GetCommonMovieCollection(role, userName);
                ViewBag.AllUsers = new SelectList(_userRepository.GetAll().Where(u => u.UserRole.ToString().Equals("customer")),
                    "Id", "Name");

                return View(model);
            }
        }
    }
}
using FilmLibrary.Business.Interfaces;
using FilmLibrary.Core.Constants;
using FilmLibrary.Models.Models;
using FilmLibrary.UI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmLibrary.UI.Controllers
{
    public class MovieController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, IGenreService genreService, ILogger<MovieController> logger,
            IArtistService artistService)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult CreateMovie()
        {
            ViewBag.Genres = new SelectList(_genreService.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult CreateMovie(CreateMovieModel model)
        {
            if (ModelState["FirstArtist"]?.Errors.Count == 0 || ModelState["SecondArtist"]?.Errors.Count == 0 || ModelState.IsValid)
            {
                _movieService.CreateMovie(model);
                return RedirectToAction("LaunchStartPage", "Home");
            }

            ModelState.AddModelError(String.Empty, ModelErrorMessages.IncorrectMovieInfoMessage);
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddMovieToFavouriteCollection(string movieName)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity is null || string.IsNullOrEmpty(User.Identity.Name))
                {
                    ModelState.AddModelError(String.Empty, ModelErrorMessages.NoSuchUserMessage);
                    _logger.LogError(LogMessages.UserNotFoundMessage);

                    return RedirectToAction("LaunchStartPage", "Home");
                }

                var userName = User.Identity.Name;

                _movieService.AddOrRemoveMovieIfExistsFromFavouriteCollection(movieName, userName);

                return RedirectToAction("LaunchStartPage", "Home");
            }

            ModelState.AddModelError(String.Empty, ModelErrorMessages.IncorrectMovieNameMessage);
            return RedirectToAction("LaunchStartPage", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteMovieFromGeneralList(string movieName)
        {
            if (ModelState.IsValid)
            {
                _movieService.DeleteMovieFromCommonCollection(movieName);
                return RedirectToAction("LaunchStartPage", "Home");
            }

            return RedirectToAction("LaunchStartPage", "Home");
        }

        [Authorize]
        public IActionResult ShowUserFavouriteMovies(UserViewModel collectionModel)
        {
            if (collectionModel != null && !string.IsNullOrEmpty(collectionModel.UserName))
            {
                FavouriteMoviesCollectionViewModel moviesModel = _movieService.GetUserFavouriteMoviesById(collectionModel.UserName);

                return View(moviesModel);
            }

            if (User.Identity is null)
            {
                ModelState.AddModelError(String.Empty, ModelErrorMessages.NoSuchUserMessage);
                _logger.LogError(LogMessages.UserNotFoundMessage);

                return View();
            }

            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError(String.Empty, ModelErrorMessages.NoSuchUserMessage);
                _logger.LogError(LogMessages.UserNotFoundMessage);
                return View("Username is empty");
            }

            FavouriteMoviesCollectionViewModel model = _movieService.GetUserFavouriteMoviesByName(userName);

            return View(model);
        } 

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EditMovieInfo(string movieName)
        {
            var movieModel = _movieService.GetMovieByName(movieName);
            var updateModel = new UpdateMovieModel() { Name = movieModel.Name, Description = movieModel.Description,
                Articul = movieModel.Articul };
            ViewBag.GenresId = new SelectList(_genreService.GetAll(), "Id", "Name");
            ViewBag.ArtistsId = new SelectList(_artistService.GetAll(), "Id", "Name");

            if (string.IsNullOrEmpty(movieModel.Name))
            {
                _logger.LogError("Movie name is empty");
                return View("Movie name is empty");
            }

            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult EditMovieInfo(UpdateMovieModel model)
        {
            _movieService.EditMovieInfo(model);
            return RedirectToAction("LaunchStartPage", "Home");
        }

        [Authorize]
        public IActionResult ShowMovieByName(string name)
        {
            var movieModel = _movieService.GetMovieByName(name);

            if (string.IsNullOrEmpty(movieModel.Name))
            {
                _logger.LogError("Movie name is empty");
                return View("Movie name is empty");
            }

            return View(movieModel);
        }
    }
}
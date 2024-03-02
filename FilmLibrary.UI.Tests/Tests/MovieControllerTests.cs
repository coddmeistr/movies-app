using FilmLibrary.Business.Interfaces;
using FilmLibrary.Models.Models;
using FilmLibrary.UI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace FilmLibrary.UI.Tests
{
    [TestFixture]
    public class MovieControllerTests
    {
        private Mock<ControllerContext> _controllerContext;
        private Mock<ILogger<MovieController>> _logger;
        private Mock<IGenreService> _genreService;
        private Mock<IMovieService> _movieService;
        private Mock<IArtistService> _artistService;
        private MovieController _movieController;

        [SetUp]
        public void Setup()
        {
            _controllerContext = new Mock<ControllerContext>();
            _logger = new Mock<ILogger<MovieController>>();
            _genreService = new Mock<IGenreService>();
            _movieService = new Mock<IMovieService>();
            _artistService = new Mock<IArtistService>();
            _movieController = new MovieController(_movieService.Object, _genreService.Object, _logger.Object, _artistService.Object);
            _movieController.ControllerContext = _controllerContext.Object;
        }

        [Test]
        public void CreateMovie_WhenModelStateIsInvalid_ShouldAddModelError()
        {
            var movieModel = new CreateMovieModel() { Name = "Test name" };
            _movieController.ModelState.AddModelError(string.Empty, "Test error");
            var expectedErrorCount = 2;

            var result = _movieController.CreateMovie(movieModel) as ViewResult;

            Assert.AreEqual(expectedErrorCount, _movieController.ModelState.ErrorCount);
        }

        [Test]
        public void CreateMovie_WhenModelStateIsValid_ShouldReturnRedirectToActionResult()
        {
            var movieModel = new CreateMovieModel() { Name ="Test name" };
            _movieService.Setup(m => m.CreateMovie(movieModel));
            var expectedErrorCount = 0;

            var result = _movieController.CreateMovie(movieModel);

            Assert.AreEqual(expectedErrorCount, _movieController.ModelState.ErrorCount);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
        
        [Test]
        public void AddMovieToFavouriteCollection_WhenModelStateIsInvalid_ShouldAddModelError()
        {
            string movieName = "Big bang theory";
            _movieController.ModelState.AddModelError("Test error", "Test");
            var expectedErrorCount = 2;

            var result = _movieController.AddMovieToFavouriteCollection(movieName);

            Assert.AreEqual(expectedErrorCount, _movieController.ModelState.ErrorCount);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void AddMovieToFavouriteCollection_WhenModelStateIsValidAndUserIdentityNameIsEmpty_ShouldAddModelError()
        {
            string movieName = "Big bang theory";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, string.Empty),
            }, "mock"));
            _movieController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var expectedErrorCount = 1;

            var result = _movieController.AddMovieToFavouriteCollection(movieName);

            Assert.AreEqual(expectedErrorCount, _movieController.ModelState.ErrorCount);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void AddMovieToFavouriteCollection_WhenModelStateIsValidAndUserIdentityNameIsValid_ShouldReturnRedirectToActionResult()
        {
            string movieName = "Big bang theory";
            string userName = "George";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userName),
            }, "mock"));
            _movieController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var expectedErrorCount = 0;

            var result = _movieController.AddMovieToFavouriteCollection(movieName) as ActionResult;

            Assert.AreEqual(expectedErrorCount, _movieController.ModelState.ErrorCount);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void DeleteMovieFromGeneralList_WhenModelStateIsValid_ShouldReturnRedirectToActionResult()
        {
            _movieController.ModelState.Clear();
            string movieName = "Hello, kitty";
            _movieService.Setup(m => m.DeleteMovieFromCommonCollection(movieName));

            var result = _movieController.DeleteMovieFromGeneralList(movieName);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void DeleteMovieFromGeneralList_WhenModelStateIsInvalid_ShouldReturnRedirectToActionResult()
        {
            _movieController.ModelState.AddModelError("Test", "Test error");
            string movieName = "Hello, kitty";

            var result = _movieController.DeleteMovieFromGeneralList(movieName);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void ShowUserFavouriteMovies_WhenUserNameIsSet_ShouldReturnViewWithCollectionViewModel()
        {
            var collectionViewModel = new UserViewModel() { UserName = "5" };
            _movieService.Setup(m => m.GetUserFavouriteMoviesById(collectionViewModel.UserName)).Returns(new FavouriteMoviesCollectionViewModel());

            var result = _movieController.ShowUserFavouriteMovies(collectionViewModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<FavouriteMoviesCollectionViewModel>(result?.Model);
        }

        [Test]
        public void ShowUserFavouriteMovies_WhenUserNameIsNotSetAndIdentityNameIsEmpty_ShouldAddModelError()
        {
            var collectionViewModel = new UserViewModel();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, string.Empty),
            }, "mock"));
            _movieController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var expectedMessage = "Username is empty";
            var expectedErrorCount = 1;

            var result = _movieController.ShowUserFavouriteMovies(collectionViewModel) as ViewResult;

            Assert.AreEqual(expectedErrorCount, _movieController.ModelState.ErrorCount);
            Assert.AreEqual(expectedMessage, result?.ViewName);
        }

        [Test]
        public void ShowUserFavouriteMovies_WhenUserNameIsNotSetAndIdentityNameIsNotEmpty_ShouldReturnViewWithCollectionViewModel()
        {
            var collectionViewModel = new UserViewModel();
            string userName = "George";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userName),
            }, "mock"));
            _movieController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _movieService.Setup(m => m.GetUserFavouriteMoviesByName(userName)).Returns(new FavouriteMoviesCollectionViewModel());

            var result = _movieController.ShowUserFavouriteMovies(collectionViewModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<FavouriteMoviesCollectionViewModel>(result?.Model);
        }

        [Test]
        public void EditMovieInfoGet_WhenMovieNameIsEmpty_ShouldReturnActionResult()
        {
            string movieName = "Now you see me";
            _movieService.Setup(m => m.GetMovieByName(movieName)).Returns(new MovieViewModel());

            var result = _movieController.EditMovieInfo(movieName) as ViewResult;
            var expectedMessage = "Movie name is empty";

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result?.ViewName);
        }

        [Test]
        public void EditMovieInfoGet_WhenMovieNameIsNotEmpty_ShouldReturnViewWithUpdateMovieModel()
        {
            string movieName = "Now you see me";
            _movieService.Setup(m => m.GetMovieByName(movieName)).Returns(new MovieViewModel() { Name = "1+1" });

            var result = _movieController.EditMovieInfo(movieName) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<UpdateMovieModel>(result?.Model);
        }

        [Test]
        public void EditMovieInfoPost_WhenCreateUpdateMovieModelIsSet_ShouldReturnRedirectToActionResult()
        {
            var model = new UpdateMovieModel() { Name = "Test name", Description ="Test description", Articul = 100 };
            _movieService.Setup(m => m.EditMovieInfo(model));

            var result = _movieController.EditMovieInfo(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void ShowMovieByName_WhenMovieNameIsEmpty_ShouldReturnActionResult()
        {
            string movieName = "Now you see me";
            _movieService.Setup(m => m.GetMovieByName(movieName)).Returns(new MovieViewModel());
            var expectedMessage = "Movie name is empty";

            var result = _movieController.ShowMovieByName(movieName) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result?.ViewName);
        }

        [Test]
        public void ShowMovieByName_WhenMovieNameIsNotEmpty_ShouldReturnViewWithMovieViewModel()
        {
            string movieName = "Now you see me";
            _movieService.Setup(m => m.GetMovieByName(movieName)).Returns(new MovieViewModel() { Name = "1+1" });

            var result = _movieController.ShowMovieByName(movieName) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<MovieViewModel>(result?.Model);
        }
    }
}

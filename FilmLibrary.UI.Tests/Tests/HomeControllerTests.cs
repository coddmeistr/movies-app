using FilmLibrary.Business.Interfaces;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using FilmLibrary.UI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;

namespace FilmLibrary.UI.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ControllerContext> _controllerContext;
        private Mock<ILogger<HomeController>> _logger;
        private Mock<IUserService> _userService;
        private Mock<IMovieService> _movieService;
        private Mock<IUserRepository> _userRepository;
        private HomeController _homeController;

        [SetUp]
        public void Setup()
        {
            _controllerContext = new Mock<ControllerContext>();
            _logger = new Mock<ILogger<HomeController>>();
            _userService = new Mock<IUserService>();
            _movieService = new Mock<IMovieService>();
            _userRepository = new Mock<IUserRepository>();
            _homeController = new HomeController(_logger.Object, _userService.Object, _movieService.Object, _userRepository.Object);
            _homeController.ControllerContext = _controllerContext.Object;
        }

        [Test]
        public void LaunchStartPage_WhenUserNameIsNotEmpty_ShouldReturnViewWithUserViewModel()
        {
            var movie = new Movie("1+1");
            var expectedModel = new UserViewModel() { UserName = "Ihor" };
            var collectionModel = new UserViewModel() { MovieStatusDictionary = new Dictionary<Movie, bool>() { { movie, true } } };
            _movieService.Setup(m => m.GetCommonMovieCollection("customer", expectedModel.UserName)).Returns(collectionModel);

            var result = _homeController.LaunchStartPage(expectedModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<UserViewModel>(result?.Model);
        }

        [Test]
        public void LaunchStartPage_WhenUserNameIsEmptyAndUserIdentityIsNotCorrect_ShouldAddModelError()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, string.Empty),
                new Claim(ClaimTypes.Role, "1"),
            }, "mock"));
            _homeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            _userService.Setup(u => u.GetUserRole($"http://schemas.microsoft.com/ws/2008/06/identity/claims/role: 1")).Returns("customer");
            var expectedMessage = "User name is empty";
            var expectedErrorCount = 1;

            var result = _homeController.LaunchStartPage(new UserViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result?.ViewName);
            Assert.AreEqual(expectedErrorCount, _homeController.ModelState.ErrorCount);
        }

        [Test]
        public void LaunchStartPage_WhenUserNameIsEmptyAndUserIdentityIsCorrect_ShouldReturnViewWithUserViewModel()
        {
            string userName = "George";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, "1"),
            }, "mock"));
            _homeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            _userService.Setup(u => u.GetUserRole($"http://schemas.microsoft.com/ws/2008/06/identity/claims/role: 1")).Returns("customer");
            _movieService.Setup(m => m.GetCommonMovieCollection("customer", userName)).Returns(new UserViewModel()
            { MovieStatusDictionary = new Dictionary<Movie, bool>() { { new Movie("1+1"), false }, { new Movie("DALL-E"), true } } });
            _userRepository.Setup(u => u.GetAll()).Returns(new List<User>() { new User() { Name = "Alexander" } });

            var result = _homeController.LaunchStartPage(new UserViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<UserViewModel>(result?.Model);
        }
    }
}
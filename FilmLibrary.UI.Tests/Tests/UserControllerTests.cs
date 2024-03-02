using FilmLibrary.Business.Interfaces;
using FilmLibrary.Models.Models;
using FilmLibrary.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FilmLibrary.UI.Tests
{
    public class UserControllerTests
    {
        private Mock<IAuthentificationService> _authentificationService;
        private Mock<IUserService> _userService;
        private Mock<ILogger<UserController>> _logger;
        private UserController _userController;

        [SetUp]
        public void Setup()
        {
            _authentificationService = new Mock<IAuthentificationService>();
            _userService = new Mock<IUserService>();
            _logger = new Mock<ILogger<UserController>>();
            _userController = new UserController(_authentificationService.Object, _userService.Object, _logger.Object);
        }

        [Test]
        public void Login_WhenModelStateIsNotValid_ShouldAddModelError()
        {
            var credentialsModel = new CredentialsModel() { UserName = "Mikhail" };
            _userController.ModelState.AddModelError("Test", "Test error");
            var expectedErrorCount = 2;

            var result = _userController.Login(credentialsModel) as ViewResult;

            Assert.AreEqual(expectedErrorCount, _userController.ModelState.ErrorCount);
            Assert.IsAssignableFrom<CredentialsModel>(result?.Model);
        }

        [Test]
        public void Login_WhenModelStateIsValidAndUserModelIsIncorrect_ShouldAddModelError()
        {
            var credentialsModel = new CredentialsModel() { UserName = "Mikhail", Password = "Test password" };
            _authentificationService.Setup(a => a.Authenticate(credentialsModel)).Returns(new CreateUserModel());
            var expectedErrorCount = 1;

            var result = _userController.Login(credentialsModel) as ViewResult;

            Assert.AreEqual(expectedErrorCount, _userController.ModelState.ErrorCount);
            Assert.IsAssignableFrom<CredentialsModel>(result?.Model);
        }

        [Test]
        public void Login_WhenModelStateIsValidAndUserModelIsCorrect_ShouldReturnRedirectToActionResult()
        {
            var credentialsModel = new CredentialsModel() { UserName = "Mikhail", Password = "Test password" };
            var userModel = new CreateUserModel() { Name = "Mikhail", UserRole = "admin"};
            _authentificationService.Setup(a => a.Authenticate(credentialsModel)).Returns(userModel);
            var expectedErrorCount = 0;

            var result = _userController.Login(credentialsModel);

            Assert.AreEqual(expectedErrorCount, _userController.ModelState.ErrorCount);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void Register_WhenModelIsNotValid_ShouldAddModelError()
        {
            var credentialsModel = new CredentialsModel() { UserName = "Mikhail" };
            _userController.ModelState.AddModelError("Test", "Test error");
            var expectedMessage = "Model state is not valid";
            var expectedErrorCount = 2;

            var result = _userController.Register(credentialsModel) as ViewResult;

            Assert.AreEqual(expectedErrorCount, _userController.ModelState.ErrorCount);
            Assert.AreEqual(expectedMessage, result?.ViewName);
        }

        [Test]
        public void Register_WhenModelStateIsValidAndUserModelIsIncorrect_ShouldAddModelError()
        {
            var credentialsModel = new CredentialsModel() { UserName = "Mikhail", Password = "Test password" };
            var userModel = new CreateUserModel();
            _userService.Setup(a => a.CreateUser(credentialsModel)).Returns(userModel);
            var expectedMessage = "User exists";
            var expectedErrorCount = 1;

            var result = _userController.Register(credentialsModel) as ViewResult;

            Assert.AreEqual(expectedErrorCount, _userController.ModelState.ErrorCount);
            Assert.AreEqual(expectedMessage, result?.ViewName);
        }

        [Test]
        public void Register_WhenModelStateIsValidAndUserModelIsCorrect_ShouldReturnRedirectToActionResult()
        {
            var credentialsModel = new CredentialsModel() { UserName = "Mikhail", Password = "Test password" };
            var userModel = new CreateUserModel() { Name = "Mikhail", UserRole = "admin"};
            _userService.Setup(a => a.CreateUser(credentialsModel)).Returns(userModel);
            var expectedErrorCount = 0;

            var result = _userController.Register(credentialsModel);

            Assert.AreEqual(expectedErrorCount, _userController.ModelState.ErrorCount);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}
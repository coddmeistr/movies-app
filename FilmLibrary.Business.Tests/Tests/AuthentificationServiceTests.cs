using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using Moq;
using NUnit.Framework;
using System;

namespace FilmLibrary.Business.Tests.Tests
{
    public class AuthentificationServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private AuthentificationService _authentificationService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _authentificationService = new AuthentificationService(_userRepository.Object);
        }

        [Test]
        public void Authenticate_WhenUserNotExists_ShouldReturnEmptyCreateAuthenticateUserModel()
        {
            var credentialsModel = new CredentialsModel() { UserName = "George" };
            _userRepository.Setup(repo => repo.GetByName(credentialsModel.UserName)).Returns((User)null);

            var result = _authentificationService.Authenticate(credentialsModel);

            Assert.IsEmpty(result.Name);
        }

        [Test]
        public void Authenticate_WhenPasswordIsWrong_ShouldReturnModelWithEmptyName()
        {
            string correctPassword = "Testpass";
            var credentialsModel = new CredentialsModel() { Password = "Wrong test password" };
            string stringSalt = "bxAh+ZLHgXPrAUXkQ3+U+g==";
            var expectedSalt = Convert.FromBase64String(stringSalt);
            _userRepository.Setup(repo => repo.GetByName(credentialsModel.UserName)).Returns((new User()
            {
                PasswordHash = Convert.ToBase64String(_authentificationService.CalculateHash(correctPassword, ref expectedSalt)),
                PasswordSalt = stringSalt
            }));

            var result = _authentificationService.Authenticate(credentialsModel);

            Assert.IsEmpty(result.Name);
        }

        [Test]
        public void Authenticate_WhenPasswordIsCorrect_ShouldReturnModelWithCorrectUserName()
        {
            string expectedPassword = "Testpass";
            var credentialsModel = new CredentialsModel() { UserName = "Patrik", Password = expectedPassword };
            string stringSalt = "bxAh+ZLHgXPrAUXkQ3+U+g==";
            var expectedSalt = Convert.FromBase64String(stringSalt);
            _userRepository.Setup(repo => repo.GetByName(credentialsModel.UserName)).Returns((new User()
            {
                PasswordHash = Convert.ToBase64String(_authentificationService.CalculateHash(expectedPassword, ref expectedSalt)),
                PasswordSalt = stringSalt
            }));

            var result = _authentificationService.Authenticate(credentialsModel);

            Assert.AreEqual(credentialsModel.UserName, result.Name);
        }
    }
}
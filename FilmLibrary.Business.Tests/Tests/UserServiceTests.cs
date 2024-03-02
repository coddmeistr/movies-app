using FilmLibrary.Business.Interfaces;
using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.Core.Enums;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FilmLibrary.Business.Tests.Tests
{
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IAuthentificationService> _authentificationService;
        private Mock<ILogger<UserService>> _logger;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _authentificationService = new Mock<IAuthentificationService>();
            _logger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_userRepository.Object, _unitOfWork.Object, _authentificationService.Object, _logger.Object);
        }

        [Test]
        public void CreateUser_WhenUserExists_ShouldReturnEmptyUserModel()
        {
            var expectedUserName = "Mickle";
            var credentialsModel = new CredentialsModel() { UserName = expectedUserName, Password = "Testpass" };
            var user = new User() { Name = expectedUserName };
            _userRepository.Setup(u => u.GetByName(credentialsModel.UserName)).Returns(user);

            var result = _userService.CreateUser(credentialsModel);

            result.Name.Should().BeNullOrEmpty();
        }

        [Test]
        public void CreateUser_WhenUserNotExists_ShouldReturnUserModelWithNewUserData()
        {
            var expectedUserName = "Mickle";
            var expectedUserRole = "customer";
            var expectedStringHash = "nfs8DQWtXSMqZGdBFqBFhgFRdXJDuF46r6jdrcVnhZ4=";
            var credentialsModel = new CredentialsModel() { UserName = expectedUserName, Password = "Testpass" };
            _userRepository.Setup(u => u.GetByName(credentialsModel.UserName)).Returns((User)null);
            var salt = new byte[16] { 2, 4, 25, 48, 16, 99, 27, 37, 99, 10, 6, 11, 7, 8, 2, 6 };
            _authentificationService.Setup(a => a.GenerateSalt()).Returns(salt);
            _authentificationService.Setup(a => a.CalculateHash(credentialsModel.Password, ref salt)).Returns(Convert.FromBase64String(
                expectedStringHash));
            string stringSalt = Convert.ToBase64String(salt);
            var user = new User(expectedUserName, expectedStringHash, stringSalt, UserRole.customer);
            _userRepository.Setup(u => u.Create(user));

            var result = _userService.CreateUser(credentialsModel);

            result.Name.Should().Be(expectedUserName);
            result.UserRole.Should().Be(expectedUserRole);
        }

        [TestCase("")]
        [TestCase("Some text")]
        public void GetUserRole_WhenClaimIsIncorrect_ShouldThrowException(string claim)
        {
            _userService.Invoking(u => u.GetUserRole(claim)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestCase("There are many variations of passages of Lorem Ipsum available, but the majority have ", ExpectedResult = ", but the majority have ")]
        [TestCase("Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots", ExpectedResult = "ext. It has roots")]
        public string GetUserRole_WhenClaimIsCorrect_ShouldReturnString(string claim)
        {
            var result = _userService.GetUserRole(claim);
            return result;
        }

        [Test]
        public void GetUserNames_WhenMethodIsInvoked_ShouldReturnUserNamesCollection()
        {
            var expectedUserNamesCount = 2;
            var firstExpectedName = "Lionel";
            var secondExpectedName = "Ronaldo";
            var firstUser = new User() { Name = firstExpectedName };
            var secondUser = new User() { Name = secondExpectedName };
            _userRepository.Setup(u => u.GetAll()).Returns(new List<User>() { firstUser, secondUser });

            var result = _userService.GetUserNames();

            result.Should().Contain(firstExpectedName);
            result.Should().Contain(secondExpectedName);
            result.Should().HaveCount(expectedUserNamesCount);
        }
    }
}

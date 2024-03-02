using FilmLibrary.Business.Interfaces;
using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Repositories;
using FilmLibrary.SharedData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Transactions;

namespace FilmLibrary.DAL.Tests.Tests
{
    public class UserRepositoryTests
    {
        private Mock<ILogger<UserRepository>> _logger;
        private ApplicationDbContext _context;
        private UserRepository _userRepository;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder().UseSqlServer(UnitTestsConstants.TestsConnectionString).Options;
            _context = new ApplicationDbContext(options);
            _logger = new Mock<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_context, _logger.Object);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Test]
        public void Create_WhenUserEntityIsSet_ShouldCreateNewUser()
        {
            using (var scope = new TransactionScope())
            {
                var expectedUserName = "Mikhail";
                var user = new User() { Name = expectedUserName };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetByName(expectedUserName);

                result?.Name.Should().Be(expectedUserName);
            }
        }

        [Test]
        public void Delete_WhenUserNotExist_ShouldNotDeleteUser()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectUserName = "Stsiapan";
                var expectedUserName = "Mikhail";
                var preresult = _userRepository.GetByName(expectedUserName);
                preresult.Should().BeNull();
                var user = new User() { Name = expectedUserName };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                _userRepository.Delete(incorrectUserName);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetByName(expectedUserName);

                result?.Name.Should().Be(expectedUserName);
            }
        }

        [Test]
        public void Delete_WhenNameIsCorrect_ShouldDeleteUser()
        {
            using (var scope = new TransactionScope())
            {
                var expectedUserName = "Mikhail";
                var user = new User() { Name = expectedUserName };
                var preresult = _userRepository.GetByName(user.Name);
                preresult.Should().BeNull();

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                _userRepository.Delete(user.Name);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetByName(expectedUserName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenUserNameIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectUserName = "Olga";
                var user = new User() { Name = "Mikhail" };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetByName(incorrectUserName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenUserNameIsCorrect_ShouldReturnUser()
        {
            using (var scope = new TransactionScope())
            {
                var expectedUserName = "User";
                var user = new User() { Name = expectedUserName };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetByName(expectedUserName);

                result?.Name.Should().Be(expectedUserName);
            }
        }

        [Test]
        public void GetAll_WhenContextIsNotNull_ShouldReturnAllUsers()
        {
            using (var scope = new TransactionScope())
            {
                var firstUser = new User() { Name = "Misha" };
                var secondUser = new User() { Name = "Volodya" };
                var thirdUser = new User() { Name = "Boris" };
                var expectedUsersList = new List<User>() { firstUser, secondUser, thirdUser };

                _userRepository.Create(firstUser);
                _userRepository.Create(secondUser);
                _userRepository.Create(thirdUser);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetAll();

                result.Should().Contain(expectedUsersList);
            }
        }

        [Test]
        public void GetUserWithFavouriteMovies_WhenUserNameIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectUserName = string.Empty;
                var user = new User() { Name = "Mikhail", FavouriteMovies = new List<Movie>() { new Movie("Test movie") } };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetUserWithFavouriteMovies(incorrectUserName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetUserWithFavouriteMovies_WhenUserNameIsCorrect_ShouldReturnUserWithFavouriteMovies()
        {
            using (var scope = new TransactionScope())
            {
                var expectedUserName = "Mikhail";
                var expectedUsersCount = 1;
                var user = new User() { Name = expectedUserName, FavouriteMovies = new List<Movie>() { new Movie("Test movie") } };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetUserWithFavouriteMovies(user.Name);

                result?.Name.Should().Be(expectedUserName);
                result?.FavouriteMovies.Should().HaveCount(expectedUsersCount);
            }
        }

        [Test]
        public void GetById_WhenIdIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectId = 10000000;
                var user = new User() { Name = "Mikhail" };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();

                var result = _userRepository.GetById(incorrectId);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetById_WhenIdIsCorrect_ShouldReturnUser()
        {
            using (var scope = new TransactionScope())
            {
                var expectedUserName = "Mikhail";
                var user = new User() { Name = expectedUserName };

                _userRepository.Create(user);
                _unitOfWork.SaveChanges();
                var expectedId = user.Id;

                var result = _userRepository.GetById(expectedId);

                result?.Name.Should().Be(expectedUserName);
            }
        }
    }
}

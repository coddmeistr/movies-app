using FilmLibrary.Business.Interfaces;
using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Repositories;
using FilmLibrary.SharedData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Transactions;

namespace FilmLibrary.DAL.Tests.Tests
{
    public class GenreRepositoryTests
    {
        private ApplicationDbContext _context;
        private GenreRepository _genreRepository;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder().UseSqlServer(UnitTestsConstants.TestsConnectionString).Options;
            _context = new ApplicationDbContext(options);
            _genreRepository = new GenreRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Test]
        public void Create_WhenGenreEntityIsSet_ShouldCreateNewGenre()
        {
            using (var scope = new TransactionScope())
            {
                var expectedGenreName = "Test genre";
                var genre = new Genre() { Name = expectedGenreName };

                _genreRepository.Create(genre);
                _unitOfWork.SaveChanges();

                var result = _genreRepository.GetByName(expectedGenreName);

                result?.Name.Should().Be(expectedGenreName);
            }
        }

        [Test]
        public void Delete_WhenGenreNotExist_ShouldNotDeleteGenre()
        {
            using (var scope = new TransactionScope())
            {
                var expectedGenreName = "Test genre";
                var incorrectGenreName = "Incorrect genre";
                var preresult = _genreRepository.GetByName(expectedGenreName);
                preresult.Should().BeNull();
                var genre = new Genre() { Name = expectedGenreName };

                _genreRepository.Create(genre);
                _unitOfWork.SaveChanges();

                _genreRepository.Delete(incorrectGenreName);
                _unitOfWork.SaveChanges();

                var result = _genreRepository.GetByName(expectedGenreName);

                result?.Name.Should().Be(expectedGenreName);
            }
        }

        [Test]
        public void Delete_WhenNameIsCorrect_ShouldDeleteGenre()
        {
            using (var scope = new TransactionScope())
            {
                var expectedGenreName = "Test genre";
                var genre = new Genre() { Name = expectedGenreName };
                var preresult = _genreRepository.GetByName(genre.Name);
                preresult.Should().BeNull();

                _genreRepository.Create(genre);
                _unitOfWork.SaveChanges();

                _genreRepository.Delete(genre.Name);
                _unitOfWork.SaveChanges();

                var result = _genreRepository.GetByName(expectedGenreName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenGenreNameIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectGenreName = "Incorrect genre";
                var genre = new Genre() { Name = "Test genre" };

                _genreRepository.Create(genre);
                _unitOfWork.SaveChanges();

                var result = _genreRepository.GetByName(incorrectGenreName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenGenreNameIsCorrect_ShouldReturnGenre()
        {
            using (var scope = new TransactionScope())
            {
                var expectedGenreName = "Test genre";
                var genre = new Genre() { Name = expectedGenreName };

                _genreRepository.Create(genre);
                _unitOfWork.SaveChanges();

                var result = _genreRepository.GetByName(expectedGenreName);

                result?.Name.Should().Be(expectedGenreName);
            }
        }

        [Test]
        public void GetAll_WhenContextIsNotNull_ShouldReturnAllGenres()
        {
            using (var scope = new TransactionScope())
            {
                var firstGenre = new Genre() { Name = "Test genre 1" };
                var secondGenre = new Genre() { Name = "Test genre 2" };
                var thirdGenre = new Genre() { Name = "Test genre 3" };
                var expectedGenresList = new List<Genre>() {  firstGenre, secondGenre, thirdGenre };

                _genreRepository.Create(firstGenre);
                _genreRepository.Create(secondGenre);
                _genreRepository.Create(thirdGenre);
                _unitOfWork.SaveChanges();

                var result = _genreRepository.GetAll();

                result.Should().Contain(expectedGenresList);
            }
        }

        [Test]
        public void GetById_WhenIdIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectId = 10000000;
                var genre = new Genre() { Name = "Test genre" };

                _genreRepository.Create(genre);
                _unitOfWork.SaveChanges();

                var result = _genreRepository.GetById(incorrectId);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetById_WhenIdIsCorrect_ShouldReturnGenre()
        {
            using (var scope = new TransactionScope())
            {
                var expectedGenreName = "Test genre";
                var genre = new Genre() { Name = expectedGenreName };

                _genreRepository.Create(genre);
                _unitOfWork.SaveChanges();
                var expectedId = genre.Id;

                var result = _genreRepository.GetById(expectedId);

                result?.Name.Should().Be(expectedGenreName);
            }
        }
    }
}

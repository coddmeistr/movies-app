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
    public class MovieRepositoryTests
    {
        private Mock<ILogger<MovieRepository>> _logger;
        private ApplicationDbContext _context;
        private MovieRepository _movieRepository;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder().UseSqlServer(UnitTestsConstants.TestsConnectionString).Options;
            _context = new ApplicationDbContext(options);
            _logger = new Mock<ILogger<MovieRepository>>();
            _movieRepository = new MovieRepository(_context, _logger.Object);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Test]
        public void Create_WhenMovieEntityIsSet_ShouldCreateNewMovie()
        {
            using (var scope = new TransactionScope())
            {
                var expectedMovieName = "Test movie";
                var movie = new Movie(expectedMovieName);

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetByName(expectedMovieName);

                result?.Name.Should().Be(expectedMovieName);
            }
        }

        [Test]
        public void Delete_WhenMovieNotExist_ShouldNotDeleteMovie()
        {
            using (var scope = new TransactionScope())
            {
                var expectedMovieName = "Test movie";
                var incorrectMovieName = "Incorrect movie name";
                var preresult = _movieRepository.GetByName(expectedMovieName);
                preresult.Should().BeNull();
                var movie = new Movie(expectedMovieName);

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                _movieRepository.Delete(incorrectMovieName);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetByName(expectedMovieName);

                result?.Name.Should().Be(expectedMovieName);
            }
        }

        [Test]
        public void Delete_WhenNameIsCorrect_ShouldDeleteMovie()
        {
            using (var scope = new TransactionScope())
            {
                var expectedMovieName = "Test movie";
                var movie = new Movie(expectedMovieName);
                var preresult = _movieRepository.GetByName(movie.Name);
                preresult.Should().BeNull();

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                _movieRepository.Delete(movie.Name);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetByName(expectedMovieName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenMovieNameIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectMovieName = "Incorrect movie name";
                var movie = new Movie("Test movie");

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetByName(incorrectMovieName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenMovieNameIsCorrect_ShouldReturnMovie()
        {
            using (var scope = new TransactionScope())
            {
                var expectedMovieName = "Test movie";
                var movie = new Movie(expectedMovieName);

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetByName(expectedMovieName);

                result?.Name.Should().Be(expectedMovieName);
            }
        }

        [Test]
        public void GetAll_WhenContextIsNotNull_ShouldReturnAllMovies()
        {
            using (var scope = new TransactionScope())
            {
                var firstMovie = new Movie("Test movie 1");
                var secondMovie = new Movie("Test movie 2");
                var thirdMovie = new Movie("Test movie 3");
                var expectedMoviesList = new List<Movie>() { firstMovie, secondMovie, thirdMovie };

                _movieRepository.Create(firstMovie);
                _movieRepository.Create(secondMovie);
                _movieRepository.Create(thirdMovie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetAll();

                result.Should().Contain(expectedMoviesList);
            }
        }

        [Test]
        public void GetMovieWithArtists_WhenMovieNameIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectMovieName = "Incorrect movie name";
                var movie = new Movie("Test movie", new List<Artist>() { new Artist() { Name = "Oleg" } });

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetMovieWithArtists(incorrectMovieName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetMovieWithArtists_WhenMovieNameIsCorrect_ShouldReturnMovieWithArtists()
        {
            using (var scope = new TransactionScope())
            {
                var expectedMovieName = "Test movie";
                var expectedArtistsCount = 1;
                var movie = new Movie(expectedMovieName, new List<Artist>() { new Artist() { Name = "Oleg" } });

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetMovieWithArtists(movie.Name);

                result?.Name.Should().Be(expectedMovieName);
                result?.Artists.Should().HaveCount(expectedArtistsCount);
            }
        }

        [Test]
        public void GetMovieWithGenres_WhenMovieNameIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectMovieName = "Incorrect movie name";
                var movie = new Movie("Test movie", new List<Genre>() { new Genre() { Name = "Test genre" } });

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetMovieWithGenres(incorrectMovieName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetMovieWithGenres_WhenMovieNameIsCorrect_ShouldReturnMovieWithGenres()
        {
            using (var scope = new TransactionScope())
            {
                var expectedMovieName = "Test movie";
                var expectedGenresCount = 1;
                var movie = new Movie(expectedMovieName, new List<Genre>() { new Genre() { Name = "Test genre" } });

                _movieRepository.Create(movie);
                _unitOfWork.SaveChanges();

                var result = _movieRepository.GetMovieWithGenres(movie.Name);

                result?.Name.Should().Be(expectedMovieName);
                result?.Genres.Should().HaveCount(expectedGenresCount);
            }
        }
    }
}

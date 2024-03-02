using FilmLibrary.Business.Interfaces;
using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace FilmLibrary.Business.Tests.Tests
{
    public class MovieServiceTests
    {
        private Mock<ILogger<MovieService>> _logger;
        private Mock<IMovieRepository> _movieRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IArtistRepository> _artistRepository;
        private Mock<IArtistService> _artistService;
        private Mock<IGenreService> _genreService;
        private Mock<IGenreRepository> _genreRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private MovieService _movieService;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<MovieService>>();
            _movieRepository = new Mock<IMovieRepository>();
            _userRepository = new Mock<IUserRepository>();
            _artistRepository = new Mock<IArtistRepository>();
            _artistService = new Mock<IArtistService>();
            _genreService = new Mock<IGenreService>();
            _genreRepository = new Mock<IGenreRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _movieService = new MovieService(_movieRepository.Object, _unitOfWork.Object, _userRepository.Object, _genreRepository.Object, _logger.Object, _artistService.Object,
                _genreService.Object, _artistRepository.Object);
        }

        [TestCase("George", "1+1", ExpectedResult = 0)]
        [TestCase("Maxim", "Big Bang Theory", ExpectedResult = 0)]
        public int AddOrRemoveMovieIfExistsFromFavouriteCollection_WhenDataIsCorrectAndFavouriteMovieExists_ShouldRemoveMovie(string uName, string mName)
        {
            string userName = uName;
            string movieName = mName;
            var favouriteMovie = new Movie(movieName);
            var user = new User() { FavouriteMovies = new List<Movie>() { favouriteMovie } };
            _movieRepository.Setup(m => m.GetByName(movieName)).Returns(favouriteMovie);
            _userRepository.Setup(u => u.GetUserWithFavouriteMovies(userName)).Returns(user);

            _movieService.AddOrRemoveMovieIfExistsFromFavouriteCollection(movieName, userName);

            return user.FavouriteMovies.Count;
        }

        [TestCase("George", "1+1", ExpectedResult = 1)]
        [TestCase("Maxim", "Big Bang Theory", ExpectedResult = 1)]
        public int AddOrRemoveMovieIfExistsFromFavouriteCollection_WhenDataIsCorrectAndFavouriteMovieNotExists_ShouldCreateMovie(string userN, string movieN)
        {
            string userName = userN;
            string movieName = movieN;
            var favouriteMovie = new Movie(movieName);
            var user = new User() { FavouriteMovies = new List<Movie>() };
            _movieRepository.Setup(m => m.GetByName(movieName)).Returns(favouriteMovie);
            _userRepository.Setup(u => u.GetUserWithFavouriteMovies(userName)).Returns(user);

            _movieService.AddOrRemoveMovieIfExistsFromFavouriteCollection(movieName, userName);

            return user.FavouriteMovies.Count;
        }

        [TestCase("Movie", "James")]
        [TestCase("Interstellar", "George")]
        public void CreateMovie_WhenMovieModelIsSet_ShouldReturnCorrectMovie(string name, string firstActor)
        {
            var movieModel = new CreateMovieModel() { Name = name, FirstArtist = firstActor };
            var expectedArtistsCount = 1;
            var expectedModelName = movieModel.Name;

            var result = _movieService.CreateMovie(movieModel);

            result.Name.Should().Be(expectedModelName);
            result.Artists.Should().HaveCount(expectedArtistsCount);
        }

        [Test]
        public void DeleteMovieFromCommonCollection_WhenMovieExists_ShouldReturnSuccess()
        {
            var expectedMovieName = "Hunger games";
            _movieRepository.Setup(m => m.Delete(expectedMovieName)).Returns(true);

            var result = _movieService.DeleteMovieFromCommonCollection(expectedMovieName);

            result.Success.Should().BeTrue();
        }

        [Test]
        public void DeleteMovieFromCommonCollection_WhenMovieNotExists_ShouldReturnFailureAndError()
        {
            var expectedMovieName = "Hunger games";
            _movieRepository.Setup(m => m.Delete(expectedMovieName)).Returns(false);

            var result = _movieService.DeleteMovieFromCommonCollection(expectedMovieName);

            result.Error.Should().Be("Wrong movie name");
            result.Success.Should().BeFalse();
        }

        [Test]
        public void EditMovieInfo_WhenMovieModelIsSet_ShouldUpdateMovieInfo()
        {
            int testArtistId = 3, testGenreId = 5;
            string expectedDescription = "New description";
            int expectedArticul = 1000, expectedArtistsAndGenresCount = 1;
            var movieModel = new UpdateMovieModel() { Name = "Test name", Description = expectedDescription,
                Articul = expectedArticul, ArtistsId = new List<int>() { testArtistId }, GenresId = new List<int>() { testGenreId } };
            var movie = new Movie("Test name", "Test description", 100);
            _movieRepository.Setup(m => m.GetMovieWithArtists(movieModel.Name)).Returns(movie);
            _movieRepository.Setup(m => m.GetMovieWithGenres(movieModel.Name)).Returns(movie);
            var artist = new Artist() { Name = "Aleksey" };
            var genre = new Genre() { Name = "Test genre" };
            _artistRepository.Setup(a => a.GetById(testArtistId)).Returns(artist);
            _artistRepository.Setup(a => a.GetAll()).Returns(new List<Artist>());
            _genreRepository.Setup(a => a.GetById(testGenreId)).Returns(genre);
            _genreRepository.Setup(a => a.GetAll()).Returns(new List<Genre>());

            _movieService.EditMovieInfo(movieModel);

            movie.Description.Should().Be(expectedDescription);
            movie.Articul.Should().Be(expectedArticul);
            movie.Artists.Should().HaveCount(expectedArtistsAndGenresCount);
            movie.Genres.Should().HaveCount(expectedArtistsAndGenresCount);
        }

        [Test]
        public void GetMovieByName_WhenMovieNotExist_ShouldReturnEmptyMovieViewModel()
        {
            string movieName = "Pulp Fiction";
            _movieRepository.Setup(m => m.GetMovieWithArtists(movieName)).Returns((Movie)null);
            _movieRepository.Setup(m => m.GetMovieWithGenres(movieName)).Returns((Movie)null);
            var expectedArticul = 0;

            var result = _movieService.GetMovieByName(movieName);

            result.Articul.Should().Be(expectedArticul);
            result.Name.Should().BeEmpty();
            result.Description.Should().BeEmpty();
        }

        [Test]
        public void GetMovieByName_WhenMovieExist_ShouldReturnCorrectMovieViewModel()
        {
            string expectedMovieName = "Pulp Fiction";
            string expectedDescription = "Test description";
            var expectedArticul = 10;
            var movie = new Movie(expectedDescription, expectedArticul);
            _movieRepository.Setup(m => m.GetMovieWithArtists(expectedMovieName)).Returns(movie);
            _movieRepository.Setup(m => m.GetMovieWithGenres(expectedMovieName)).Returns(movie);

            var result = _movieService.GetMovieByName(expectedMovieName);

            result.Name.Should().Be(expectedMovieName);
            result.Description.Should().Be(expectedDescription);
            result.Articul.Should().Be(expectedArticul);
        }

        [Test]
        public void GetUserFavouriteMoviesByName_WhenUserNotExist_ShouldReturnEmptyCollectionViewModel()
        {
            string expectedUserName = "User";
            var expectedMoviesCount = 0;
            _userRepository.Setup(u => u.GetUserWithFavouriteMovies(expectedUserName)).Returns((User)null);

            var result = _movieService.GetUserFavouriteMoviesByName(expectedUserName);

            result.Movies.Count.Should().Be(expectedMoviesCount);
        }

        [Test]
        public void GetUserFavouriteMoviesByName_WhenUserExist_ShouldReturnCorrectCollectionViewModel()
        {
            var expectedMoviesCount = 1;
            string userName = "Alexander";
            var movie = new Movie("VALL-E");
            var user = new User() { FavouriteMovies = new List<Movie>() { movie } };
            _userRepository.Setup(u => u.GetUserWithFavouriteMovies(userName)).Returns(user);

            var result = _movieService.GetUserFavouriteMoviesByName(userName);

            result.Movies.Should().Contain(movie);
            result.Movies.Count.Should().Be(expectedMoviesCount);
        }

        [Test]
        public void GetUserFavouriteMoviesById_WhenUserNotExist_ShouldReturnEmptyCollectionViewModel()
        {
            string expectedIdString = "5";
            _userRepository.Setup(u => u.GetById(It.IsAny<int>())).Returns((User)null);

            var result = _movieService.GetUserFavouriteMoviesById(expectedIdString);

            result.Movies.Should().BeEmpty();
        }

        [Test]
        public void GetUserFavouriteMoviesById_WhenUserExist_ShouldReturnCorrectCollectionViewModel()
        {
            var expectedMoviesCount = 1;
            string expectedIdString = "5";
            var movie = new Movie("VALL-E");
            var user = new User() { Name = "Aleksei", FavouriteMovies = new List<Movie>() { movie } };
            _userRepository.Setup(u => u.GetById(It.IsAny<int>())).Returns(user);
            _userRepository.Setup(u => u.GetUserWithFavouriteMovies(user.Name)).Returns(user);

            var result = _movieService.GetUserFavouriteMoviesById(expectedIdString);

            result.Movies.Should().Contain(movie);
            result.Movies.Count.Should().Be(expectedMoviesCount);
        }

        [Test]
        public void GetCommonMovieCollection_WhenUserRoleAndNameAreSet_ShouldReturnUserViewModel()
        {
            var expectedMoviesCount = 1;
            string userName = "Mickle";
            string role = "customer";
            var movie = new Movie("The Dark Night");
            var user = new User() { FavouriteMovies = new List<Movie>() { movie } };
            _movieRepository.Setup(m => m.GetAll()).Returns(new List<Movie>() { movie });
            _userRepository.Setup(u => u.GetUserWithFavouriteMovies(userName)).Returns(user);

            var result = _movieService.GetCommonMovieCollection(role, userName);

            result.MovieStatusDictionary.Count.Should().Be(expectedMoviesCount);
            result.MovieStatusDictionary[movie].Should().BeTrue();
        }
    }
}
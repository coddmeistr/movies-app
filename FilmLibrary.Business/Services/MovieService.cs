using FilmLibrary.Business.Interfaces;
using FilmLibrary.Core.Constants;
using FilmLibrary.Core.Entities;
using FilmLibrary.Core.Enums;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using Microsoft.Extensions.Logging;

namespace FilmLibrary.Business.Services
{
    public class MovieService : IMovieService
    {
        private readonly ILogger<MovieService> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IArtistService _artistService;
        private readonly IGenreService _genreService;
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MovieService(IMovieRepository movieRepository, IUnitOfWork unitOfWork, IUserRepository userRepository,
            IGenreRepository genreRepository, ILogger<MovieService> logger, IArtistService artistService, IGenreService genreService,
            IArtistRepository artistRepository)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
            _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
            _genreRepository = genreRepository ?? throw new ArgumentNullException(nameof(genreRepository));
            _artistRepository = artistRepository ?? throw new ArgumentNullException(nameof(artistRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ResultModel AddOrRemoveMovieIfExistsFromFavouriteCollection(string movieName, string userName)
        {
            var favouriteMovie = _movieRepository.GetByName(movieName);
            var user = _userRepository.GetUserWithFavouriteMovies(userName);
            var result = new ResultModel();

            if (favouriteMovie is null)
            {
                _logger.LogWarning("Movie wasn't found");
                result.Success = false;
                result.Error = "Movie wasn't found";
                return result;
            }

            if (user is null)
            {
                _logger.LogError(LogMessages.UserNotFoundMessage);
                result.Success = false;
                result.Error = "User wasn't found";
                return result;
            }

            if (user.FavouriteMovies.FirstOrDefault(m => m.Name.Equals(favouriteMovie.Name)) != null)
            {
                user.FavouriteMovies.Remove(favouriteMovie);
                _logger.LogInformation("Removed favourite movies from user's favourite collection"); 
            }
            else
            {
                user.FavouriteMovies.Add(favouriteMovie);
                _logger.LogInformation("Added movie to user's favourite collection");
            }

            _unitOfWork.SaveChanges();
            _logger.LogInformation(LogMessages.SavedChangesToDbMessage);

            result.Success = true;
            return result;
        }

        public Movie CreateMovie(CreateMovieModel model)
        {
            var newMovie = new Movie(model.Name, model.Description, model.Articul);

            if (!string.IsNullOrEmpty(model.FirstArtist))
            {
                newMovie.Artists.Add(new Artist() { Name = model.FirstArtist });
            }

            if (!string.IsNullOrEmpty(model.SecondArtist))
            {
                newMovie.Artists.Add(new Artist() { Name = model.SecondArtist });
            }

            foreach (var genreId in model.GenresId)
            {
                var genre = _genreRepository.GetById(genreId);
                if (genre != null)
                {
                    newMovie.Genres.Add(genre);
                }
            }

            _movieRepository.Create(newMovie);
            _logger.LogInformation("Created new movie");

            _unitOfWork.SaveChanges();
            _logger.LogInformation(LogMessages.SavedChangesToDbMessage);

            return newMovie;
        }

        public ResultModel DeleteMovieFromCommonCollection(string movieName)
        {
            var success = _movieRepository.Delete(movieName);
            var result = new ResultModel() { Success = success };
            if (!success)
            {
                result.Error = "Wrong movie name";
                _logger.LogError("Wrong movie name when deleting");
            }
            else
            {
                _logger.LogInformation("Deleted movie from common collection");
            }

            _unitOfWork.SaveChanges();
            _logger.LogInformation(LogMessages.SavedChangesToDbMessage);
            return result;
        }

        public ResultModel EditMovieInfo(UpdateMovieModel model)
        {
            var result = new ResultModel();
            var movieWithArtists = _movieRepository.GetMovieWithArtists(model.Name);
            if (movieWithArtists is null)
            {
                result.Success = false;
                result.Error = "Movie not found";
                _logger.LogError("Movie not found");
                return result;
            }

            movieWithArtists.UpdateMainInfo(model.Description, model.Articul);
            _logger.LogInformation("Edited movie's description and articul");

            var newArtists = new List<Artist>();
            foreach (var artistId in model.ArtistsId)
            {
                var artist = _artistRepository.GetById(artistId);
                if (artist != null)
                {
                    newArtists.Add(artist);
                }
            }

            if (newArtists.Count != 0)
            {
                movieWithArtists.UpdateArtists(_artistService.GetAll(), newArtists);
                _logger.LogInformation("Changed movie's artists");
            }

            _unitOfWork.SaveChanges();
            _logger.LogInformation(LogMessages.SavedChangesToDbMessage);

            var movieWithGenres = _movieRepository.GetMovieWithGenres(model.Name);
            if (movieWithGenres is null)
            {
                result.Success = false;
                result.Error = "Movie not found";
                _logger.LogError("Movie not found");
                return result;
            }

            var newGenres = new List<Genre>();
            foreach (var genreId in model.GenresId)
            {
                var genre = _genreRepository.GetById(genreId);
                if (genre != null)
                {
                    newGenres.Add(genre);
                }
            }

            if (newGenres.Count != 0)
            {
                movieWithGenres.UpdateGenres(_genreService.GetAll(), newGenres);
                _logger.LogInformation("Changed movie's genres");
            }

            _unitOfWork.SaveChanges();
            _logger.LogInformation(LogMessages.SavedChangesToDbMessage);
            result.Success = true;
            return result;
        }

        public MovieViewModel GetMovieByName(string movieName)
        {
            if (string.IsNullOrEmpty(movieName))
            {
                return new MovieViewModel();
            }

            var movieWithArtists = _movieRepository.GetMovieWithArtists(movieName);
            var movieWithGenres = _movieRepository.GetMovieWithGenres(movieName);

            if (movieWithArtists is null || movieWithGenres is null)
            {
                return new MovieViewModel();
            }

            return new MovieViewModel(movieName, movieWithArtists.Description, movieWithArtists.Articul,
                movieWithArtists.Artists, movieWithGenres.Genres);
        }

        public FavouriteMoviesCollectionViewModel GetUserFavouriteMoviesByName(string userName)
        {
            return GetFavouriteMovies(ref userName);
        }

        public FavouriteMoviesCollectionViewModel GetUserFavouriteMoviesById(string userIdString)
        {
            int userId = int.Parse(userIdString);
            var userById = _userRepository.GetById(userId);

            if (userById is null)
            {
                _logger.LogError(LogMessages.UserNotFoundMessage);
                return new FavouriteMoviesCollectionViewModel();
            }

            string userName = userById.Name;
            return GetFavouriteMovies(ref userName);
        }

        private FavouriteMoviesCollectionViewModel GetFavouriteMovies(ref string userName)
        {
            var user = _userRepository.GetUserWithFavouriteMovies(userName);
            var favouriteMovies = new FavouriteMoviesCollectionViewModel();

            if (user is null)
            {
                _logger.LogError(LogMessages.UserNotFoundMessage);
                return new FavouriteMoviesCollectionViewModel();
            }

            favouriteMovies.Movies = user.FavouriteMovies.ToList();
            _logger.LogInformation("Got user's favourite movies");

            return favouriteMovies;
        }

        public UserViewModel GetCommonMovieCollection(string role, string userName)
        {
            var userViewModel = new UserViewModel
            {
                Role = (UserRole)Enum.Parse(typeof(UserRole), role)
            };

            var movies = _movieRepository.GetAll().ToList();
            var favouriteMoviesModel = GetUserFavouriteMoviesByName(userName);

            foreach (var item in movies)
            {
                if (favouriteMoviesModel.Movies.Contains(item))
                {
                    userViewModel.MovieStatusDictionary.Add(item, true);
                }
                else
                {
                    userViewModel.MovieStatusDictionary.Add(item, false);
                }
            }

            return userViewModel;
        }
    }
}
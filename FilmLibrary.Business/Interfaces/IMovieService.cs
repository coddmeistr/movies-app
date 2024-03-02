using FilmLibrary.Core.Entities;
using FilmLibrary.Models.Models;

namespace FilmLibrary.Business.Interfaces
{
    public interface IMovieService
    {
        public Movie CreateMovie(CreateMovieModel model);

        public ResultModel AddOrRemoveMovieIfExistsFromFavouriteCollection(string movieName, string userName);

        public ResultModel DeleteMovieFromCommonCollection(string movieName);

        public FavouriteMoviesCollectionViewModel GetUserFavouriteMoviesByName(string userName);

        public FavouriteMoviesCollectionViewModel GetUserFavouriteMoviesById(string userIdString);

        public UserViewModel GetCommonMovieCollection(string role, string userName);

        public ResultModel EditMovieInfo(UpdateMovieModel model);

        public MovieViewModel GetMovieByName(string movieName);
    }
}

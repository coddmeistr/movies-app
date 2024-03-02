using FilmLibrary.Core.Entities;

namespace FilmLibrary.DAL.Interfaces
{
    public interface IMovieRepository
    {
        public Movie? GetByName(string name);

        public void Create(Movie movie);

        public bool Delete(string name);

        public IEnumerable<Movie> GetAll();

        public Movie? GetMovieWithArtists(string movieName);

        public Movie? GetMovieWithGenres(string movieName);
    }
}

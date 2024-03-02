using FilmLibrary.Core.Constants;
using FilmLibrary.Core.Entities;
using FilmLibrary.Core.Exceptions;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmLibrary.DAL.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ILogger<MovieRepository> _logger;
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context, ILogger<MovieRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Create(Movie movie)
        {
            _context.Add(movie);
        }

        public bool Delete(string name)
        {
            if (_context.Movies is null)
                return false;

            var movie = _context.Movies.FirstOrDefault(n => n.Name.Equals(name));

            if (movie != null)
            {
                _context.Remove(movie);
                return true;
            }

            return false;
        }

        public Movie? GetByName(string name)
        {
            if (_context.Movies is null) return null;

            return _context.Movies.FirstOrDefault(n => n.Name.Equals(name));
        }

        public IEnumerable<Movie> GetAll()
        {
            if (_context.Movies is null) return new List<Movie>();

            return _context.Movies;
        }

        public Movie? GetMovieWithArtists(string movieName)
        {
            if (_context.Movies is null)
            {
                _logger.LogError(LogMessages.NoMoviesInDbMessage);
                throw new MovieDbSetNullException(nameof(_context.Movies));
            }

            return _context.Movies.Include(u => u.Artists).FirstOrDefault(m => m.Name.Equals(movieName));
        }

        public Movie? GetMovieWithGenres(string movieName)
        {
            if (_context.Movies is null)
            {
                _logger.LogError(LogMessages.NoMoviesInDbMessage);
                throw new MovieDbSetNullException(nameof(_context.Movies));
            }

            return _context.Movies.Include(u => u.Genres).FirstOrDefault(m => m.Name.Equals(movieName));
        }
    }
}

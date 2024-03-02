using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Interfaces;

namespace FilmLibrary.DAL.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Create(Genre genre)
        {
            _context.Add(genre);
        }

        public void Delete(string name)
        {
            if (_context.Genres is null) return;

            var genre = _context.Genres.FirstOrDefault(n => n.Name.Equals(name));

            if (genre != null) _context.Remove(genre);
        }

        public Genre? GetByName(string name)
        {
            if (_context.Genres is null) return null;

            return _context.Genres.FirstOrDefault(n => n.Name.Equals(name));
        }

        public IEnumerable<Genre> GetAll()
        {
            if (_context.Genres is null) return new List<Genre>();

            return _context.Genres;
        }

        public Genre? GetById(int id)
        {
            if (_context.Genres is null) return null;

            return _context.Genres.FirstOrDefault(n => n.Id == id);
        }
    }
}

using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Interfaces;

namespace FilmLibrary.DAL.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly ApplicationDbContext _context;

        public ArtistRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int Create(Artist artist)
        {
            _context.Add(artist);
            return artist.Id;
        }

        public void Delete(string name)
        {
            if (_context.Artists is null) return;

            var artist = _context.Artists.FirstOrDefault(n => n.Name.Equals(name));

            if (artist != null) _context.Remove(artist);
        }

        public Artist? GetByName(string name)
        {
            if (_context.Artists is null) return null;

            return _context.Artists.FirstOrDefault(n => n.Name.Equals(name));
        }

        public IEnumerable<Artist> GetAll()
        {
            if (_context.Artists is null) return new List<Artist>();

            return _context.Artists;
        }

        public Artist? GetById(int id)
        {
            if (_context.Artists is null) return null;

            return _context.Artists.FirstOrDefault(n => n.Id == id);
        }
    }
}

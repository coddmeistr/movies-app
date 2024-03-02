using FilmLibrary.Core.Entities;

namespace FilmLibrary.DAL.Interfaces
{
    public interface IArtistRepository
    {
        public Artist? GetByName(string name);

        public int Create(Artist artist);

        public void Delete(string name);

        public IEnumerable<Artist> GetAll();

        public Artist? GetById(int id);
    }
}

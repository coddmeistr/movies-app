using FilmLibrary.Core.Entities;

namespace FilmLibrary.Business.Interfaces
{
    public interface IArtistService
    {
        public IEnumerable<Artist> GetAll();
    }
}

using FilmLibrary.Core.Entities;

namespace FilmLibrary.Business.Interfaces
{
    public interface IGenreService
    {
        public IEnumerable<Genre> GetAll();
    }
}

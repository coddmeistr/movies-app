using FilmLibrary.Core.Entities;

namespace FilmLibrary.DAL.Interfaces
{
    public interface IGenreRepository
    {
        public Genre? GetByName(string name);

        public Genre? GetById(int id);

        public void Create(Genre genre);

        public void Delete(string name);

        public IEnumerable<Genre> GetAll();
    }
}
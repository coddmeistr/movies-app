using FilmLibrary.Business.Interfaces;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Interfaces;

namespace FilmLibrary.Business.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository ?? throw new ArgumentNullException(nameof(genreRepository));
        }

        public IEnumerable<Genre> GetAll()
        {
            return _genreRepository.GetAll();
        }
    }
}

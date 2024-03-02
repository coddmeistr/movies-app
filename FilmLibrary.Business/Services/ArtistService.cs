using FilmLibrary.Business.Interfaces;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Interfaces;

namespace FilmLibrary.Business.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository ?? throw new ArgumentNullException(nameof(artistRepository));
        }

        public IEnumerable<Artist> GetAll()
        {
            return _artistRepository.GetAll();
        }
    }
}
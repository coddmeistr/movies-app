using FilmLibrary.Core.Entities;

namespace FilmLibrary.DAL.Interfaces
{
    public interface IUserRepository
    {
        public User? GetByName(string name);

        public void Create(User user);

        public void Delete(string name);

        public IEnumerable<User> GetAll();

        public User? GetUserWithFavouriteMovies(string userName);

        public User? GetById(int id);
    }
}

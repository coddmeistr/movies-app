using FilmLibrary.Core.Entities;
using FilmLibrary.Core.Exceptions;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmLibrary.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Create(User user)
        {
            _context.Add(user);
        }

        public void Delete(string name)
        {
            if (_context.Users is null) return;

            var user = _context.Users.FirstOrDefault(n => n.Name.Equals(name));

            if (user != null) _context.Remove(user);
        }

        public User? GetByName(string name)
        {
            if (_context.Users is null) return null;

            return _context.Users.FirstOrDefault(n => n.Name.Equals(name));
        }

        public IEnumerable<User> GetAll()
        {
            if (_context.Users is null) return new List<User>();

            return _context.Users;
        }

        public User? GetUserWithFavouriteMovies(string userName)
        {
            if (_context.Users is null)
            {
                _logger.LogError("No users dbset was found in context");
                throw new UserDbSetNullException(nameof(_context.Users));
            }

            return _context.Users.Include(u => u.FavouriteMovies).FirstOrDefault(u => u.Name.Equals(userName));
        }

        public User? GetById(int id)
        {
            if (_context.Users is null) return null;

            return _context.Users.FirstOrDefault(n => n.Id == id);
        }
    }
}

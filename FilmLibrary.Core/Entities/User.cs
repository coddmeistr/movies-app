using FilmLibrary.Core.Enums;

namespace FilmLibrary.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public virtual ICollection<Movie> FavouriteMovies { get; set; } = new List<Movie>();

        public User() { }

        public User(string name, string hash, string salt, UserRole role)
        {
            Name = name;
            PasswordHash = hash;
            PasswordSalt = salt;
            UserRole = role;
        }
    }
}
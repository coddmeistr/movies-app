using FilmLibrary.Core.Entities;
using FilmLibrary.Core.Enums;

namespace FilmLibrary.Models.Models
{
    public class UserViewModel
    {
        public UserRole Role { get; set; }
        public Dictionary<Movie, bool> MovieStatusDictionary = new Dictionary<Movie, bool>();
        public string UserName { get; set; } = string.Empty;
    }
}
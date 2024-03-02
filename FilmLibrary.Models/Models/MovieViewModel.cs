using FilmLibrary.Core.Entities;

namespace FilmLibrary.Models.Models
{
    public class MovieViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Articul { get; set; }
        public ICollection<Artist> Artists { get; set; } = new List<Artist>();
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public MovieViewModel() { }

        public MovieViewModel(string name, string description, int articul, ICollection<Artist> artists, ICollection<Genre> genres)
        {
            Name = name;
            Description = description;
            Articul = articul;
            Artists = artists;
            Genres = genres;
        }
    }
}
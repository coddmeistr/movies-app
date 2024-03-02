using System.ComponentModel.DataAnnotations;

namespace FilmLibrary.Models.Models
{
    public class CreateMovieModel
    {
        [Required(ErrorMessage = "Movie name is not specified")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Movie description is not specified")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Movie articul is not specified")]
        public int Articul { get; set; }
        public string FirstArtist { get; set; } = string.Empty;
        public string SecondArtist { get; set; } = string.Empty;
        public ICollection<int> GenresId { get; set; } = new List<int>();
    }
}
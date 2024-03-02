using System.ComponentModel.DataAnnotations;

namespace FilmLibrary.Models.Models
{
    public class UpdateMovieModel
    {

        [Required(ErrorMessage = "Movie name is not specified")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Movie description is not specified")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Movie articul is not specified")]
        public int Articul { get; set; }
        public ICollection<int> GenresId { get; set; } = new List<int>();
        public ICollection<int> ArtistsId { get; set; } = new List<int>();
    }
}

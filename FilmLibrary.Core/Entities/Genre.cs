namespace FilmLibrary.Core.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}

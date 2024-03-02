namespace FilmLibrary.Core.Entities
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
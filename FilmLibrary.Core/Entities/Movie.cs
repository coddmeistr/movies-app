namespace FilmLibrary.Core.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Articul { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();
        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public Movie(string name)
        {
            Name = name;
        }

        public Movie(string description, int articul)
        {
            Description = description;
            Articul = articul;
        }

        public Movie(string name, ICollection<Artist> artists)
        {
            Name = name;
            Artists = artists;
        }

        public Movie(string name, ICollection<Genre> genres)
        {
            Name = name;
            Genres = genres;
        }

        public Movie(string name, string description, int articul)
        {
            Name = name;
            Description = description;
            Articul = articul;
        }

        public void UpdateMainInfo(string description, int articul)
        {
            Description = description;
            Articul = articul;
        }

        public void UpdateArtists(IEnumerable<Artist> oldArtists, IEnumerable<Artist> newArtists)
        {
            foreach (Artist artist in oldArtists)
            {
                Artists.Remove(artist);
            }

            foreach (Artist artist in newArtists)
            {
                Artists.Add(artist);
            }
        }

        public void UpdateGenres(IEnumerable<Genre> oldGenres, IEnumerable<Genre> newGenres)
        {
            foreach (Genre genre in oldGenres)
            {
                Genres.Remove(genre);
            }

            foreach (Genre genre in newGenres)
            {
                Genres.Add(genre);
            }
        }
    }
}
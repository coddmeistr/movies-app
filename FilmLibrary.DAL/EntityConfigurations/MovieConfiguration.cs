using FilmLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmLibrary.DAL.EntityConfigurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.Property(f => f.Name).IsRequired().HasMaxLength(50);
            builder.Property(f => f.Description).HasMaxLength(250);
            builder.Property(f => f.Articul);

            builder.HasMany(p => p.Genres)
                .WithMany(t => t.Movies)
                .UsingEntity(j => j.ToTable("MovieGenre"));

            builder.HasMany(p => p.Artists)
                .WithMany(t => t.Movies)
                .UsingEntity(j => j.ToTable("MovieArtist"));
        }
    }
}
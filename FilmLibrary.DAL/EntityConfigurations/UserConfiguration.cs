using FilmLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmLibrary.DAL.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(f => f.Name).HasMaxLength(50);
            builder.Property(f => f.PasswordHash);
            builder.Property(f => f.PasswordSalt);
            builder.Property(f => f.UserRole);

            builder.HasMany(p => p.FavouriteMovies)
                .WithMany(t => t.Users)
                .UsingEntity(j => j.ToTable("UserMovie"));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmLibrary.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Articul = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieArtist",
                columns: table => new
                {
                    ArtistsId = table.Column<int>(type: "int", nullable: false),
                    MoviesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieArtist", x => new { x.ArtistsId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_MovieArtist_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieArtist_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenre",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "int", nullable: false),
                    MoviesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenre", x => new { x.GenresId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_MovieGenre_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieGenre_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMovie",
                columns: table => new
                {
                    FavouriteMoviesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMovie", x => new { x.FavouriteMoviesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserMovie_Movies_FavouriteMoviesId",
                        column: x => x.FavouriteMoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMovie_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("INSERT INTO Genres (Name) VALUES ('Horror'), ('Comedy'), ('Sitcom'), ('Action')," +
                "('Thriller'), ('Drama'), ('Detective'), ('Romcom'), ('Adventures'), ('Tragedy')");

            migrationBuilder.Sql("INSERT INTO Users (Name, PasswordHash, PasswordSalt, UserRole) VALUES ('Maxim', 'CpkNP/Rg834eJaN/HOMnxIEXMdGXy5ZlcU/pWF0wXcI=', 'fagXLzrDjFFIJL9sXi6uIA==', 1)", true);
            migrationBuilder.Sql("INSERT INTO Users (Name, PasswordHash, PasswordSalt, UserRole) VALUES ('George', 'Qbgapi0dZu2n9Cqd2GIu+uiWxqTBg3EKq5nBf0BXdk8=', '1ZArOGmGtxHsmLSixyOBew==', 0)", true);

            migrationBuilder.Sql("INSERT INTO Movies (Name, Articul, Description) VALUES ('Green Mile', 1, 'Realism drama film based on S.Kings book'), " +
                "('The Shawshank Redemption', 2 , 'Deep movie about prisoners'), ('Schindlers list', 3 , 'American historical drama'), " +
                "('The Lord of the Rings: The Return of the King', 4 , '3 part of great adventure saga'), ('Forest Gump', 5 , 'Movie about meaning of things'), " +
                "('The Lord of the Rings: The Two Towers', 6 , '2 part of great adventure saga'), " +
                "('The Lord of the Rings: The Fellowship of the Ring', 7 , '1 part of great adventure movie'), " +
                "('1+1', 8 , 'Movie about friendship'), ('Pulp Fiction', 9 , 'Philosophical movie about gangsters'), " +
                "('Ivan Vasilyevich changes the profession', 10 , 'Soviet adventure movie'), ('The Lion King', 11 , 'Great movie about lions'), " +
                "('Interstellar', 12 , 'Philosophical movie about space'), ('Coco', 13 , 'American computer-animated fantasy film'), " +
                "('Back to the Future', 14 , 'Adventure movie about time travelling'), ('WALL-E', 15 , 'American computer-animated science fiction film'), " +
                "('The Dark Knight', 16 , 'Nice movie about batman'), ('Inception', 17 , 'Science fiction action film directed by Christopher Nolan'), " +
                "('Fight Club', 18 , 'American film based on the 1996 novel of the same name by Chuck Palahniuk'), " +
                "('Sen to Chihiro no kamikakushi', 19 , 'Japanese animated fantasy film'), " +
                "('Lock, Stock and Two Smoking Barrels', 20 , '1998 crime black comedy film written and directed by Guy Ritchie')", true);

            migrationBuilder.Sql("INSERT INTO Artists (Name) VALUES ('Tom Hanks'), ('Tim Robins'), ('Liam Nison'), " +
                "('Elijah Wood'), ('Robin Right'), ('Ian McKellen'), ('Sean Estin'), ('Omar See'), ('John Travolta'), " +
                "('Yuriy Yakovlev'), ('Matthew Broderik'), ('Matthew Macconahi'), ('Antony Gonsalez'), ('Mickle Fox')," +
                "('Ben Burtt'), ('Cristian Bale'), ('Leonardo DiCaprio'), ('Brad Pitt'), ('Rumi Hiiragi'), ('Jason Flaming')");

            migrationBuilder.Sql("INSERT INTO MovieArtist (ArtistsID, MoviesID) VALUES (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), " +
                "(9, 9), (10, 10), (11, 11), (12, 12), (13, 13), (14, 14), (15, 15), (16, 16), (17, 17), (18, 18), (19, 19), (20, 20)");

            migrationBuilder.Sql("INSERT INTO MovieGenre (GenresID, MoviesID) VALUES (6, 1), (6, 2), (6, 3), (9, 4), (4, 5), (9, 6), (9, 7), (6, 8), " +
                "(7, 9), (2, 10), (6, 11), (6, 12), (9, 13), (9, 14), (9, 15), (6, 16), (9, 17), (5, 18), (9, 19), (4, 20)");

            migrationBuilder.CreateIndex(
                name: "IX_MovieArtist_MoviesId",
                table: "MovieArtist",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenre_MoviesId",
                table: "MovieGenre",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMovie_UsersId",
                table: "UserMovie",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieArtist");

            migrationBuilder.DropTable(
                name: "MovieGenre");

            migrationBuilder.DropTable(
                name: "UserMovie");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

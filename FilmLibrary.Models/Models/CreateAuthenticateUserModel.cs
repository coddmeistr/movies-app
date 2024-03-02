namespace FilmLibrary.Models.Models
{
    public class CreateUserModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        public CreateUserModel() { }

        public CreateUserModel(string name, string role)
        {
            Name = name;
            UserRole = role;
        }

        public CreateUserModel(string name, string role, int id)
        {
            Name = name;
            UserRole = role;
            Id = id;
        }
    }
}
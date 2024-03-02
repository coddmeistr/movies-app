using FilmLibrary.Models.Models;

namespace FilmLibrary.Business.Interfaces
{
    public interface IUserService
    {
        public CreateUserModel CreateUser(CredentialsModel credentialsModel);

        public string GetUserRole(string claim);

        public ICollection<string> GetUserNames();
    }
}
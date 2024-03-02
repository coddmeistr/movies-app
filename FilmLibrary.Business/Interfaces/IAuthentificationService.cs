using FilmLibrary.Core.Entities;
using FilmLibrary.Core.Enums;
using FilmLibrary.Models.Models;

namespace FilmLibrary.Business.Interfaces
{
    public interface IAuthentificationService
    {

        public CreateUserModel Authenticate(CredentialsModel model);

        public byte[] CalculateHash(string password, ref byte[] salt);

        public byte[] GenerateSalt();
    }
}

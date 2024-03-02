using FilmLibrary.Business.Interfaces;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace FilmLibrary.Business.Services
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IUserRepository _userRepository;

        public AuthentificationService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public CreateUserModel Authenticate(CredentialsModel model)
        {
            var user = _userRepository.GetByName(model.UserName);
            if (user is null) return new CreateUserModel();

            var salt = Convert.FromBase64String(user.PasswordSalt);
            string password = model.Password;
            var hashString = Convert.ToBase64String(CalculateHash(password, ref salt));

            if (user.PasswordHash != hashString)
            {
                return new CreateUserModel(); 
            }

            var userModel = new CreateUserModel(model.UserName, user.UserRole.ToString());
            return userModel;
        }

        public byte[] CalculateHash(string password, ref byte[] salt) => KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(salt);
            }

            return salt;
        }
    }
}

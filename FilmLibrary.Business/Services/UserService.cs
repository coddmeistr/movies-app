using FilmLibrary.Business.Interfaces;
using FilmLibrary.Core.Constants;
using FilmLibrary.Core.Entities;
using FilmLibrary.Core.Enums;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.Models.Models;
using Microsoft.Extensions.Logging;

namespace FilmLibrary.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthentificationService _authentificationService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork,
            IAuthentificationService authentificationService, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _authentificationService = authentificationService ?? throw new ArgumentNullException(nameof(authentificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public CreateUserModel CreateUser(CredentialsModel credentialsModel)
        {
            User? user = _userRepository.GetByName(credentialsModel.UserName);
            CreateUserModel userModel;

            if (user is null)
            {
                var salt = _authentificationService.GenerateSalt();
                var saltString = Convert.ToBase64String(salt);
                var hashString = Convert.ToBase64String(_authentificationService.CalculateHash(credentialsModel.Password, ref salt));

                var newUser = new User(credentialsModel.UserName, hashString, saltString, UserRole.customer);

                _userRepository.Create(newUser);
                _logger.LogInformation("Added new user");

                _unitOfWork.SaveChanges();
                _logger.LogInformation(LogMessages.SavedChangesToDbMessage);

                userModel = new CreateUserModel(credentialsModel.UserName, UserRole.customer.ToString(), newUser.Id);

                return userModel;
            }

            userModel = new CreateUserModel();
            _logger.LogInformation("There already was such a user in db while registering");

            return userModel;
        }

        public string GetUserRole(string claim) => claim.Remove(0, 62);

        public ICollection<string> GetUserNames()
        {
            List<User> users = _userRepository.GetAll().ToList();
            var allUserNames = new List<string>();

            foreach (var item in users)
            {
                allUserNames.Add(item.Name);
            }

            return allUserNames;
        }
    }
}

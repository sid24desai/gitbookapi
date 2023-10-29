namespace BookkeeperAPI.Service
{
    using Azure.Core;
    using BookkeeperAPI.Entity;
    #region usings
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using BookkeeperAPI.Service.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    #endregion
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserView> GetUserByIdAsync(Guid userId)
        {
            User? result = await _userRepository.GetUserByIdAsync(userId);

            if (result == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"User with id '{userId}' does not exist");
            }

            UserView user = new UserView()
            {
                DisplayName = result.Credential.DisplayName,
                Email = result.Credential.Email,
                Id = result.Id,
                Preferences = result.Preferences!
            };

            return user;
        }

        public async Task<UserView> CreateNewUserAsync(CreateUserRequest request)
        {
            User user = new User();
            user.Preferences = request.UserPreference;
            user.Credential = new UserCredential()
            {
                UserId = user.Id,
                DisplayName = request.DisplayName,
                Password = request.Password,
                Email = request.Email,
                LastUpdated = DateTime.UtcNow,
                CreatedTime = DateTime.UtcNow,
            };

            await _userRepository.CreateUserAsync(user);

            return new UserView()
            {
                DisplayName = user.Credential.DisplayName,
                Email = user.Credential.Email,
                Id = user.Id,
                Preferences = user.Preferences
            };
        }

        public async Task<UserView> UpdateUserPreferenceAsync(Guid userId, UserPreference preference)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"User with id '{userId}' does not exist");
            }

            user.Preferences = preference;
            await _userRepository.SaveChangesAsync();

            return new UserView()
            {
                DisplayName = user.Credential.DisplayName,
                Email = user.Credential.Email,
                Id = user.Id,
                Preferences = user.Preferences
            };
        }

        public async Task CreatePasswordResetTokenAsync(CreatePasswordResetTokenRequest request)
        {
            User? user = await _userRepository.GetUserByEmailAsync(request.Email!);

            if (user == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"User with email '{request.Email}' does not exist");
            }

            // TODO(BOOKA-25): Update logic to create a redirection link with token as query parameter
            user.Credential.Password = "PasswordReset";
            user.Credential.LastUpdated = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"User with id '{userId}' does not exist");
            }

            await _userRepository.DeleteUserAsync(user);
        }
    }
}

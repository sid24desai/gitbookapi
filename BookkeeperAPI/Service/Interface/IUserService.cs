using BookkeeperAPI.Entity;
using BookkeeperAPI.Model;

namespace BookkeeperAPI.Service.Interface
{
    public interface IUserService
    {
        public Task<UserView> GetUserByIdAsync(Guid userId);

        public Task<UserView> CreateNewUserAsync(CreateUserRequest request);

        public Task<UserView> UpdateUserPreferenceAsync(Guid userId, UserPreference preference);

        public Task CreatePasswordResetTokenAsync(CreatePasswordResetTokenRequest request);

        public Task DeleteUserAsync(Guid userId);
    }
}

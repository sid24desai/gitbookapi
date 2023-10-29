namespace BookkeeperAPI.Repository.Interface
{

    #region usings
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public interface IUserRepository
    {
        public Task<User?> GetUserByIdAsync(Guid userId);

        public Task<User?> GetUserByEmailAsync([EmailAddress] string email);

        public Task CreateUserAsync(User user);

        public Task SaveChangesAsync();

        public Task DeleteUserAsync(User user);
    }
}

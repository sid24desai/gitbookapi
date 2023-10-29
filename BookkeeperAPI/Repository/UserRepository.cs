namespace BookkeeperAPI.Repository
{
    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class UserRepository : IUserRepository
    {
        private readonly BookkeeperContext _context;
        public UserRepository(BookkeeperContext context) 
        {
            _context = context;
        }


        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync([EmailAddress] string email)
        {
            return await _context.Users.Where(x => x.Credential.Email == email).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.Credentials.AddAsync(user.Credential);

            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

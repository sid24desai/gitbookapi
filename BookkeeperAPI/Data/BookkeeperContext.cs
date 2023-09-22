namespace BookkeeperAPI.Data
{
    #region usings
    using BookkeeperAPI.Entity;
    using Microsoft.EntityFrameworkCore;
    #endregion

    public class BookkeeperContext : DbContext
    {
        public BookkeeperContext(DbContextOptions<BookkeeperContext> options) 
            :base(options)
        { }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<UserCredential> Credentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Expenses)
                .WithOne(u => u.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Credential)
                .WithOne(c => c.User)
                .HasForeignKey<UserCredential>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}

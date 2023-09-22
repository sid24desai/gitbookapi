using BookkeeperAPI.Entity;

namespace BookkeeperAPI.Model
{
    public class UserView
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public UserPreference Preferences { get; set; }
    }
}

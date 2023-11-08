namespace BookkeeperAPI.Model
{
    public class JwtAccessToken
    {
        public string? AccessToken { get; set; }

        public DateTime ExpiresAt { get; set; }
        
        public Guid TokenId { get; set; }
    }
}

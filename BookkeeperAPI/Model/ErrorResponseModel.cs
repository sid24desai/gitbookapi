namespace BookkeeperAPI.Model
{
    public class ErrorResponseModel
    {
        public int StatusCode { get; set; }

        public string? ErrorMessage { get; set; }
    }
}

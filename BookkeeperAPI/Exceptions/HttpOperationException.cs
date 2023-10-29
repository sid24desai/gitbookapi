namespace BookkeeperAPI.Exceptions
{
    public class HttpOperationException : Exception
    {
        public int StatusCode { get; private set; }

        public HttpOperationException() : base() { }

        public HttpOperationException(string message) : base(message) 
        {
            this.StatusCode = StatusCodes.Status500InternalServerError;
        }

        public HttpOperationException(int statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}

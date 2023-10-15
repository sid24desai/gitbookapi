namespace BookkeeperAPI.Utility
{
    internal static class Parser
    {
        internal static T ToEnum<T>(this string value) where T : struct  
        { 
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            T result;

            return Enum.TryParse<T>(value, out result) ? result : throw new Exception("Invalid value");
        }
    }
}

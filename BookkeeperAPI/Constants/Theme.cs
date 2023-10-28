namespace BookkeeperAPI.Constants
{
    #region usings
    using System.Text.Json.Serialization;
    #endregion

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Theme
    {
        Default,
        Dark,
        Light
    }
}

namespace BookkeeperAPI.Entity
{
    #region usings
    using BookkeeperAPI.Constants;
    using Microsoft.EntityFrameworkCore;
    using System.Text.Json.Serialization;
    #endregion

    [Keyless]
    public class UserPreference
    {
        public Currency DefaultCurrency { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Theme DefaultTheme { get; set; }

        public bool DailyReminder { get; set; }
    }
}

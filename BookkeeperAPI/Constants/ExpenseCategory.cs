namespace BookkeeperAPI.Constants
{
    #region usings
    using System.Text.Json.Serialization;
    #endregion

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExpenseCategory
    {
        shopping,
        food,
        housing,
        transportation,
        vehicle,
        entertainment,
        communication,
        financialExpenses,
        investment
    }
}

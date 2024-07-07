using Axpo;

public static class Utilities
{
    public static string GetFormattedDateTime(string timeZoneId, string format)
    {
        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime dateToRetrieve = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
        
        string formattedDateTime = dateToRetrieve.ToString(format);

        return formattedDateTime;
    }

    public static DateTime GetCurrentDateTime(string timeZoneId)
    {

        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime dateToRetrieve = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
     
        return dateToRetrieve;
    }

  public static void PrintTrades(IEnumerable<PowerTrade> trades)
    {

        foreach (PowerTrade trade in trades)
        {
            Console.WriteLine($"Trade ID: {trade.TradeId}");

            foreach (PowerPeriod period in trade.Periods)
            {
                Console.WriteLine($"Period {period.Period}: Volume: {period.Volume}");
            }
        }
    }
}
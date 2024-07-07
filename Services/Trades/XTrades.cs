using Axpo;

public static class XTrades
{
    public static async Task<Dictionary<int, double>> GetTotalVolumePerPeriodPerDay(DateTime dateToRetrieve)
    {
        try
        {
            IEnumerable<PowerTrade> trades = await RetrieveTradesAsync(dateToRetrieve);
            Dictionary<int, double> total = CalculateTotalVolumeByPeriod(trades);
            
            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTotalVolumePerPeriodPerDay: {ex.Message}");
            throw; 
        }
    }

    private static async Task<IEnumerable<PowerTrade>> RetrieveTradesAsync(DateTime dateToRetrieve)
    {
        try
        {
            PowerService powerService = new PowerService();
            IEnumerable<PowerTrade> trades = await powerService.GetTradesAsync(dateToRetrieve);

            return trades;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RetrieveTradesAsync: {ex.Message}");
            throw;
        }
    }

    private static Dictionary<int, double> CalculateTotalVolumeByPeriod(IEnumerable<PowerTrade> trades)
    {
        Dictionary<int, double> totalVolumesByPeriod = new Dictionary<int, double>();

        foreach (var trade in trades)
        {
            foreach (var period in trade.Periods)
            {
                if (totalVolumesByPeriod.ContainsKey(period.Period))
                {
                    totalVolumesByPeriod[period.Period] += period.Volume;
                }
                else
                {
                    totalVolumesByPeriod.Add(period.Period, period.Volume);
                }
            }
        }
        return totalVolumesByPeriod;
    }
}

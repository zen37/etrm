using Axpo;
using Microsoft.Extensions.Logging;

public class TradesAsync : ITrade
{
    private readonly ILogger<TradesAsync> _log;

    public TradesAsync(ILogger<TradesAsync> logger){
        _log = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Dictionary<int, double>> GetTotalVolumePerPeriodPerDayAsync(DateTime dateToRetrieve)
    {
        try
        {
            IEnumerable<PowerTrade> trades = await RetrieveTradesAsync(dateToRetrieve);
            Dictionary<int, double> total = CalculateTotalVolumeByPeriod(trades);
            return total;
        }
        catch (Exception ex)
        {
            _log.LogError($"Error in GetTotalVolumePerPeriodPerDay (async): {ex.Message}");
            throw; 
        }
    }

    private async Task<IEnumerable<PowerTrade>> RetrieveTradesAsync(DateTime dateToRetrieve)
    {
        try
        {
            PowerService powerService = new PowerService();
            IEnumerable<PowerTrade> trades = await powerService.GetTradesAsync(dateToRetrieve);

            return trades;
        }
        catch (Exception ex)
        {
            _log.LogError($"Error in RetrieveTradesAsync: {ex.Message}");
            throw;
        }
    }

    private Dictionary<int, double> CalculateTotalVolumeByPeriod(IEnumerable<PowerTrade> trades)
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

    public Dictionary<int, double> GetTotalVolumePerPeriodPerDay(DateTime dateToRetrieve)
    {
        throw new NotSupportedException("This implementation does not support synchronous operations.");
    }
}

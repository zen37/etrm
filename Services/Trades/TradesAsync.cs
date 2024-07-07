using System.Runtime.Serialization;
using Axpo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class TradesAsync : ITrade
{
    private readonly IConfiguration _config;
    private readonly ILogger<TradesAsync> _log;

    public TradesAsync(IConfiguration config, ILogger<TradesAsync> logger){
        _config = config ?? throw new ArgumentNullException(nameof(config));
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

            string debugConfigValue = _config["Logging:Debug"];
            bool debugEnabled = bool.TryParse(debugConfigValue, out bool result) ? result : false;
            if (debugEnabled) {
                PrintTrades(trades);
            }

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

    private void PrintTrades(IEnumerable<PowerTrade> trades)
    {
        int counterPeriods = int.TryParse(_config["Logging:Periods"], out int result) ? result : 0 ;

        foreach (PowerTrade trade in trades)
        {
            _log.LogDebug($"Trade ID: {trade.TradeId}");
            int i = 1;
            foreach (PowerPeriod period in trade.Periods)
            {
                _log.LogDebug($"Period {period.Period}: Volume: {period.Volume}");
                if (i >= counterPeriods)
                {
                    break;
                }
                i += 1;
            }
        }
    }

    public Dictionary<int, double> GetTotalVolumePerPeriodPerDay(DateTime dateToRetrieve)
    {
        throw new NotSupportedException("This implementation does not support synchronous operations.");
    }
}

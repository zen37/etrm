public interface ITrade
{
    Dictionary<int, double> GetTotalVolumePerPeriodPerDay(DateTime dateToRetrieve);
    Task<Dictionary<int, double>> GetTotalVolumePerPeriodPerDayAsync(DateTime dateToRetrieve);
}

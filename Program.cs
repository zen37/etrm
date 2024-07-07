﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

try
{
    // Configure Serilog at the beginning
    ConfigureLogging();

    // Main logic starts here
    string environment = ParseEnvironment(args);

    IConfiguration config = LoadConfiguration(environment);

    if (config == null)
        throw new ArgumentNullException(nameof(config));

    var serviceProvider = new ServiceCollection()
        .AddSingleton<IConfiguration>(provider => config)
        .AddTransient<IStorage, FileStorage>()
        .AddTransient<ITrade, TradesAsync>()
        .AddLogging(builder =>
        {
            builder.AddSerilog();
        })
        .BuildServiceProvider();

    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<Program>();

    logger.LogInformation($"Using environment: {environment}");

    string timeZoneId = config["Time:Zone"];
    DateTime dateToRetrieve = Utilities.GetCurrentDateTime(timeZoneId);
    logger.LogInformation($"Date time for {timeZoneId}: {dateToRetrieve}");

    // get trades
    var tradeService = serviceProvider.GetRequiredService<ITrade>();
    Dictionary<int, double> total = await tradeService.GetTotalVolumePerPeriodPerDayAsync(dateToRetrieve);

    //save data
    var exportData = serviceProvider.GetService<IStorage>();
    exportData.Save(total);
}
catch (Exception ex)
{
    Log.Logger.Error(ex, $"ERROR retrieving or processing trades: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}

void ConfigureLogging()
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/power_trades-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
}

string ParseEnvironment(string[] args)
{
    // Default environment if not specified
    string environment = "Development";

    // Parse environment from command-line arguments, if provided
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i].StartsWith("--environment="))
        {
            environment = args[i].Substring("--environment=".Length);
            break;
        }
    }
    return environment;
}

IConfiguration LoadConfiguration(string environment)
{
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"config/appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .Build();

    return config;
}

void GetTrades() {

}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;


try
{
    // Parse environment from command-line arguments
    string environment = ParseEnvironment(args);

    IConfiguration config = LoadConfiguration(environment);

    if (config == null)
        throw new ArgumentNullException(nameof(config));

    // Configure Serilog logging
    ConfigureLogging(config);

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

    int runInterval = int.TryParse(config["RunIntervalMinutes"], out int result) ? result : 60;

    var timer = new System.Threading.Timer(async (_) =>
    {
        try
        {
            string timeZoneId = config["Time:Zone"];
            DateTime dateToRetrieve = Utilities.GetCurrentDateTime(timeZoneId);
            logger.LogInformation($"Date time for {timeZoneId}: {dateToRetrieve}");

            // Fetch trades using ITrade service
            var tradeService = serviceProvider.GetRequiredService<ITrade>();
            Dictionary<int, double> total = await tradeService.GetTotalVolumePerPeriodPerDayAsync(dateToRetrieve);

            // Save data using IStorage service
            var exportData = serviceProvider.GetService<IStorage>();
            exportData.Save(total);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, $"ERROR retrieving or processing trades: {ex.Message}");
        }
    }, null, TimeSpan.Zero, TimeSpan.FromMinutes(runInterval));

    Console.WriteLine("Press Ctrl+C to exit...");
    await Task.Delay(Timeout.Infinite);
}
catch (Exception ex)
{
    Log.Logger.Error(ex, $"ERROR starting application: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}


// Load configuration from environment arguments
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

// Load configuration settings based on environment
IConfiguration LoadConfiguration(string environment)
{
    IConfiguration config = null;

    try
    {
        config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"config/appsettings.{environment}.json", optional: false, reloadOnChange: true)
            .Build();
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine($"Error loading configuration for environment '{environment}': {ex.Message}");
        throw; // Rethrow the exception to stop further execution
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading configuration for environment '{environment}': {ex.Message}");
        throw;
    }

    return config;
}

// Configure Serilog logging based on configuration settings
void ConfigureLogging(IConfiguration config)
{
    string debugConfigValue = config["Logging:Debug"];
    bool debugEnabled = bool.TryParse(debugConfigValue, out bool result) ? result : false;

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Is(debugEnabled ? LogEventLevel.Debug : LogEventLevel.Information)
        .WriteTo.Console(restrictedToMinimumLevel: debugEnabled ? LogEventLevel.Debug : LogEventLevel.Information)
        .WriteTo.File("logs/power_trades-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
}
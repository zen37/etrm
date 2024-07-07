using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class FileStorage : IStorage
{
    private readonly IConfiguration _config;
    private readonly ILogger<FileStorage> _logger;

    public FileStorage(IConfiguration configuration, ILogger<FileStorage> logger)
    {
        _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public void Save(Dictionary<int, double> data)
    {
        try
        {
            string timeZoneId = _config["Time:Zone"];
            string formatDateTime = _config["Time:Format:FileName"];

            string folder = GetFolder();

            string fileName = GetFileName(Utilities.GetFormattedDateTime(timeZoneId, formatDateTime));
            string filePath = Path.Combine(folder, fileName);

            _logger.LogInformation($"File extract path: {filePath}");

            string headerField1 = _config["File:Header:Field1"] ?? "Local Time";
            string headerField2 = _config["File:Header:Field2"] ?? "Volume";
            string valuesSeparator = _config["File:ValueSep"] ?? "\t";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"{headerField1}{valuesSeparator}{headerField2}");
                foreach (var item in data)
                {
                    string hour = Constants.PeriodTime[item.Key];
                    writer.WriteLine($"{hour}{valuesSeparator}{item.Value}");
                }
            }

            _logger.LogInformation($"SUCCESS save data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving data to file: {ex.Message}");
            throw; 
        }
    }

    private string GetFolder()
    {
        string folder = _config["File:Folder"];

        // Ensure the CSV folder exists; create it if it doesn't
        if (!Directory.Exists(folder))
        {
            _logger.LogInformation($"Folder '{folder}' does not exist. Creating...");

            try
            {
                Directory.CreateDirectory(folder);
                _logger.LogInformation($"Folder '{folder}' created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating folder '{folder}': {ex.Message}");
                throw; // Re-throw the exception to propagate it upwards
            }
        }
        return folder;
    }

    private string GetFileName(string datetime)
    {
        try
        {
            string fileName = $"{_config["File:Prefix"]}{_config["File:PrefixSep"]}{datetime}{_config["File:Extension"]}";
            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating file name");
            throw new ApplicationException("Error generating file name", ex);
        }
    }
}

# Publish

`dotnet publish -c Release -r <runtime_identifier> --self-contained true`

Replace <runtime_identifier> with the appropriate identifier for your target platform:

For Windows: win-x64, win-x86, or win-arm
For macOS: osx-arm64, osx-x64
For Linux: linux-x64, linux-arm, etc.

# Settings

config/appsettings.Development.json

```json
{
  "File": {
    "Folder": "extracts",
    "Prefix": "PowerPosition",
    "PrefixSep": "_",
    "Extension": ".csv",
    "Header": {
      "Field1": "Local Time",
      "Field2": "Volume"
    },
    "ValSep": "\t"
  },
  "Time": {
    "Zone": "Europe/London",
    "Format": {
      "FileName": "yyyyMMdd_HHmm",
      "SubFolder": "yyyyMMdd"
    }
  },
  "RunIntervalMinutes": 30,
  "Logging": {
    "Debug": true,
    "Periods": 3
  }
}
```

## File Configuration
- **Folder**: Specifies the folder where files will be saved. In this case, it's set to `extracts`.
- **Prefix**: Defines the prefix for the file names. Here, it's `PowerPosition`.
- **PrefixSep**: Sets the separator between the prefix and the rest of the file name. It's set to `_`.
- **Extension**: Specifies the file extension, which is `.csv`.
- **Header**: Defines the headers for the CSV file.
  - **Field1**: The first field header is **Local Time**.
  - **Field2**: The second field header is **Volume**.
- **ValSep**: Specifies the value separator within the file, which is a tab character (`\t`).

## Time Configuration
- **Zone**: Sets the time zone to **Europe/London**.
- **Format**: Defines the date and time formats.
  - **FileName**: Specifies the format for the file name timestamps. Here, it's `yyyyMMdd_HHmm` (e.g., 20230707_0751).
  - **SubFolder**: Specifies the format for the subfolder timestamps. Here, it's `yyyyMMdd` (e.g., /extracts/20230707/PowerPosition_20240707_1917.csv). - not implemented yet

## Run Interval
- **RunIntervalMinutes**: Sets the interval at which the app is scheduled to run. 

## Logging Configuration
- **Debug**: A boolean value indicating if debug logging is enabled.
- **Periods**: Specifies the number of periods for logging.


# Run

## Windows

1. **Open Command Prompt or PowerShell**:
   - Press `Win + R`, type `cmd` or `powershell`, and press Enter.

2. **Navigate to Application Directory**:
   - Use `cd` command to navigate to the directory where `PowerTrades.exe` is located:
     ```cmd
     cd C:\Path\To\Your\Application
     ```

3. **Run the Application**:
   - Execute the application by typing its name without the `.exe` extension:
     ```cmd
     PowerTrades
     ```

## MacOS

1. **Open Terminal**:
   - Open Terminal from the Applications folder or Spotlight.

2. **Navigate to Application Directory**:
   - Use `cd` command to navigate to the directory where `PowerTrades` is located:
     ```bash
     cd /Path/To/Your/Application
     ```

3. **Run the Application**:
   - Execute the application by typing its name:
     ```bash
     ./PowerTrades
     ```

### Notes:
- Environment is Development by default, for other environment launch application with parameter 

`--environment==<name>`
- Ensure that the application has executable permissions (`chmod +x PowerTrades` on macOS/Linux) if needed.


# Testing

Unit testing not implemented yet.
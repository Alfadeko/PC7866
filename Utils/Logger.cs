namespace PC7866.Utils;

/// <summary>
/// Sistema de logging simple para la aplicación
/// </summary>
public class Logger
{
    private static readonly object _lock = new();
    private static Logger? _instance;
    private readonly string _logFilePath;

    public static Logger Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new Logger();
                }
            }
            return _instance;
        }
    }

    private Logger()
    {
        string logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PC7866", "Logs");

        Directory.CreateDirectory(logDirectory);

        _logFilePath = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyyMMdd}.txt");
    }

    public void Log(LogLevel level, string message)
    {
        try
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            }

            // También escribir en consola de debug
            System.Diagnostics.Debug.WriteLine(logMessage);
        }
        catch
        {
            // Evitar excepciones en el logger
        }
    }

    public void Info(string message) => Log(LogLevel.Info, message);
    public void Warning(string message) => Log(LogLevel.Warning, message);
    public void Error(string message) => Log(LogLevel.Error, message);
    public void Debug(string message) => Log(LogLevel.Debug, message);
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

namespace PC7866.Configuration;

/// <summary>
/// Configuración global de la aplicación
/// </summary>
public class AppSettings
{
    private static AppSettings? _instance;
    private static readonly object _lock = new();

    public static AppSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new AppSettings();
                }
            }
            return _instance;
        }
    }

    private AppSettings()
    {
        LoadDefaults();
    }

    // Configuración de comunicación serie
    public string DefaultPortName { get; set; } = "COM4";
    public int DefaultBaudRate { get; set; } = 115200;
    public int DefaultTimeout { get; set; } = 5000;

    // Configuración de base de datos
    public string DatabaseServer { get; set; } = "localhost";
    public string DatabaseName { get; set; } = "pc7866_test";
    public string DatabaseUser { get; set; } = "root";
    public string DatabasePassword { get; set; } = "";
    public int DatabasePort { get; set; } = 3306;

    // Configuración de test
    public int MaxRetries { get; set; } = 3;
    public int DelayBetweenCommandsMs { get; set; } = 100;
    public bool AutoSaveResults { get; set; } = true;

    // Configuración de interfaz
    public string ApplicationTitle { get; set; } = "PC7866 - Test Resistivo Embega";
    public bool ShowDetailedLogs { get; set; } = true;

    public string GetConnectionString()
    {
        return $"Server={DatabaseServer};Port={DatabasePort};Database={DatabaseName};" +
               $"User={DatabaseUser};Password={DatabasePassword};";
    }

    private void LoadDefaults()
    {
        // Aquí se podría cargar desde archivo de configuración
        // Por ahora usa valores por defecto
    }

    public void Save()
    {
        // TODO: Implementar guardado en archivo de configuración
    }
}

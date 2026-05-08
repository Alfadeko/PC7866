namespace PC7866.Models;

/// <summary>
/// Representa un comando individual a enviar al medidor
/// </summary>
public class MeasurementCommand
{
    public int Sequence { get; set; }

    public string Command { get; set; } = string.Empty;

    public string? ExpectedResponsePattern { get; set; }

    public int DelayAfterMs { get; set; } = 100;

    public bool IsCritical { get; set; } = true;

    public string? Description { get; set; }

    public CommandType Type { get; set; } = CommandType.Query;
}

public enum CommandType
{
    Query,      // Solicita información
    Set,        // Configura parámetro
    Execute,    // Ejecuta acción
    Initialize  // Inicialización
}

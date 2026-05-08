namespace PC7866.Models;

/// <summary>
/// Parámetros de configuración para un test resistivo
/// </summary>
public class TestParameters
{
    public int Id { get; set; }

    public string TestName { get; set; } = string.Empty;

    public string DeviceModel { get; set; } = "PC7866";

    public List<MeasurementCommand> CommandSequence { get; set; } = new();

    public int TimeoutMs { get; set; } = 5000;

    public decimal TolerancePercent { get; set; } = 5.0m;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public string? Description { get; set; }
}

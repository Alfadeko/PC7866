namespace PC7866.Models;

/// <summary>
/// Resultado de una medición individual
/// </summary>
public class MeasurementResult
{
    public int Sequence { get; set; }

    public string Command { get; set; } = string.Empty;

    public string Response { get; set; } = string.Empty;

    public decimal? Value { get; set; }

    public bool Success { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public string? ErrorMessage { get; set; }

    public int ResponseTimeMs { get; set; }
}

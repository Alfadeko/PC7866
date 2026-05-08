namespace PC7866.Models;

/// <summary>
/// Respuesta del dispositivo serie
/// </summary>
public class DeviceResponse
{
    public string RawData { get; set; } = string.Empty;

    public bool IsValid { get; set; }

    public DateTime ReceivedAt { get; set; } = DateTime.Now;

    public string? ErrorCode { get; set; }

    public Dictionary<string, string> ParsedValues { get; set; } = new();
}

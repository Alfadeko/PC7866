using PC7866.Models;
using System.Text.RegularExpressions;

namespace PC7866.Services.SerialCommunication;

/// <summary>
/// Parser de comandos y respuestas del dispositivo
/// </summary>
public class CommandParser
{
    /// <summary>
    /// Parsea una respuesta del dispositivo
    /// </summary>
    public DeviceResponse ParseResponse(string rawData)
    {
        var response = new DeviceResponse
        {
            RawData = rawData,
            ReceivedAt = DateTime.Now
        };

        try
        {
            // Verificar si hay código de error
            if (rawData.StartsWith("ERR", StringComparison.OrdinalIgnoreCase))
            {
                response.IsValid = false;
                response.ErrorCode = rawData;
                return response;
            }

            // TODO: Implementar parsing específico según protocolo PC7866
            // Ejemplo genérico:
            response.IsValid = !string.IsNullOrWhiteSpace(rawData);

            // Extraer valores numéricos si existen
            ExtractNumericValues(rawData, response);

        }
        catch (Exception ex)
        {
            response.IsValid = false;
            response.ErrorCode = $"Parse error: {ex.Message}";
        }

        return response;
    }

    /// <summary>
    /// Extrae el valor numérico de una respuesta
    /// </summary>
    public decimal? ExtractNumericValue(string response)
    {
        // Patrón para extraer números (enteros o decimales)
        var match = Regex.Match(response, @"[-+]?\d+\.?\d*");

        if (match.Success && decimal.TryParse(match.Value, System.Globalization.NumberStyles.Any, 
            System.Globalization.CultureInfo.InvariantCulture, out decimal value))
        {
            return value;
        }

        return null;
    }

    /// <summary>
    /// Valida si una respuesta coincide con un patrón esperado
    /// </summary>
    public bool ValidateResponse(string response, string? expectedPattern)
    {
        if (string.IsNullOrEmpty(expectedPattern))
            return true;

        try
        {
            return Regex.IsMatch(response, expectedPattern, RegexOptions.IgnoreCase);
        }
        catch
        {
            return response.Contains(expectedPattern, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Formatea un comando para enviar al dispositivo
    /// </summary>
    public string FormatCommand(string command)
    {
        // Eliminar espacios extras y normalizar
        return command.Trim().ToUpperInvariant();
    }

    private void ExtractNumericValues(string rawData, DeviceResponse response)
    {
        // Patrón para encontrar pares clave=valor
        var matches = Regex.Matches(rawData, @"(\w+)[=:]\s*([-+]?\d+\.?\d*)");

        foreach (Match match in matches)
        {
            if (match.Groups.Count >= 3)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                response.ParsedValues[key] = value;
            }
        }
    }
}

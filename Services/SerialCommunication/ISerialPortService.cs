namespace PC7866.Services.SerialCommunication;

/// <summary>
/// Interfaz para la comunicación serie con el dispositivo
/// </summary>
public interface ISerialPortService : IDisposable
{
    bool IsOpen { get; }

    string? CurrentPort { get; }

    event EventHandler<string>? DataReceived;
    event EventHandler<string>? ErrorOccurred;
    event EventHandler? PortOpened;
    event EventHandler? PortClosed;

    /// <summary>
    /// Obtiene la lista de puertos serie disponibles
    /// </summary>
    string[] GetAvailablePorts();

    /// <summary>
    /// Abre el puerto serie
    /// </summary>
    bool Open(string portName, int baudRate = 9600, int dataBits = 8, System.IO.Ports.Parity parity = System.IO.Ports.Parity.None, System.IO.Ports.StopBits stopBits = System.IO.Ports.StopBits.One);

    /// <summary>
    /// Cierra el puerto serie
    /// </summary>
    void Close();

    /// <summary>
    /// Envía un comando y espera respuesta de forma asíncrona
    /// </summary>
    Task<string> SendCommandAsync(string command, int timeoutMs = 5000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía datos sin esperar respuesta
    /// </summary>
    Task SendDataAsync(string data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Limpia los buffers de entrada y salida
    /// </summary>
    void ClearBuffers();
}

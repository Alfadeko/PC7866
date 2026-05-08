using System.IO.Ports;
using System.Text;

namespace PC7866.Services.SerialCommunication;

/// <summary>
/// Implementación del servicio de comunicación serie
/// </summary>
public class SerialPortService : ISerialPortService
{
    private SerialPort? _serialPort;
    private readonly object _lock = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private TaskCompletionSource<string>? _responseTask;
    private readonly StringBuilder _receiveBuffer = new();

    public bool IsOpen => _serialPort?.IsOpen ?? false;
    public string? CurrentPort => _serialPort?.PortName;

    public event EventHandler<string>? DataReceived;
    public event EventHandler<string>? ErrorOccurred;
    public event EventHandler? PortOpened;
    public event EventHandler? PortClosed;

    public string[] GetAvailablePorts()
    {
        return SerialPort.GetPortNames();
    }

    public bool Open(string portName, int baudRate = 115200, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
    {
        lock (_lock)
        {
            try
            {
                if (IsOpen)
                {
                    Close();
                }

                _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
                {
                    ReadTimeout = 500,
                    WriteTimeout = 500,
                    Encoding = Encoding.ASCII,
                    NewLine = "\r\n",
                    DtrEnable = true,
                    RtsEnable = true
                };

                _serialPort.DataReceived += OnSerialDataReceived;
                _serialPort.ErrorReceived += OnSerialErrorReceived;

                _serialPort.Open();

                ClearBuffers();

                PortOpened?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error opening port {portName}: {ex.Message}");
                return false;
            }
        }
    }

    public void Close()
    {
        lock (_lock)
        {
            try
            {
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }

                    _serialPort.DataReceived -= OnSerialDataReceived;
                    _serialPort.ErrorReceived -= OnSerialErrorReceived;
                    _serialPort.Dispose();
                    _serialPort = null;
                }

                _receiveBuffer.Clear();
                PortClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error closing port: {ex.Message}");
            }
        }
    }

    public async Task<string> SendCommandAsync(string command, int timeoutMs = 5000, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (!IsOpen || _serialPort == null)
            {
                throw new InvalidOperationException("Puerto serie no está abierto");
            }

            _receiveBuffer.Clear();
            _responseTask = new TaskCompletionSource<string>();

            await _serialPort.BaseStream.WriteAsync(Encoding.ASCII.GetBytes(command + "\r\n"), cancellationToken);
            await _serialPort.BaseStream.FlushAsync(cancellationToken);

            using var timeoutCts = new CancellationTokenSource(timeoutMs);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var timeoutTask = Task.Delay(timeoutMs, linkedCts.Token);
            var responseTask = _responseTask.Task;

            var completedTask = await Task.WhenAny(responseTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                throw new TimeoutException($"Timeout esperando respuesta del comando: {command}");
            }

            return await responseTask;
        }
        finally
        {
            _responseTask = null;
            _semaphore.Release();
        }
    }

    public async Task SendDataAsync(string data, CancellationToken cancellationToken = default)
    {
        if (!IsOpen || _serialPort == null)
        {
            throw new InvalidOperationException("Puerto serie no está abierto");
        }

        await _serialPort.BaseStream.WriteAsync(Encoding.ASCII.GetBytes(data + "\r\n"), cancellationToken);
        await _serialPort.BaseStream.FlushAsync(cancellationToken);
    }

    public void ClearBuffers()
    {
        if (_serialPort?.IsOpen == true)
        {
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
        }
        _receiveBuffer.Clear();
    }

    private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_serialPort == null || !_serialPort.IsOpen)
                return;

            string data = _serialPort.ReadExisting();
            _receiveBuffer.Append(data);

            string bufferContent = _receiveBuffer.ToString();

            if (bufferContent.Contains('\n') || bufferContent.Contains('\r'))
            {
                string completeResponse = bufferContent.Trim();
                _receiveBuffer.Clear();

                DataReceived?.Invoke(this, completeResponse);

                _responseTask?.TrySetResult(completeResponse);
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Error leyendo datos serie: {ex.Message}");
            _responseTask?.TrySetException(ex);
        }
    }

    private void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        ErrorOccurred?.Invoke(this, $"Error en puerto serie: {e.EventType}");
    }

    public void Dispose()
    {
        Close();
        _semaphore?.Dispose();
        GC.SuppressFinalize(this);
    }
}

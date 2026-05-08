using PC7866.Models;
using PC7866.Services.SerialCommunication;

namespace PC7866.Services.StateMachine;

/// <summary>
/// Contexto compartido que fluye por todos los estados de la máquina
/// </summary>
public class TestContext
{
    public TestParameters Parameters { get; set; } = null!;
    public TestResult Result { get; set; } = null!;
    public ISerialPortService SerialPort { get; set; } = null!;
    public CommandParser Parser { get; set; } = null!;
    public CancellationToken CancellationToken { get; set; }
    public IProgress<TestProgressReport>? Progress { get; set; }
}

/// <summary>
/// Reporte de progreso del test
/// </summary>
public class TestProgressReport
{
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public string Message { get; set; } = string.Empty;
    public TestState State { get; set; }
}

/// <summary>
/// Estados posibles de la máquina
/// </summary>
public enum TestState
{
    Idle,
    Initializing,
    Running,
    Completed,
    Error,
    Aborted
}

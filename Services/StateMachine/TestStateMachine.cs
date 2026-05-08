using PC7866.Models;
using PC7866.Services.SerialCommunication;
using PC7866.Services.StateMachine.States;

namespace PC7866.Services.StateMachine;

/// <summary>
/// Máquina de estados que gestiona la ejecución automática de un test
/// </summary>
public class TestStateMachine
{
    private readonly Dictionary<TestState, ITestState> _states;
    private TestState _currentState = TestState.Idle;

    public TestState CurrentState => _currentState;
    public event EventHandler<TestState>? StateChanged;

    public TestStateMachine()
    {
        _states = new Dictionary<TestState, ITestState>
        {
            { TestState.Initializing, new InitializingState() },
            { TestState.Running,      new RunningState()      },
            { TestState.Completed,    new CompletedState()    }
        };
    }

    /// <summary>
    /// Ejecuta el test completo y devuelve el resultado
    /// </summary>
    public async Task<TestResult> RunAsync(
        TestParameters parameters,
        ISerialPortService serialPort,
        CommandParser parser,
        IProgress<TestProgressReport>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new TestResult
        {
            TestParametersId = parameters.Id,
            ExecutionDate    = DateTime.Now,
            Status           = TestStatus.NotStarted
        };

        var context = new TestContext
        {
            Parameters        = parameters,
            Result            = result,
            SerialPort        = serialPort,
            Parser            = parser,
            CancellationToken = cancellationToken,
            Progress          = progress
        };

        _currentState = TestState.Initializing;
        StateChanged?.Invoke(this, _currentState);

        // Bucle de transición entre estados
        while (_currentState != TestState.Idle   &&
               _currentState != TestState.Error  &&
               _currentState != TestState.Aborted)
        {
            if (!_states.TryGetValue(_currentState, out var state))
                break;

            _currentState = await state.ExecuteAsync(context);
            StateChanged?.Invoke(this, _currentState);
        }

        return result;
    }
}

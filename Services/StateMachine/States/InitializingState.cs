using PC7866.Models;

namespace PC7866.Services.StateMachine.States;

/// <summary>
/// Estado: inicialización – verifica que el puerto esté abierto y prepara el resultado
/// </summary>
public class InitializingState : ITestState
{
    public TestState StateId => TestState.Initializing;

    public Task<TestState> ExecuteAsync(TestContext context)
    {
        context.Progress?.Report(new TestProgressReport
        {
            CurrentStep = 0,
            TotalSteps  = context.Parameters.CommandSequence.Count,
            Message     = "Inicializando test…",
            State       = TestState.Initializing
        });

        if (!context.SerialPort.IsOpen)
        {
            context.Result.Status = TestStatus.Error;
            context.Result.Observations = "Puerto serie no está abierto";
            return Task.FromResult(TestState.Error);
        }

        context.Result.ExecutionDate = DateTime.Now;
        context.Result.Status        = TestStatus.Running;
        context.Result.Measurements.Clear();

        return Task.FromResult(TestState.Running);
    }
}

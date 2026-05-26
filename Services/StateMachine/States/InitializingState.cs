namespace PC7866.Services.StateMachine.States;

/// <summary>
/// Estado: inicialización – verifica que el puerto esté abierto y prepara el resultado.
/// </summary>
public class InitializingState : ITestState
{
    public TestState StateId => TestState.Initializing;

    public Task<TestState> ExecuteAsync(TestContext context)
    {
        context.Progress?.Report(new TestProgressReport
        {
            CurrentStep = 0,
            TotalSteps  = context.Parametros.Count,
            Message     = "Inicializando ensayo…",
            State       = TestState.Initializing
        });

        if (!context.SerialPort.IsOpen)
        {
            context.Resultado.ResultadoGlobal = false;
            return Task.FromResult(TestState.Error);
        }

        context.Resultado.FechaPrueba = DateTime.Now;
        context.Resultado.Detalles.Clear();

        return Task.FromResult(TestState.Running);
    }
}

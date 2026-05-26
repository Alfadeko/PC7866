namespace PC7866.Services.StateMachine.States;

/// <summary>
/// Estado: completado – emite el reporte final de progreso.
/// </summary>
public class CompletedState : ITestState
{
    public TestState StateId => TestState.Completed;

    public Task<TestState> ExecuteAsync(TestContext context)
    {
        int total  = context.Parametros.Count;
        int passed = context.Resultado.Detalles.Count(d => d.Resultado);
        string icon = context.Resultado.ResultadoGlobal ? "✅" : "❌";

        context.Progress?.Report(new TestProgressReport
        {
            CurrentStep = total,
            TotalSteps  = total,
            Message     = $"{icon} Ensayo completado: {passed}/{total} pasos OK",
            State       = TestState.Completed,
            StepOk      = context.Resultado.ResultadoGlobal
        });

        return Task.FromResult(TestState.Idle);
    }
}

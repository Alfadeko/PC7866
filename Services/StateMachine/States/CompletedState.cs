using PC7866.Models;

namespace PC7866.Services.StateMachine.States;

/// <summary>
/// Estado: completado – calcula duración y emite el reporte final
/// </summary>
public class CompletedState : ITestState
{
    public TestState StateId => TestState.Completed;

    public Task<TestState> ExecuteAsync(TestContext context)
    {
        context.Result.Duration = DateTime.Now - context.Result.ExecutionDate;

        string statusIcon = context.Result.Status == TestStatus.Passed ? "✅" : "❌";
        int passed = context.Result.Measurements.Count(m => m.Success);
        int total  = context.Result.Measurements.Count;

        context.Progress?.Report(new TestProgressReport
        {
            CurrentStep = total,
            TotalSteps  = total,
            Message     = $"{statusIcon} Test completado: {passed}/{total} mediciones correctas " +
                          $"en {context.Result.Duration.TotalSeconds:F1}s",
            State       = TestState.Completed
        });

        return Task.FromResult(TestState.Idle);
    }
}

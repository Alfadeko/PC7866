namespace PC7866.Services.StateMachine;

/// <summary>
/// Interfaz que deben implementar todos los estados de la máquina
/// </summary>
public interface ITestState
{
    TestState StateId { get; }
    Task<TestState> ExecuteAsync(TestContext context);
}

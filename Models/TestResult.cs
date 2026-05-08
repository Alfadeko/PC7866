namespace PC7866.Models;

/// <summary>
/// Resultado completo de la ejecución de un test
/// </summary>
public class TestResult
{
    public int Id { get; set; }

    public int TestParametersId { get; set; }

    public DateTime ExecutionDate { get; set; } = DateTime.Now;

    public TestStatus Status { get; set; } = TestStatus.NotStarted;

    public List<MeasurementResult> Measurements { get; set; } = new();

    public string? Observations { get; set; }

    public TimeSpan Duration { get; set; }

    public string? OperatorName { get; set; }

    public string? SerialNumber { get; set; }
}

public enum TestStatus
{
    NotStarted,
    Running,
    Passed,
    Failed,
    Error,
    Aborted
}

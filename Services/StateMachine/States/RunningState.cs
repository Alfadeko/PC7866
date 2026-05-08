using PC7866.Models;

namespace PC7866.Services.StateMachine.States;

/// <summary>
/// Estado: ejecución – recorre la secuencia de comandos uno a uno
/// </summary>
public class RunningState : ITestState
{
    public TestState StateId => TestState.Running;

    public async Task<TestState> ExecuteAsync(TestContext context)
    {
        var commands  = context.Parameters.CommandSequence
                               .OrderBy(c => c.Sequence)
                               .ToList();
        int total     = commands.Count;
        bool anyFail  = false;

        for (int i = 0; i < total; i++)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                context.Result.Status = TestStatus.Aborted;
                return TestState.Aborted;
            }

            var cmd = commands[i];

            context.Progress?.Report(new TestProgressReport
            {
                CurrentStep = i + 1,
                TotalSteps  = total,
                Message     = $"[{i + 1}/{total}] {cmd.Description ?? cmd.Command}",
                State       = TestState.Running
            });

            var measurement = await ExecuteCommandAsync(cmd, context);
            context.Result.Measurements.Add(measurement);

            if (!measurement.Success && cmd.IsCritical)
            {
                context.Result.Status = TestStatus.Failed;
                context.Result.Observations = $"Fallo crítico en paso {i + 1}: {cmd.Command}";
                return TestState.Completed;
            }

            if (!measurement.Success)
                anyFail = true;

            if (cmd.DelayAfterMs > 0)
                await Task.Delay(cmd.DelayAfterMs, context.CancellationToken);
        }

        context.Result.Status = anyFail ? TestStatus.Failed : TestStatus.Passed;
        return TestState.Completed;
    }

    private static async Task<MeasurementResult> ExecuteCommandAsync(
        MeasurementCommand cmd, TestContext context)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var measurement = new MeasurementResult
        {
            Sequence  = cmd.Sequence,
            Command   = cmd.Command,
            Timestamp = DateTime.Now
        };

        try
        {
            string response = await context.SerialPort.SendCommandAsync(
                cmd.Command,
                context.Parameters.TimeoutMs,
                context.CancellationToken);

            sw.Stop();
            measurement.Response       = response;
            measurement.ResponseTimeMs = (int)sw.ElapsedMilliseconds;

            var parsed = context.Parser.ParseResponse(response);
            measurement.Value   = context.Parser.ExtractNumericValue(response);
            measurement.Success = parsed.IsValid &&
                                  context.Parser.ValidateResponse(response, cmd.ExpectedResponsePattern);

            if (!measurement.Success)
                measurement.ErrorMessage = $"Respuesta inesperada: {response}";
        }
        catch (TimeoutException)
        {
            sw.Stop();
            measurement.Success      = false;
            measurement.ErrorMessage = "Timeout esperando respuesta";
            measurement.ResponseTimeMs = (int)sw.ElapsedMilliseconds;
        }
        catch (Exception ex)
        {
            sw.Stop();
            measurement.Success      = false;
            measurement.ErrorMessage = ex.Message;
            measurement.ResponseTimeMs = (int)sw.ElapsedMilliseconds;
        }

        return measurement;
    }
}

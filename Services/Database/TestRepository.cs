using Dapper;
using MySqlConnector;
using PC7866.Models;
using System.Text.Json;

namespace PC7866.Services.Database;

/// <summary>
/// Implementación de acceso a MariaDB mediante Dapper
/// </summary>
public class TestRepository : ITestRepository
{
    private readonly string _connectionString;

    public TestRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    // ─────────────────────────────────────────────────────────────────────────
    // TestParameters
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<TestParameters>> GetAllTestParametersAsync()
    {
        const string sql = """
            SELECT id, test_name, device_model, timeout_ms, tolerance_percent,
                   created_date, description, command_sequence_json
            FROM test_parameters
            ORDER BY created_date DESC
            """;

        using var conn = CreateConnection();
        var rows = await conn.QueryAsync(sql);
        return rows.Select(MapTestParameters);
    }

    public async Task<TestParameters?> GetTestParametersByIdAsync(int id)
    {
        const string sql = """
            SELECT id, test_name, device_model, timeout_ms, tolerance_percent,
                   created_date, description, command_sequence_json
            FROM test_parameters
            WHERE id = @Id
            """;

        using var conn = CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(sql, new { Id = id });
        return row is null ? null : MapTestParameters(row);
    }

    public async Task<int> InsertTestParametersAsync(TestParameters p)
    {
        const string sql = """
            INSERT INTO test_parameters
                (test_name, device_model, timeout_ms, tolerance_percent,
                 created_date, description, command_sequence_json)
            VALUES
                (@TestName, @DeviceModel, @TimeoutMs, @TolerancePercent,
                 @CreatedDate, @Description, @CommandSequenceJson);
            SELECT LAST_INSERT_ID();
            """;

        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            p.TestName,
            p.DeviceModel,
            p.TimeoutMs,
            p.TolerancePercent,
            p.CreatedDate,
            p.Description,
            CommandSequenceJson = JsonSerializer.Serialize(p.CommandSequence)
        });
    }

    public async Task UpdateTestParametersAsync(TestParameters p)
    {
        const string sql = """
            UPDATE test_parameters SET
                test_name           = @TestName,
                device_model        = @DeviceModel,
                timeout_ms          = @TimeoutMs,
                tolerance_percent   = @TolerancePercent,
                description         = @Description,
                command_sequence_json = @CommandSequenceJson
            WHERE id = @Id
            """;

        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql, new
        {
            p.Id,
            p.TestName,
            p.DeviceModel,
            p.TimeoutMs,
            p.TolerancePercent,
            p.Description,
            CommandSequenceJson = JsonSerializer.Serialize(p.CommandSequence)
        });
    }

    public async Task DeleteTestParametersAsync(int id)
    {
        const string sql = "DELETE FROM test_parameters WHERE id = @Id";
        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TestResult
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<TestResult>> GetAllTestResultsAsync()
    {
        const string sql = """
            SELECT id, test_parameters_id, execution_date, status,
                   observations, duration_ms, operator_name, serial_number,
                   measurements_json
            FROM test_results
            ORDER BY execution_date DESC
            """;

        using var conn = CreateConnection();
        var rows = await conn.QueryAsync(sql);
        return rows.Select(MapTestResult);
    }

    public async Task<IEnumerable<TestResult>> GetTestResultsByParametersIdAsync(int parametersId)
    {
        const string sql = """
            SELECT id, test_parameters_id, execution_date, status,
                   observations, duration_ms, operator_name, serial_number,
                   measurements_json
            FROM test_results
            WHERE test_parameters_id = @ParametersId
            ORDER BY execution_date DESC
            """;

        using var conn = CreateConnection();
        var rows = await conn.QueryAsync(sql, new { ParametersId = parametersId });
        return rows.Select(MapTestResult);
    }

    public async Task<TestResult?> GetTestResultByIdAsync(int id)
    {
        const string sql = """
            SELECT id, test_parameters_id, execution_date, status,
                   observations, duration_ms, operator_name, serial_number,
                   measurements_json
            FROM test_results
            WHERE id = @Id
            """;

        using var conn = CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(sql, new { Id = id });
        return row is null ? null : MapTestResult(row);
    }

    public async Task<int> InsertTestResultAsync(TestResult r)
    {
        const string sql = """
            INSERT INTO test_results
                (test_parameters_id, execution_date, status, observations,
                 duration_ms, operator_name, serial_number, measurements_json)
            VALUES
                (@TestParametersId, @ExecutionDate, @Status, @Observations,
                 @DurationMs, @OperatorName, @SerialNumber, @MeasurementsJson);
            SELECT LAST_INSERT_ID();
            """;

        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            r.TestParametersId,
            r.ExecutionDate,
            Status = r.Status.ToString(),
            r.Observations,
            DurationMs = (long)r.Duration.TotalMilliseconds,
            r.OperatorName,
            r.SerialNumber,
            MeasurementsJson = JsonSerializer.Serialize(r.Measurements)
        });
    }

    public async Task UpdateTestResultAsync(TestResult r)
    {
        const string sql = """
            UPDATE test_results SET
                status           = @Status,
                observations     = @Observations,
                duration_ms      = @DurationMs,
                measurements_json = @MeasurementsJson
            WHERE id = @Id
            """;

        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql, new
        {
            r.Id,
            Status = r.Status.ToString(),
            r.Observations,
            DurationMs = (long)r.Duration.TotalMilliseconds,
            MeasurementsJson = JsonSerializer.Serialize(r.Measurements)
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Utilidades
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task InitializeDatabaseAsync()
    {
        const string createTestParameters = """
            CREATE TABLE IF NOT EXISTS test_parameters (
                id                    INT AUTO_INCREMENT PRIMARY KEY,
                test_name             VARCHAR(100) NOT NULL,
                device_model          VARCHAR(50)  NOT NULL DEFAULT 'PC7866',
                timeout_ms            INT          NOT NULL DEFAULT 5000,
                tolerance_percent     DECIMAL(5,2) NOT NULL DEFAULT 5.00,
                created_date          DATETIME     NOT NULL,
                description           TEXT,
                command_sequence_json LONGTEXT
            );
            """;

        const string createTestResults = """
            CREATE TABLE IF NOT EXISTS test_results (
                id                  INT AUTO_INCREMENT PRIMARY KEY,
                test_parameters_id  INT          NOT NULL,
                execution_date      DATETIME     NOT NULL,
                status              VARCHAR(20)  NOT NULL,
                observations        TEXT,
                duration_ms         BIGINT       NOT NULL DEFAULT 0,
                operator_name       VARCHAR(100),
                serial_number       VARCHAR(100),
                measurements_json   LONGTEXT,
                FOREIGN KEY (test_parameters_id) REFERENCES test_parameters(id)
            );
            """;

        using var conn = CreateConnection();
        await conn.ExecuteAsync(createTestParameters);
        await conn.ExecuteAsync(createTestResults);
    }

    public void Dispose() { }

    // ─────────────────────────────────────────────────────────────────────────
    // Mappers privados
    // ─────────────────────────────────────────────────────────────────────────

    private static TestParameters MapTestParameters(dynamic row)
    {
        var p = new TestParameters
        {
            Id              = (int)row.id,
            TestName        = (string)row.test_name,
            DeviceModel     = (string)row.device_model,
            TimeoutMs       = (int)row.timeout_ms,
            TolerancePercent = (decimal)row.tolerance_percent,
            CreatedDate     = (DateTime)row.created_date,
            Description     = (string?)row.description
        };

        string? json = (string?)row.command_sequence_json;
        if (!string.IsNullOrEmpty(json))
        {
            p.CommandSequence = JsonSerializer.Deserialize<List<MeasurementCommand>>(json) ?? new();
        }

        return p;
    }

    private static TestResult MapTestResult(dynamic row)
    {
        var r = new TestResult
        {
            Id               = (int)row.id,
            TestParametersId = (int)row.test_parameters_id,
            ExecutionDate    = (DateTime)row.execution_date,
            Status           = Enum.Parse<TestStatus>((string)row.status),
            Observations     = (string?)row.observations,
            Duration         = TimeSpan.FromMilliseconds((long)row.duration_ms),
            OperatorName     = (string?)row.operator_name,
            SerialNumber     = (string?)row.serial_number
        };

        string? json = (string?)row.measurements_json;
        if (!string.IsNullOrEmpty(json))
        {
            r.Measurements = JsonSerializer.Deserialize<List<MeasurementResult>>(json) ?? new();
        }

        return r;
    }
}

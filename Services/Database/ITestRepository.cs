using PC7866.Models;

namespace PC7866.Services.Database;

/// <summary>
/// Repositorio para gestionar TestParameters y TestResult en la base de datos
/// </summary>
public interface ITestRepository : IDisposable
{
    // --- TestParameters ---
    Task<IEnumerable<TestParameters>> GetAllTestParametersAsync();
    Task<TestParameters?> GetTestParametersByIdAsync(int id);
    Task<int> InsertTestParametersAsync(TestParameters parameters);
    Task UpdateTestParametersAsync(TestParameters parameters);
    Task DeleteTestParametersAsync(int id);

    // --- TestResult ---
    Task<IEnumerable<TestResult>> GetAllTestResultsAsync();
    Task<IEnumerable<TestResult>> GetTestResultsByParametersIdAsync(int parametersId);
    Task<TestResult?> GetTestResultByIdAsync(int id);
    Task<int> InsertTestResultAsync(TestResult result);
    Task UpdateTestResultAsync(TestResult result);

    // --- Utilidades ---
    Task<bool> TestConnectionAsync();
    Task InitializeDatabaseAsync();
}

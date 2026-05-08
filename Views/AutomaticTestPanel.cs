using PC7866.Configuration;
using PC7866.Models;
using PC7866.Services.Database;
using PC7866.Services.SerialCommunication;
using PC7866.Services.StateMachine;
using PC7866.Utils;

namespace PC7866.Views;

/// <summary>
/// Panel de ejecución automática de tests.
/// </summary>
public partial class AutomaticTestPanel : UserControl
{
    private readonly ISerialPortService _serialPort;
    private readonly CommandParser      _parser;
    private readonly TestStateMachine   _stateMachine;
    private ITestRepository?            _repository;
    private CancellationTokenSource?    _cts;

    public AutomaticTestPanel()
    {
        InitializeComponent();

        _serialPort   = new SerialPortService();
        _parser       = new CommandParser();
        _stateMachine = new TestStateMachine();
        _stateMachine.StateChanged += (_, s) => Invoke(() => lblMachineState.Text = $"Estado: {s}");

        InitializeControls();
        AttachEventHandlers();
        _ = TryInitDatabaseAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Inicialización
    // ─────────────────────────────────────────────────────────────────────────

    private void InitializeControls()
    {
        cmbPort.Items.AddRange(_serialPort.GetAvailablePorts());
        if (cmbPort.Items.Count > 0) cmbPort.SelectedIndex = 0;

        cmbBaudRate.Items.AddRange(new object[] { 9600, 19200, 38400, 57600, 115200 });
        cmbBaudRate.SelectedItem = AppSettings.Instance.DefaultBaudRate;

        SetConnectedState(false);
        btnStartTest.Enabled = false;
        btnAbortTest.Enabled = false;
        progressBar.Value    = 0;
        lblCurrentStep.Text  = "—";
        lblMachineState.Text = "Estado: Idle";
    }

    private void AttachEventHandlers()
    {
        btnConnect.Click    += BtnConnect_Click;
        btnDisconnect.Click += BtnDisconnect_Click;
        btnStartTest.Click  += BtnStartTest_Click;
        btnAbortTest.Click  += (_, _) => { _cts?.Cancel(); AddLog("⛔ Abortando…", LogLevel.Warning); };
        btnRefreshPorts.Click += (_, _) =>
        {
            cmbPort.Items.Clear();
            cmbPort.Items.AddRange(_serialPort.GetAvailablePorts());
            if (cmbPort.Items.Count > 0) cmbPort.SelectedIndex = 0;
        };
        btnRefreshProfiles.Click += async (_, _) => await LoadTestParametersAsync();

        _serialPort.PortOpened    += (_, _) => Invoke(() => SetConnectedState(true));
        _serialPort.PortClosed    += (_, _) => Invoke(() => SetConnectedState(false));
        _serialPort.ErrorOccurred += (_, err) => Invoke(() => AddLog($"❌ {err}", LogLevel.Error));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Base de datos
    // ─────────────────────────────────────────────────────────────────────────

    private async Task TryInitDatabaseAsync()
    {
        try
        {
            _repository = new TestRepository(AppSettings.Instance.GetConnectionString());
            if (await _repository.TestConnectionAsync())
            {
                await _repository.InitializeDatabaseAsync();
                AddLog("🗄️ Base de datos conectada", LogLevel.Info);
                await LoadTestParametersAsync();
            }
            else
            {
                AddLog("⚠️ Sin conexión a la base de datos. Compruebe la configuración.", LogLevel.Warning);
            }
        }
        catch (Exception ex)
        {
            AddLog($"⚠️ Error BD: {ex.Message}", LogLevel.Warning);
        }
    }

    private async Task LoadTestParametersAsync()
    {
        if (_repository is null) return;
        try
        {
            var list = await _repository.GetAllTestParametersAsync();
            cmbTestParameters.Items.Clear();
            foreach (var tp in list) cmbTestParameters.Items.Add(tp);
            cmbTestParameters.DisplayMember = "TestName";
            if (cmbTestParameters.Items.Count > 0) cmbTestParameters.SelectedIndex = 0;
            AddLog($"📋 {cmbTestParameters.Items.Count} perfiles cargados", LogLevel.Info);
            UpdateStartButton();
        }
        catch (Exception ex)
        {
            AddLog($"❌ Error cargando perfiles: {ex.Message}", LogLevel.Error);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Conexión serie
    // ─────────────────────────────────────────────────────────────────────────

    private async void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (cmbPort.SelectedItem is null || cmbBaudRate.SelectedItem is null) return;
        btnConnect.Enabled = false;
        string port = cmbPort.SelectedItem.ToString()!;
        int    baud = (int)cmbBaudRate.SelectedItem;
        bool   ok   = await Task.Run(() => _serialPort.Open(port, baud));
        if (!ok) { btnConnect.Enabled = true; AddLog($"❌ No se pudo abrir {port}", LogLevel.Error); }
    }

    private void BtnDisconnect_Click(object? sender, EventArgs e) => _serialPort.Close();

    private void SetConnectedState(bool connected)
    {
        lblConnectionStatus.Text      = connected ? $"● {_serialPort.CurrentPort}" : "○ Sin conexión";
        lblConnectionStatus.ForeColor = connected ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        btnConnect.Enabled    = !connected;
        btnDisconnect.Enabled =  connected;
        cmbPort.Enabled       = !connected;
        cmbBaudRate.Enabled   = !connected;
        UpdateStartButton();
        if (connected) AddLog($"✅ Conectado: {_serialPort.CurrentPort}", LogLevel.Info);
    }

    private void UpdateStartButton() =>
        btnStartTest.Enabled = _serialPort.IsOpen && cmbTestParameters.Items.Count > 0;

    // ─────────────────────────────────────────────────────────────────────────
    // Ejecución del test
    // ─────────────────────────────────────────────────────────────────────────

    private async void BtnStartTest_Click(object? sender, EventArgs e)
    {
        if (cmbTestParameters.SelectedItem is not TestParameters parameters)
        {
            MessageBox.Show("Seleccione un perfil de test.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (parameters.CommandSequence.Count == 0)
        {
            MessageBox.Show("El perfil no tiene comandos definidos.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _cts = new CancellationTokenSource();
        btnStartTest.Enabled = false;
        btnAbortTest.Enabled = true;
        progressBar.Value    = 0;
        progressBar.Maximum  = parameters.CommandSequence.Count;
        txtLog.Clear();

        var progress = new Progress<TestProgressReport>(r =>
        {
            progressBar.Value   = Math.Min(r.CurrentStep, progressBar.Maximum);
            lblCurrentStep.Text = r.Message;
            AddLog(r.Message, LogLevel.Info);
        });

        AddLog($"▶️ Test: {parameters.TestName}", LogLevel.Info);

        try
        {
            var result = await _stateMachine.RunAsync(
                parameters, _serialPort, _parser, progress, _cts.Token);

            ShowTestResult(result);

            if (_repository is not null)
            {
                result.Id = await _repository.InsertTestResultAsync(result);
                AddLog($"💾 Resultado guardado (ID {result.Id})", LogLevel.Info);
            }
        }
        catch (OperationCanceledException)
        {
            AddLog("⛔ Test cancelado", LogLevel.Warning);
        }
        catch (Exception ex)
        {
            AddLog($"❌ {ex.Message}", LogLevel.Error);
            Logger.Instance.Error($"Error test automático: {ex}");
        }
        finally
        {
            btnStartTest.Enabled = true;
            btnAbortTest.Enabled = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void ShowTestResult(TestResult result)
    {
        int    passed  = result.Measurements.Count(m => m.Success);
        int    total   = result.Measurements.Count;
        string icon    = result.Status == TestStatus.Passed ? "✅" : "❌";
        string summary = $"{icon} {result.Status}  |  {passed}/{total} OK  |  {result.Duration.TotalSeconds:F1}s";
        lblCurrentStep.Text = summary;
        AddLog(summary, result.Status == TestStatus.Passed ? LogLevel.Info : LogLevel.Warning);
        if (!string.IsNullOrEmpty(result.Observations))
            AddLog($"ℹ️ {result.Observations}", LogLevel.Warning);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Log
    // ─────────────────────────────────────────────────────────────────────────

    private void AddLog(string message, LogLevel level)
    {
        if (InvokeRequired) { Invoke(() => AddLog(message, level)); return; }
        string ts = DateTime.Now.ToString("HH:mm:ss.fff");
        txtLog.AppendText($"[{ts}] {message}{Environment.NewLine}");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();
        Logger.Instance.Log(level, message);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _cts?.Cancel();
        _serialPort.Dispose();
        _repository?.Dispose();
        base.OnHandleDestroyed(e);
    }
}

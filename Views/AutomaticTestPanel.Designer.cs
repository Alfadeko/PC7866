namespace PC7866.Views;

partial class AutomaticTestPanel
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    private void InitializeComponent()
    {
        grpConnection = new GroupBox();
        lblPort = new Label();
        cmbPort = new ComboBox();
        lblBaudRate = new Label();
        cmbBaudRate = new ComboBox();
        btnRefreshPorts = new Button();
        btnConnect = new Button();
        btnDisconnect = new Button();
        lblConnectionStatus = new Label();
        grpProfile = new GroupBox();
        lblProfile = new Label();
        cmbTestParameters = new ComboBox();
        btnRefreshProfiles = new Button();
        btnStartTest = new Button();
        btnAbortTest = new Button();
        grpProgress = new GroupBox();
        progressBar = new ProgressBar();
        lblCurrentStep = new Label();
        lblMachineState = new Label();
        grpLog = new GroupBox();
        txtLog = new TextBox();
        grpConnection.SuspendLayout();
        grpProfile.SuspendLayout();
        grpProgress.SuspendLayout();
        grpLog.SuspendLayout();
        SuspendLayout();
        // 
        // grpConnection
        // 
        grpConnection.Controls.Add(lblPort);
        grpConnection.Controls.Add(cmbPort);
        grpConnection.Controls.Add(lblBaudRate);
        grpConnection.Controls.Add(cmbBaudRate);
        grpConnection.Controls.Add(btnRefreshPorts);
        grpConnection.Controls.Add(btnConnect);
        grpConnection.Controls.Add(btnDisconnect);
        grpConnection.Controls.Add(lblConnectionStatus);
        grpConnection.Location = new Point(3, 3);
        grpConnection.Name = "grpConnection";
        grpConnection.Size = new Size(770, 65);
        grpConnection.TabIndex = 0;
        grpConnection.TabStop = false;
        grpConnection.Text = "Conexión serie";
        // 
        // lblPort
        // 
        lblPort.AutoSize = true;
        lblPort.Location = new Point(8, 28);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(45, 15);
        lblPort.TabIndex = 0;
        lblPort.Text = "Puerto:";
        // 
        // cmbPort
        // 
        cmbPort.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPort.Location = new Point(62, 25);
        cmbPort.Name = "cmbPort";
        cmbPort.Size = new Size(130, 23);
        cmbPort.TabIndex = 1;
        // 
        // lblBaudRate
        // 
        lblBaudRate.AutoSize = true;
        lblBaudRate.Location = new Point(238, 28);
        lblBaudRate.Name = "lblBaudRate";
        lblBaudRate.Size = new Size(52, 15);
        lblBaudRate.TabIndex = 2;
        lblBaudRate.Text = "Baudios:";
        // 
        // cmbBaudRate
        // 
        cmbBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBaudRate.Location = new Point(298, 25);
        cmbBaudRate.Name = "cmbBaudRate";
        cmbBaudRate.Size = new Size(110, 23);
        cmbBaudRate.TabIndex = 3;
        // 
        // btnRefreshPorts
        // 
        btnRefreshPorts.Location = new Point(198, 24);
        btnRefreshPorts.Name = "btnRefreshPorts";
        btnRefreshPorts.Size = new Size(30, 25);
        btnRefreshPorts.TabIndex = 4;
        btnRefreshPorts.Text = "🔄";
        // 
        // btnConnect
        // 
        btnConnect.Location = new Point(425, 24);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(85, 25);
        btnConnect.TabIndex = 5;
        btnConnect.Text = "Conectar";
        // 
        // btnDisconnect
        // 
        btnDisconnect.Enabled = false;
        btnDisconnect.Location = new Point(516, 24);
        btnDisconnect.Name = "btnDisconnect";
        btnDisconnect.Size = new Size(95, 25);
        btnDisconnect.TabIndex = 6;
        btnDisconnect.Text = "Desconectar";
        // 
        // lblConnectionStatus
        // 
        lblConnectionStatus.AutoSize = true;
        lblConnectionStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblConnectionStatus.ForeColor = Color.Red;
        lblConnectionStatus.Location = new Point(620, 28);
        lblConnectionStatus.Name = "lblConnectionStatus";
        lblConnectionStatus.Size = new Size(91, 15);
        lblConnectionStatus.TabIndex = 7;
        lblConnectionStatus.Text = "○ Sin conexión";
        // 
        // grpProfile
        // 
        grpProfile.Controls.Add(lblProfile);
        grpProfile.Controls.Add(cmbTestParameters);
        grpProfile.Controls.Add(btnRefreshProfiles);
        grpProfile.Controls.Add(btnStartTest);
        grpProfile.Controls.Add(btnAbortTest);
        grpProfile.Location = new Point(3, 74);
        grpProfile.Name = "grpProfile";
        grpProfile.Size = new Size(770, 60);
        grpProfile.TabIndex = 1;
        grpProfile.TabStop = false;
        grpProfile.Text = "Perfil de test";
        // 
        // lblProfile
        // 
        lblProfile.AutoSize = true;
        lblProfile.Location = new Point(8, 22);
        lblProfile.Name = "lblProfile";
        lblProfile.Size = new Size(37, 15);
        lblProfile.TabIndex = 0;
        lblProfile.Text = "Perfil:";
        // 
        // cmbTestParameters
        // 
        cmbTestParameters.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTestParameters.Location = new Point(52, 19);
        cmbTestParameters.Name = "cmbTestParameters";
        cmbTestParameters.Size = new Size(380, 23);
        cmbTestParameters.TabIndex = 1;
        // 
        // btnRefreshProfiles
        // 
        btnRefreshProfiles.Location = new Point(438, 18);
        btnRefreshProfiles.Name = "btnRefreshProfiles";
        btnRefreshProfiles.Size = new Size(30, 25);
        btnRefreshProfiles.TabIndex = 2;
        btnRefreshProfiles.Text = "🔄";
        // 
        // btnStartTest
        // 
        btnStartTest.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnStartTest.Location = new Point(560, 18);
        btnStartTest.Name = "btnStartTest";
        btnStartTest.Size = new Size(100, 28);
        btnStartTest.TabIndex = 3;
        btnStartTest.Text = "▶ Iniciar test";
        // 
        // btnAbortTest
        // 
        btnAbortTest.Enabled = false;
        btnAbortTest.Location = new Point(666, 18);
        btnAbortTest.Name = "btnAbortTest";
        btnAbortTest.Size = new Size(94, 28);
        btnAbortTest.TabIndex = 4;
        btnAbortTest.Text = "⛔ Abortar";
        // 
        // grpProgress
        // 
        grpProgress.Controls.Add(progressBar);
        grpProgress.Controls.Add(lblCurrentStep);
        grpProgress.Controls.Add(lblMachineState);
        grpProgress.Location = new Point(3, 140);
        grpProgress.Name = "grpProgress";
        grpProgress.Size = new Size(770, 58);
        grpProgress.TabIndex = 2;
        grpProgress.TabStop = false;
        grpProgress.Text = "Progreso";
        // 
        // progressBar
        // 
        progressBar.Location = new Point(8, 22);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(480, 22);
        progressBar.TabIndex = 0;
        // 
        // lblCurrentStep
        // 
        lblCurrentStep.AutoSize = true;
        lblCurrentStep.Location = new Point(500, 26);
        lblCurrentStep.Name = "lblCurrentStep";
        lblCurrentStep.Size = new Size(19, 15);
        lblCurrentStep.TabIndex = 1;
        lblCurrentStep.Text = "—";
        // 
        // lblMachineState
        // 
        lblMachineState.AutoSize = true;
        lblMachineState.ForeColor = Color.Gray;
        lblMachineState.Location = new Point(8, 44);
        lblMachineState.Name = "lblMachineState";
        lblMachineState.Size = new Size(67, 15);
        lblMachineState.TabIndex = 2;
        lblMachineState.Text = "Estado: Idle";
        // 
        // grpLog
        // 
        grpLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpLog.Controls.Add(txtLog);
        grpLog.Location = new Point(3, 204);
        grpLog.Name = "grpLog";
        grpLog.Size = new Size(2038, 982);
        grpLog.TabIndex = 3;
        grpLog.TabStop = false;
        grpLog.Text = "Log de ejecución";
        // 
        // txtLog
        // 
        txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtLog.BackColor = Color.Black;
        txtLog.Font = new Font("Consolas", 9F);
        txtLog.ForeColor = Color.LightGreen;
        txtLog.Location = new Point(8, 20);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(2020, 951);
        txtLog.TabIndex = 0;
        // 
        // AutomaticTestPanel
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(grpConnection);
        Controls.Add(grpProfile);
        Controls.Add(grpProgress);
        Controls.Add(grpLog);
        Name = "AutomaticTestPanel";
        Size = new Size(1268, 639);
        grpConnection.ResumeLayout(false);
        grpConnection.PerformLayout();
        grpProfile.ResumeLayout(false);
        grpProfile.PerformLayout();
        grpProgress.ResumeLayout(false);
        grpProgress.PerformLayout();
        grpLog.ResumeLayout(false);
        grpLog.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.GroupBox   grpConnection;
    private System.Windows.Forms.Label      lblPort;
    private System.Windows.Forms.ComboBox   cmbPort;
    private System.Windows.Forms.Label      lblBaudRate;
    private System.Windows.Forms.ComboBox   cmbBaudRate;
    private System.Windows.Forms.Button     btnRefreshPorts;
    private System.Windows.Forms.Button     btnConnect;
    private System.Windows.Forms.Button     btnDisconnect;
    private System.Windows.Forms.Label      lblConnectionStatus;
    private System.Windows.Forms.GroupBox   grpProfile;
    private System.Windows.Forms.Label      lblProfile;
    private System.Windows.Forms.ComboBox   cmbTestParameters;
    private System.Windows.Forms.Button     btnRefreshProfiles;
    private System.Windows.Forms.Button     btnStartTest;
    private System.Windows.Forms.Button     btnAbortTest;
    private System.Windows.Forms.GroupBox   grpProgress;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Label      lblCurrentStep;
    private System.Windows.Forms.Label      lblMachineState;
    private System.Windows.Forms.GroupBox   grpLog;
    private System.Windows.Forms.TextBox    txtLog;
}

using PC7866.Views;

namespace PC7866
{
    public partial class Form1 : Form
    {
        private ManualControlPanel?   _manualPanel;
        private AutomaticTestPanel?   _automaticPanel;

        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            AttachMenuHandlers();
            // Arranca en modo manual por defecto
            ShowManualPanel();
        }

        private void AttachMenuHandlers()
        {
            mToolStripMenuItem.Click        += (_, _) => ShowManualPanel();
            automáticoToolStripMenuItem.Click += (_, _) => ShowAutomaticPanel();
            salirToolStripMenuItem.Click     += (_, _) => Application.Exit();
        }

        private void ShowManualPanel()
        {
            _automaticPanel?.Hide();

            if (_manualPanel is null)
            {
                _manualPanel = new ManualControlPanel { Dock = DockStyle.Fill };
                panelContent.Controls.Add(_manualPanel);
            }

            _manualPanel.Show();
            Text = "PC7866 – Modo Manual";
            mToolStripMenuItem.Checked        = true;
            automáticoToolStripMenuItem.Checked = false;
        }

        private void ShowAutomaticPanel()
        {
            _manualPanel?.Hide();

            if (_automaticPanel is null)
            {
                _automaticPanel = new AutomaticTestPanel { Dock = DockStyle.Fill };
                panelContent.Controls.Add(_automaticPanel);
            }

            _automaticPanel.Show();
            Text = "PC7866 – Modo Automático";
            mToolStripMenuItem.Checked          = false;
            automáticoToolStripMenuItem.Checked = true;
        }
    }
}

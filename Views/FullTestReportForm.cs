using PC7866.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Globalization;
using System.Text;

namespace PC7866.Views;

/// <summary>
/// Ventana modal que muestra los resultados del test completo manual,
/// con exportación a CSV y PDF.
/// </summary>
public sealed class FullTestReportForm : Form
{
    private readonly IReadOnlyList<FullTestRow> _rows;

    private DataGridView _grid = null!;
    private Button  _btnCsv     = null!;
    private Button  _btnPdf     = null!;
    private Button  _btnClose   = null!;
    private Label   _lblInfo    = null!;
    private TextBox _txtFileName = null!;
    private Label   _lblFileName = null!;

    public FullTestReportForm(IReadOnlyList<FullTestRow> rows)
    {
        _rows = rows;
        BuildUi();
        PopulateGrid();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────────────────────────────────

    private void BuildUi()
    {
        Text            = "Informe de test completo";
        Size            = new Size(1400, 680);
        StartPosition   = FormStartPosition.CenterParent;
        MinimumSize     = new Size(900, 450);
        FormBorderStyle = FormBorderStyle.Sizable;

        // Bottom bar
        var pnlBottom = new Panel
        {
            Dock   = DockStyle.Bottom,
            Height = 56,
            Padding= new Padding(8, 8, 8, 8),
            BackColor = Color.FromArgb(240, 242, 248)
        };

        _lblFileName = new Label
        {
            Text     = "Nombre de archivo:",
            AutoSize = true,
            Location = new Point(10, 18),
            Font     = new Font("Segoe UI", 9f)
        };

        string defaultName = "MAN_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _txtFileName = new TextBox
        {
            Text     = defaultName,
            Location = new Point(140, 14),
            Size     = new Size(280, 26),
            Font     = new Font("Segoe UI", 9.5f)
        };

        _btnCsv = new Button
        {
            Text      = "💾 Guardar CSV",
            Location  = new Point(434, 12),
            Size      = new Size(140, 32),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            BackColor = Color.FromArgb(0, 153, 76),
            ForeColor = Color.White
        };
        _btnCsv.Click += BtnCsv_Click;

        _btnPdf = new Button
        {
            Text      = "📄 Guardar PDF",
            Location  = new Point(584, 12),
            Size      = new Size(140, 32),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            BackColor = Color.FromArgb(0, 120, 215),
            ForeColor = Color.White
        };
        _btnPdf.Click += BtnPdf_Click;

        _btnClose = new Button
        {
            Text      = "Cerrar",
            Location  = new Point(740, 12),
            Size      = new Size(90, 32),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f),
            DialogResult = DialogResult.Cancel
        };

        _lblInfo = new Label
        {
            Text      = $"Test finalizado – {_rows.Count} salidas analizadas.",
            AutoSize  = true,
            Location  = new Point(850, 18),
            Font      = new Font("Segoe UI", 8.5f),
            ForeColor = Color.Gray
        };

        pnlBottom.Controls.AddRange(new Control[]
            { _lblFileName, _txtFileName, _btnCsv, _btnPdf, _btnClose, _lblInfo });

        // Grid
        _grid = new DataGridView
        {
            Dock                  = DockStyle.Fill,
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.AllCells,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect           = false,
            RowHeadersWidth       = 40,
            Font                  = new Font("Consolas", 8.5f),
            BackgroundColor       = Color.White,
            GridColor             = Color.LightSteelBlue,
            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                { BackColor = Color.FromArgb(242, 246, 255) }
        };

        // Column header style
        _grid.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 100, 180);
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _grid.EnableHeadersVisualStyles = false;

        Controls.Add(_grid);
        Controls.Add(pnlBottom);
    }

    private void PopulateGrid()
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_out",  HeaderText = "Nº Salida",      DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a1r",  HeaderText = "AIN1 Raw" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a1f",  HeaderText = "AIN1 Filt" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a2r",  HeaderText = "AIN2 Raw" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a2f",  HeaderText = "AIN2 Filt" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a3r",  HeaderText = "AIN3 Raw" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a3f",  HeaderText = "AIN3 Filt" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a4r",  HeaderText = "AIN4 Raw" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_a4f",  HeaderText = "AIN4 Filt" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_vain", HeaderText = "Vain (V)" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_ve",   HeaderText = "Ve (V)" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_r",    HeaderText = "R (Ω)",          DefaultCellStyle = { Font = new Font("Consolas", 8.5f, FontStyle.Bold) } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_err",  HeaderText = "Estado" });

        foreach (var row in _rows)
        {
            string rStr   = ManualControlPanel.FormatResistance(row.Resistance);
            string vainStr= row.Vain.ToString("F4", CultureInfo.InvariantCulture);
            string veStr  = row.Ve.ToString("F4", CultureInfo.InvariantCulture);
            string estado = string.IsNullOrEmpty(row.Error) ? "✅ OK" : $"⚠️ {row.Error}";

            int idx = _grid.Rows.Add(
                row.Output,
                row.Ain1Raw, row.Ain1Filt,
                row.Ain2Raw, row.Ain2Filt,
                row.Ain3Raw, row.Ain3Filt,
                row.Ain4Raw, row.Ain4Filt,
                vainStr, veStr, rStr, estado);

            // Colorear R errónea
            if (!string.IsNullOrEmpty(row.Error))
                _grid.Rows[idx].DefaultCellStyle.ForeColor = Color.DarkRed;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Exportación CSV
    // ─────────────────────────────────────────────────────────────────────────

    private void BtnCsv_Click(object? sender, EventArgs e)
    {
        string baseName = _txtFileName.Text.Trim();
        if (string.IsNullOrWhiteSpace(baseName)) baseName = "MAN_export";
        // Retirar extensión si la puso el usuario
        if (baseName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            baseName = baseName[..^4];

        using var dlg = new SaveFileDialog
        {
            Title            = "Guardar informe CSV",
            FileName         = baseName + ".csv",
            DefaultExt       = "csv",
            Filter           = "CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*",
            OverwritePrompt  = true
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        var sb = new StringBuilder();
        // Cabecera
        sb.AppendLine("Nº Salida;AIN1 Raw;AIN1 Filt;AIN2 Raw;AIN2 Filt;AIN3 Raw;AIN3 Filt;AIN4 Raw;AIN4 Filt;Vain (V);Ve (V);R (Ohm);Estado");
        foreach (var row in _rows)
        {
            string rStr    = ManualControlPanel.FormatResistance(row.Resistance);
            string estado  = string.IsNullOrEmpty(row.Error) ? "OK" : row.Error;
            sb.AppendLine(string.Join(";",
                row.Output,
                row.Ain1Raw, row.Ain1Filt,
                row.Ain2Raw, row.Ain2Filt,
                row.Ain3Raw, row.Ain3Filt,
                row.Ain4Raw, row.Ain4Filt,
                row.Vain.ToString("F4", CultureInfo.InvariantCulture),
                row.Ve.ToString("F4",   CultureInfo.InvariantCulture),
                rStr, estado));
        }
        File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
        MessageBox.Show($"CSV guardado:\n{dlg.FileName}", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Exportación PDF
    // ─────────────────────────────────────────────────────────────────────────

    private void BtnPdf_Click(object? sender, EventArgs e)
    {
        string baseName = _txtFileName.Text.Trim();
        if (string.IsNullOrWhiteSpace(baseName)) baseName = "MAN_export";
        if (baseName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            baseName = baseName[..^4];

        using var dlg = new SaveFileDialog
        {
            Title           = "Guardar informe PDF",
            FileName        = baseName + ".pdf",
            DefaultExt      = "pdf",
            Filter          = "PDF (*.pdf)|*.pdf|Todos los archivos (*.*)|*.*",
            OverwritePrompt = true
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            GeneratePdf(dlg.FileName);
            MessageBox.Show($"PDF guardado:\n{dlg.FileName}", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al generar PDF:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void GeneratePdf(string path)
    {
        var doc  = new PdfDocument();
        doc.Info.Title   = "Informe Test Completo PC7866";
        doc.Info.Author  = "PC7866";
        doc.Info.Subject = $"Test manual – {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

        // Fuentes
        var fontTitle  = new XFont("Segoe UI", 14, XFontStyle.Bold);
        var fontHeader = new XFont("Segoe UI",  7, XFontStyle.Bold);
        var fontCell   = new XFont("Courier New", 6.5, XFontStyle.Regular);
        var fontOk     = new XFont("Courier New", 6.5, XFontStyle.Regular);

        // Columnas: nº salida, A1R, A1F, A2R, A2F, A3R, A3F, A4R, A4F, Vain, Ve, R, Estado
        double[] colWidths = { 40, 36, 36, 36, 36, 36, 36, 36, 36, 46, 46, 46, 50 };
        string[] headers   = { "Nº Sal.", "A1 Raw", "A1 Filt", "A2 Raw", "A2 Filt", "A3 Raw", "A3 Filt", "A4 Raw", "A4 Filt", "Vain(V)", "Ve(V)", "R(Ω)", "Estado" };

        const double marginX    = 25;
        const double marginY    = 40;
        const double rowH       = 13;
        const double headerH    = 16;

        double pageH = XUnit.FromMillimeter(297).Point;
        double pageW = XUnit.FromMillimeter(420).Point;   // A3 apaisado

        PdfPage? page = null;
        XGraphics? gfx = null;
        double y = 0;

        void NewPage()
        {
            page = doc.AddPage();
            page.Width  = pageW;
            page.Height = pageH;
            gfx = XGraphics.FromPdfPage(page);
            y   = marginY;

            // Título
            gfx.DrawString("Informe Test Completo PC7866", fontTitle, XBrushes.DarkBlue,
                new XRect(marginX, y, pageW - marginX * 2, 22), XStringFormats.TopLeft);
            y += 24;

            gfx.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}   –   Salidas: {_rows.Count}",
                new XFont("Segoe UI", 8, XFontStyle.Italic), XBrushes.Gray,
                new XRect(marginX, y, pageW - marginX * 2, 14), XStringFormats.TopLeft);
            y += 18;

            // Cabecera tabla
            double x = marginX;
            for (int c = 0; c < headers.Length; c++)
            {
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(50, 100, 180)),
                    x, y, colWidths[c], headerH);
                gfx.DrawString(headers[c], fontHeader, XBrushes.White,
                    new XRect(x + 2, y + 2, colWidths[c] - 4, headerH - 2), XStringFormats.TopLeft);
                x += colWidths[c];
            }
            y += headerH;
        }

        NewPage();

        int rowIdx = 0;
        foreach (var row in _rows)
        {
            if (y + rowH > pageH - marginY) NewPage();

            double x = marginX;
            bool   isEven  = (rowIdx % 2 == 0);
            bool   hasError= !string.IsNullOrEmpty(row.Error);
            var    bgBrush = hasError
                ? new XSolidBrush(XColor.FromArgb(255, 220, 220))
                : isEven
                    ? new XSolidBrush(XColor.FromArgb(242, 246, 255))
                    : XBrushes.White;

            string rStr   = ManualControlPanel.FormatResistance(row.Resistance);
            string estado = string.IsNullOrEmpty(row.Error) ? "OK" : row.Error;
            string[] cells =
            {
                row.Output.ToString(),
                row.Ain1Raw.ToString(), row.Ain1Filt.ToString(),
                row.Ain2Raw.ToString(), row.Ain2Filt.ToString(),
                row.Ain3Raw.ToString(), row.Ain3Filt.ToString(),
                row.Ain4Raw.ToString(), row.Ain4Filt.ToString(),
                row.Vain.ToString("F4", CultureInfo.InvariantCulture),
                row.Ve.ToString("F4",   CultureInfo.InvariantCulture),
                rStr, estado
            };

            for (int c = 0; c < cells.Length; c++)
            {
                gfx!.DrawRectangle(bgBrush, x, y, colWidths[c], rowH);
                gfx.DrawRectangle(XPens.LightGray, x, y, colWidths[c], rowH);
                var cellFont = (c == 11 && hasError) ? new XFont("Courier New", 6.5, XFontStyle.Bold) : fontCell;
                var cellBrush= (c == 11 && hasError) ? XBrushes.DarkRed : XBrushes.Black;
                gfx.DrawString(cells[c], cellFont, cellBrush,
                    new XRect(x + 2, y + 2, colWidths[c] - 4, rowH - 2), XStringFormats.TopLeft);
                x += colWidths[c];
            }
            y += rowH;
            rowIdx++;
        }

        doc.Save(path);
    }
}

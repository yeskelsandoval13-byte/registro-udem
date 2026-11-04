using System;
using System.Linq;
using System.Windows.Forms;
using RegistroUdeM.Application.Services;
using RegistroUdeM.Domain;
using RegistroUdeM.Domain.Academico;
using RegistroUdeM.Domain.Personas;

namespace RegistroUdeM.WinForms;

/// <summary>
/// Ventana principal de Windows Forms para el registro universitario.
/// </summary>
public sealed class MainForm : Form
{
    private readonly RegistroService _registro;
    private readonly MatriculaService _matriculas;
    private readonly PagoService _pagos;
    private readonly BecaService _becas;
    private readonly ReporteService _reportes;

    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _dgvEstudiantes = CrearGrid();
    private readonly DataGridView _dgvCarreras = CrearGrid();
    private readonly DataGridView _dgvMatriculas = CrearGrid();
    private readonly DataGridView _dgvPagos = CrearGrid();
    private readonly TextBox _txtReportes = new() { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical, Font = new System.Drawing.Font("Consolas", 10), ReadOnly = true };

    public MainForm(RegistroService registro, MatriculaService matriculas, PagoService pagos, BecaService becas, ReporteService reportes)
    {
        _registro = registro;
        _matriculas = matriculas;
        _pagos = pagos;
        _becas = becas;
        _reportes = reportes;

        Text = "Registro UdeM - Universidad de Managua";
        Width = 1100;
        Height = 700;
        MinimumSize = new System.Drawing.Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;

        var menu = new MenuStrip();
        var archivo = new ToolStripMenuItem("Archivo");
        archivo.DropDownItems.Add("Salir", null, (_, _) => Close());
        menu.Items.Add(archivo);
        MainMenuStrip = menu;

        CrearPestana("Estudiantes", _dgvEstudiantes, new (string, EventHandler)[] { ("Nuevo estudiante", BtnNuevoEstudiante_Click), ("Nueva carrera", BtnNuevaCarrera_Click) });
        CrearPestana("Carreras", _dgvCarreras, new (string, EventHandler)[] { ("Nueva asignatura", BtnNuevaAsignatura_Click) });
        CrearPestana("Matrículas", _dgvMatriculas, new (string, EventHandler)[] { ("Inscribir", BtnInscribir_Click), ("Asignar nota", BtnNota_Click) });
        CrearPestana("Pagos", _dgvPagos, new (string, EventHandler)[] { ("Registrar pago", BtnPago_Click) });

        var tabReportes = new TabPage("Reportes");
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        var btn = new Button { Text = "Actualizar reportes", Dock = DockStyle.Top, Height = 34 };
        btn.Click += (_, _) => CargarReportes();
        panel.Controls.Add(_txtReportes);
        panel.Controls.Add(btn);
        tabReportes.Controls.Add(panel);
        _tabs.TabPages.Add(tabReportes);

        Controls.Add(_tabs);
        Controls.Add(menu);
        CargarTodo();
    }

    private static DataGridView CrearGrid() => new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AllowUserToAddRows = false,
        RowHeadersVisible = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = System.Drawing.Color.FromArgb(245, 247, 250) }
    };

    private void CrearPestana(string titulo, DataGridView dgv, (string, EventHandler)[] botones)
    {
        var tab = new TabPage(titulo);
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(4), FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
        foreach (var (texto, evento) in botones)
        {
            var boton = new Button { Text = texto, Width = 170, Height = 30, Margin = new Padding(4) };
            boton.Click += evento;
            barra.Controls.Add(boton);
        }
        panel.Controls.Add(dgv);
        panel.Controls.Add(barra);
        tab.Controls.Add(panel);
        _tabs.TabPages.Add(tab);
    }

    private void CargarTodo()
    {
        CargarEstudiantes();
        CargarCarreras();
        CargarMatriculas();
        CargarPagos();
        CargarReportes();
    }

    private void CargarEstudiantes()
    {
        _dgvEstudiantes.DataSource = _registro.ObtenerEstudiantes().Select(e => new
        {
            e.Id,
            Nombre = e.NombreCompleto,
            e.Carnet,
            Carrera = _registro.ObtenerCarreras().FirstOrDefault(c => c.Id == e.CarreraId)?.Nombre ?? "",
            e.Indice,
            e.Estado
        }).ToList();
    }

    private void CargarCarreras()
    {
        _dgvCarreras.DataSource = _registro.ObtenerCarreras().Select(c => new
        {
            c.Id,
            c.Codigo,
            c.Nombre,
            c.Facultad,
            c.CreditosTotales,
            Estudiantes = _registro.ObtenerEstudiantes().Count(e => e.CarreraId == c.Id)
        }).ToList();
    }

    private void CargarMatriculas()
    {
        _dgvMatriculas.DataSource = _matriculas.ObtenerMatriculas().Select(m => new
        {
            m.Id,
            Estudiante = _registro.ObtenerEstudiantes().FirstOrDefault(e => e.Id == m.EstudianteId)?.NombreCompleto ?? "",
            Asignatura = _registro.ObtenerAsignaturas().FirstOrDefault(a => a.Id == m.AsignaturaId)?.Nombre ?? "",
            m.Periodo,
            m.Nota,
            m.Estado
        }).ToList();
    }

    private void CargarPagos()
    {
        _dgvPagos.DataSource = _pagos.ObtenerPagos().Select(p => new
        {
            p.Id,
            Estudiante = _registro.ObtenerEstudiantes().FirstOrDefault(e => e.Id == p.EstudianteId)?.NombreCompleto ?? "",
            p.Concepto,
            p.Monto,
            p.Fecha,
            p.Pagado
        }).ToList();
    }

    private void CargarReportes()
    {
        var r = _reportes.ObtenerResumen();
        _txtReportes.Text =
            $"Estudiantes: {r.Estudiantes}\r\nDocentes: {r.Docentes}\r\nCarreras: {r.Carreras}\r\nAsignaturas: {r.Asignaturas}\r\nMatrículas: {r.Matriculas}\r\nPagos: {r.Pagos}\r\nBecas activas: {r.BecasActivas}\r\n\r\n" +
            "Estudiantes por carrera:\r\n" + string.Join("\r\n", _reportes.EstudiantesPorCarrera().Select(x => $"- {x.Carrera}: {x.Cantidad}")) +
            "\r\n\r\nMejores estudiantes:\r\n" + string.Join("\r\n", _reportes.MejoresEstudiantes().Select(x => $"- {x.Nombre} ({x.Carrera}) - Índice: {x.Indice:N2}"));
    }

    private void BtnNuevoEstudiante_Click(object? sender, EventArgs e)
    {
        using var form = new EstudianteForm(_registro.ObtenerCarreras());
        if (form.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _registro.RegistrarEstudiante(form.Nombre, form.Apellido, form.Documento, form.Telefono, form.Email, form.CarreraId, form.FechaIngreso);
            CargarEstudiantes();
            MessageBox.Show("Estudiante registrado.", "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void BtnNuevaCarrera_Click(object? sender, EventArgs e)
    {
        using var form = new CarreraForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _registro.RegistrarCarrera(form.Codigo, form.Nombre, form.Facultad, form.Creditos);
            CargarCarreras();
            MessageBox.Show("Carrera registrada.", "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void BtnNuevaAsignatura_Click(object? sender, EventArgs e)
    {
        using var form = new AsignaturaForm(_registro.ObtenerCarreras());
        if (form.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _registro.RegistrarAsignatura(form.Codigo, form.Nombre, form.Creditos, form.CarreraId);
            CargarCarreras();
            MessageBox.Show("Asignatura registrada.", "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void BtnInscribir_Click(object? sender, EventArgs e)
    {
        using var form = new InscripcionForm(_registro.ObtenerEstudiantes(), _registro.ObtenerAsignaturas());
        if (form.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _matriculas.Inscribir(form.EstudianteId, form.AsignaturaId, form.Periodo);
            CargarMatriculas();
            MessageBox.Show("Matrícula creada.", "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void BtnNota_Click(object? sender, EventArgs e)
    {
        if (_dgvMatriculas.CurrentRow is null) { MessageBox.Show("Seleccione una matrícula.", "Registro UdeM"); return; }
        var id = (Guid)_dgvMatriculas.CurrentRow.Cells["Id"].Value;
        var texto = PedirTexto("Nota (0-100):", "Asignar nota", "80");
        if (string.IsNullOrWhiteSpace(texto) || !decimal.TryParse(texto, out var nota)) return;
        try
        {
            _matriculas.AsignarNota(id, nota);
            CargarMatriculas();
            CargarEstudiantes();
            MessageBox.Show("Nota asignada.", "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private static string PedirTexto(string etiqueta, string titulo, string valorInicial)
    {
        using var form = new Form { Text = titulo, Width = 320, Height = 150, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent };
        var label = new Label { Text = etiqueta, Left = 12, Top = 12, AutoSize = true };
        var textBox = new TextBox { Left = 12, Top = 34, Width = 280, Text = valorInicial };
        var aceptar = new Button { Text = "Aceptar", Left = 12, Top = 70, Width = 90, DialogResult = DialogResult.OK };
        var cancelar = new Button { Text = "Cancelar", Left = 112, Top = 70, Width = 90, DialogResult = DialogResult.Cancel };
        form.Controls.AddRange(new Control[] { label, textBox, aceptar, cancelar });
        form.AcceptButton = aceptar;
        form.CancelButton = cancelar;
        return form.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
    }

    private void BtnPago_Click(object? sender, EventArgs e)
    {
        using var form = new PagoForm(_registro.ObtenerEstudiantes());
        if (form.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _pagos.RegistrarPago(form.EstudianteId, form.Monto, form.Concepto, form.Referencia);
            CargarPagos();
            MessageBox.Show("Pago registrado.", "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Registro UdeM", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using RegistroUdeM.Domain.Academico;
using RegistroUdeM.Domain.Financiero;
using RegistroUdeM.Domain.Personas;

namespace RegistroUdeM.WinForms;

public sealed class EstudianteForm : Form
{
    private readonly ComboBox _cmbCarrera = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtNombre = new() { Width = 250 };
    private readonly TextBox _txtApellido = new() { Width = 250 };
    private readonly TextBox _txtDocumento = new() { Width = 250 };
    private readonly TextBox _txtTelefono = new() { Width = 250 };
    private readonly TextBox _txtEmail = new() { Width = 250 };
    private readonly DateTimePicker _dtpIngreso = new() { Width = 250, Format = DateTimePickerFormat.Short };

    public string Nombre => _txtNombre.Text;
    public string Apellido => _txtApellido.Text;
    public string Documento => _txtDocumento.Text;
    public string Telefono => _txtTelefono.Text;
    public string Email => _txtEmail.Text;
    public Guid CarreraId => (Guid)_cmbCarrera.SelectedValue!;
    public DateTime FechaIngreso => _dtpIngreso.Value;

    public EstudianteForm(IEnumerable<Carrera> carreras)
    {
        Text = "Nuevo estudiante";
        Width = 420; Height = 360; FormBorderStyle = FormBorderStyle.FixedDialog; StartPosition = FormStartPosition.CenterParent;
        _cmbCarrera.DataSource = carreras.ToList(); _cmbCarrera.DisplayMember = "Nombre"; _cmbCarrera.ValueMember = "Id";
        _dtpIngreso.Value = DateTime.Today;
        var layout = CrearLayout();
        Agregar(layout, "Nombre", _txtNombre);
        Agregar(layout, "Apellido", _txtApellido);
        Agregar(layout, "Documento", _txtDocumento);
        Agregar(layout, "Teléfono", _txtTelefono);
        Agregar(layout, "Correo", _txtEmail);
        Agregar(layout, "Carrera", _cmbCarrera);
        Agregar(layout, "Ingreso", _dtpIngreso);
        Botones(layout);
        Controls.Add(layout);
    }

    private static TableLayoutPanel CrearLayout() => new() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 0, Padding = new Padding(12), ColumnStyles = { new ColumnStyle(SizeType.Absolute, 110), new ColumnStyle(SizeType.Absolute, 260) }, AutoSize = true };

    private static void Agregar(TableLayoutPanel layout, string etiqueta, Control control)
    {
        var fila = layout.RowCount;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3) }, 0, fila);
        layout.Controls.Add(control, 1, fila);
        layout.RowCount++;
    }

    private void Botones(TableLayoutPanel layout)
    {
        var barra = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var ok = new Button { Text = "Aceptar", Width = 100, Height = 30, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Cancelar", Width = 100, Height = 30, DialogResult = DialogResult.Cancel };
        barra.Controls.Add(ok); barra.Controls.Add(cancel);
        Controls.Add(barra);
    }
}

public sealed class CarreraForm : Form
{
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string Facultad { get; private set; } = string.Empty;
    public int Creditos { get; private set; } = 180;

    private readonly TextBox _txtCodigo = new() { Width = 250 };
    private readonly TextBox _txtNombre = new() { Width = 250 };
    private readonly TextBox _txtFacultad = new() { Width = 250 };
    private readonly NumericUpDown _numCreditos = new() { Width = 250, Minimum = 1, Maximum = 300, Value = 180 };

    public CarreraForm()
    {
        Text = "Nueva carrera";
        Width = 420; Height = 260; FormBorderStyle = FormBorderStyle.FixedDialog; StartPosition = FormStartPosition.CenterParent;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 0, Padding = new Padding(12), ColumnStyles = { new ColumnStyle(SizeType.Absolute, 110), new ColumnStyle(SizeType.Absolute, 260) }, AutoSize = true };
        Agregar(layout, "Código", _txtCodigo);
        Agregar(layout, "Nombre", _txtNombre);
        Agregar(layout, "Facultad", _txtFacultad);
        Agregar(layout, "Créditos", _numCreditos);
        var barra = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var ok = new Button { Text = "Aceptar", Width = 100, Height = 30, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Cancelar", Width = 100, Height = 30, DialogResult = DialogResult.Cancel };
        ok.Click += (_, _) => { Codigo = _txtCodigo.Text; Nombre = _txtNombre.Text; Facultad = _txtFacultad.Text; Creditos = (int)_numCreditos.Value; };
        barra.Controls.Add(ok); barra.Controls.Add(cancel);
        Controls.Add(layout); Controls.Add(barra);
    }

    private static void Agregar(TableLayoutPanel layout, string etiqueta, Control control)
    {
        var fila = layout.RowCount;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3) }, 0, fila);
        layout.Controls.Add(control, 1, fila);
        layout.RowCount++;
    }
}

public sealed class AsignaturaForm : Form
{
    private readonly ComboBox _cmbCarrera = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtCodigo = new() { Width = 250 };
    private readonly TextBox _txtNombre = new() { Width = 250 };
    private readonly NumericUpDown _numCreditos = new() { Width = 250, Minimum = 1, Maximum = 12, Value = 4 };

    public Guid CarreraId => (Guid)_cmbCarrera.SelectedValue!;
    public string Codigo => _txtCodigo.Text;
    public string Nombre => _txtNombre.Text;
    public int Creditos => (int)_numCreditos.Value;

    public AsignaturaForm(IEnumerable<Carrera> carreras)
    {
        Text = "Nueva asignatura";
        Width = 420; Height = 250; FormBorderStyle = FormBorderStyle.FixedDialog; StartPosition = FormStartPosition.CenterParent;
        _cmbCarrera.DataSource = carreras.ToList(); _cmbCarrera.DisplayMember = "Nombre"; _cmbCarrera.ValueMember = "Id";
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 0, Padding = new Padding(12), ColumnStyles = { new ColumnStyle(SizeType.Absolute, 110), new ColumnStyle(SizeType.Absolute, 260) }, AutoSize = true };
        Agregar(layout, "Carrera", _cmbCarrera);
        Agregar(layout, "Código", _txtCodigo);
        Agregar(layout, "Nombre", _txtNombre);
        Agregar(layout, "Créditos", _numCreditos);
        var barra = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var ok = new Button { Text = "Aceptar", Width = 100, Height = 30, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Cancelar", Width = 100, Height = 30, DialogResult = DialogResult.Cancel };
        barra.Controls.Add(ok); barra.Controls.Add(cancel);
        Controls.Add(layout); Controls.Add(barra);
    }

    private static void Agregar(TableLayoutPanel layout, string etiqueta, Control control)
    {
        var fila = layout.RowCount;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3) }, 0, fila);
        layout.Controls.Add(control, 1, fila);
        layout.RowCount++;
    }
}

public sealed class InscripcionForm : Form
{
    private readonly ComboBox _cmbEstudiante = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbAsignatura = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtPeriodo = new() { Width = 250, Text = "2026-1" };

    public Guid EstudianteId => (Guid)_cmbEstudiante.SelectedValue!;
    public Guid AsignaturaId => (Guid)_cmbAsignatura.SelectedValue!;
    public string Periodo => _txtPeriodo.Text;

    public InscripcionForm(IEnumerable<Estudiante> estudiantes, IEnumerable<Asignatura> asignaturas)
    {
        Text = "Inscribir matrícula";
        Width = 420; Height = 220; FormBorderStyle = FormBorderStyle.FixedDialog; StartPosition = FormStartPosition.CenterParent;
        _cmbEstudiante.DataSource = estudiantes.ToList(); _cmbEstudiante.DisplayMember = "NombreCompleto"; _cmbEstudiante.ValueMember = "Id";
        _cmbAsignatura.DataSource = asignaturas.ToList(); _cmbAsignatura.DisplayMember = "Nombre"; _cmbAsignatura.ValueMember = "Id";
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 0, Padding = new Padding(12), ColumnStyles = { new ColumnStyle(SizeType.Absolute, 110), new ColumnStyle(SizeType.Absolute, 260) }, AutoSize = true };
        Agregar(layout, "Estudiante", _cmbEstudiante);
        Agregar(layout, "Asignatura", _cmbAsignatura);
        Agregar(layout, "Período", _txtPeriodo);
        var barra = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var ok = new Button { Text = "Aceptar", Width = 100, Height = 30, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Cancelar", Width = 100, Height = 30, DialogResult = DialogResult.Cancel };
        barra.Controls.Add(ok); barra.Controls.Add(cancel);
        Controls.Add(layout); Controls.Add(barra);
    }

    private static void Agregar(TableLayoutPanel layout, string etiqueta, Control control)
    {
        var fila = layout.RowCount;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3) }, 0, fila);
        layout.Controls.Add(control, 1, fila);
        layout.RowCount++;
    }
}

public sealed class PagoForm : Form
{
    private readonly ComboBox _cmbEstudiante = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _numMonto = new() { Width = 250, Minimum = 1, Maximum = 100000, DecimalPlaces = 2, Value = 100 };
    private readonly ComboBox _cmbConcepto = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtReferencia = new() { Width = 250 };

    public Guid EstudianteId => (Guid)_cmbEstudiante.SelectedValue!;
    public decimal Monto => _numMonto.Value;
    public ConceptoPago Concepto => Enum.Parse<ConceptoPago>(_cmbConcepto.Text);
    public string Referencia => _txtReferencia.Text;

    public PagoForm(IEnumerable<Estudiante> estudiantes)
    {
        Text = "Registrar pago";
        Width = 420; Height = 250; FormBorderStyle = FormBorderStyle.FixedDialog; StartPosition = FormStartPosition.CenterParent;
        _cmbEstudiante.DataSource = estudiantes.ToList(); _cmbEstudiante.DisplayMember = "NombreCompleto"; _cmbEstudiante.ValueMember = "Id";
        _cmbConcepto.Items.AddRange(Enum.GetNames(typeof(ConceptoPago)));
        _cmbConcepto.SelectedIndex = 1;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 0, Padding = new Padding(12), ColumnStyles = { new ColumnStyle(SizeType.Absolute, 110), new ColumnStyle(SizeType.Absolute, 260) }, AutoSize = true };
        Agregar(layout, "Estudiante", _cmbEstudiante);
        Agregar(layout, "Monto", _numMonto);
        Agregar(layout, "Concepto", _cmbConcepto);
        Agregar(layout, "Referencia", _txtReferencia);
        var barra = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var ok = new Button { Text = "Aceptar", Width = 100, Height = 30, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Cancelar", Width = 100, Height = 30, DialogResult = DialogResult.Cancel };
        barra.Controls.Add(ok); barra.Controls.Add(cancel);
        Controls.Add(layout); Controls.Add(barra);
    }

    private static void Agregar(TableLayoutPanel layout, string etiqueta, Control control)
    {
        var fila = layout.RowCount;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3) }, 0, fila);
        layout.Controls.Add(control, 1, fila);
        layout.RowCount++;
    }
}

using System;
using System.IO;
using System.Windows.Forms;
using RegistroUdeM.Application.Services;
using RegistroUdeM.Infrastructure.Persistencia;

namespace RegistroUdeM.WinForms;

internal static class Program
{
    private static string ObtenerCadenaConexion()
    {
        var variable = Environment.GetEnvironmentVariable("REGISTROUDEM_SQL");
        if (!string.IsNullOrWhiteSpace(variable)) return variable;

        var ruta = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(ruta))
        {
            using var documento = System.Text.Json.JsonDocument.Parse(File.ReadAllText(ruta));
            if (documento.RootElement.TryGetProperty("ConnectionStrings", out var conexiones) &&
                conexiones.TryGetProperty("RegistroUdeM", out var valor) &&
                valor.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                return valor.GetString() ?? "Server=localhost;Database=RegistroUdeM;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
            }
        }

        return "Server=localhost;Database=RegistroUdeM;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
    }

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var cadena = ObtenerCadenaConexion();
        var almacen = new SqlAlmacenRegistro(cadena);
        var registro = new RegistroService(almacen);
        var matriculas = new MatriculaService(almacen);
        var pagos = new PagoService(almacen);
        var becas = new BecaService(almacen);
        var reportes = new ReporteService(almacen);
        System.Windows.Forms.Application.Run(new MainForm(registro, matriculas, pagos, becas, reportes));
    }
}

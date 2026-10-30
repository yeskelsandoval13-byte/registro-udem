using System;
using System.IO;
using System.Windows.Forms;
using RegistroUdeM.Application.Services;
using RegistroUdeM.Infrastructure.Persistencia;

namespace RegistroUdeM.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var cadena = "Server=localhost;Database=RegistroUdeM;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
        var almacen = new SqlAlmacenRegistro(cadena);
        var registro = new RegistroService(almacen);
        var matriculas = new MatriculaService(almacen);
        var pagos = new PagoService(almacen);
        var becas = new BecaService(almacen);
        var reportes = new ReporteService(almacen);
        System.Windows.Forms.Application.Run(new MainForm(registro, matriculas, pagos, becas, reportes));
    }
}

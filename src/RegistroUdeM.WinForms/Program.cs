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
        var ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RegistroUdeM", "registro.json");
        var almacen = new JsonAlmacenRegistro(ruta);
        var registro = new RegistroService(almacen);
        var matriculas = new MatriculaService(almacen);
        var pagos = new PagoService(almacen);
        var becas = new BecaService(almacen);
        var reportes = new ReporteService(almacen);
        System.Windows.Forms.Application.Run(new MainForm(registro, matriculas, pagos, becas, reportes));
    }
}

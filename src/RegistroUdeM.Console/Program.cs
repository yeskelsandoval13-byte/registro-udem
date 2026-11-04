using RegistroUdeM.Application.Services;
using RegistroUdeM.Application.Services;
using RegistroUdeM.Laboratorio.Clase03;

namespace RegistroUdeM.AppConsole;

/// <summary>
/// Punto de entrada de la consola. Permite operar el registro y ejecutar la demo.
/// </summary>
internal class Program
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

    private static void Main(string[] args)
    {
        var cadena = ObtenerCadenaConexion();
        var almacen = new RegistroUdeM.Infrastructure.Persistencia.SqlAlmacenRegistro(cadena);
        var registro = new RegistroService(almacen);
        var matriculas = new MatriculaService(almacen);
        var pagos = new PagoService(almacen);
        var becas = new BecaService(almacen);
        var reportes = new ReporteService(almacen);

        if (args.Contains("--demo"))
        {
            DemostracionClase03.Herencia();
            DemostracionClase03.Polimorfismo();
            DemostracionClase03.Sobrecarga();
            Console.WriteLine();
            var resumen = reportes.ObtenerResumen();
            Console.WriteLine($"Estudiantes: {resumen.Estudiantes}");
            Console.WriteLine($"Carreras: {resumen.Carreras}");
            Console.WriteLine($"Matrículas: {resumen.Matriculas}");
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("REGISTRO UDEM - UNIVERSIDAD DE MANAGUA");
            Console.WriteLine("1. Ver estudiantes");
            Console.WriteLine("2. Registrar estudiante");
            Console.WriteLine("3. Ver carreras");
            Console.WriteLine("4. Registrar carrera");
            Console.WriteLine("5. Ver matrículas");
            Console.WriteLine("6. Inscribir matrícula");
            Console.WriteLine("7. Asignar nota");
            Console.WriteLine("8. Ver reportes");
            Console.WriteLine("9. Laboratorio Clase 03");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            try
            {
                switch (opcion)
                {
                    case "1":
                        foreach (var est in registro.ObtenerEstudiantes())
                            Console.WriteLine($"{est.Carnet} - {est.NombreCompleto} - Índice: {est.Indice:N2} - {est.Estado}");
                        break;
                    case "2":
                        Console.Write("Nombre: "); var nombre = Console.ReadLine() ?? "";
                        Console.Write("Apellido: "); var apellido = Console.ReadLine() ?? "";
                        Console.Write("Documento: "); var documento = Console.ReadLine() ?? "";
                        Console.Write("Teléfono: "); var telefono = Console.ReadLine() ?? "";
                        Console.Write("Correo: "); var correo = Console.ReadLine() ?? "";
                        Console.Write("Carrera (código): "); var codigo = Console.ReadLine() ?? "";
                        var carrera = registro.ObtenerCarreras().FirstOrDefault(c => c.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase));
                        if (carrera is null) { Console.WriteLine("Carrera no encontrada."); break; }
                        var estudiante = registro.RegistrarEstudiante(nombre, apellido, documento, telefono, correo, carrera.Id, DateTime.Today);
                        Console.WriteLine($"Estudiante registrado: {estudiante.NombreCompleto} ({estudiante.Carnet})");
                        break;
                    case "3":
                        foreach (var car in registro.ObtenerCarreras())
                            Console.WriteLine($"{car.Codigo} - {car.Nombre} - {car.Facultad} - {car.CreditosTotales} créditos");
                        break;
                    case "4":
                        Console.Write("Código: "); var cod = Console.ReadLine() ?? "";
                        Console.Write("Nombre: "); var nom = Console.ReadLine() ?? "";
                        Console.Write("Facultad: "); var fac = Console.ReadLine() ?? "";
                        Console.Write("Créditos: "); var cred = int.TryParse(Console.ReadLine(), out var c) ? c : 180;
                        var nueva = registro.RegistrarCarrera(cod, nom, fac, cred);
                        Console.WriteLine($"Carrera registrada: {nueva.Nombre}");
                        break;
                    case "5":
                        foreach (var mat in matriculas.ObtenerMatriculas())
                        {
                            var est = registro.ObtenerEstudiante(mat.EstudianteId);
                            var asig = registro.ObtenerAsignatura(mat.AsignaturaId);
                            Console.WriteLine($"{est?.NombreCompleto} - {asig?.Nombre} - {mat.Periodo} - Nota: {mat.Nota:N2} - {mat.Estado}");
                        }
                        break;
                    case "6":
                        Console.Write("ID estudiante: "); var estId = Guid.TryParse(Console.ReadLine(), out var e) ? e : Guid.Empty;
                        Console.Write("ID asignatura: "); var asigId = Guid.TryParse(Console.ReadLine(), out var a) ? a : Guid.Empty;
                        Console.Write("Período: "); var periodo = Console.ReadLine() ?? "2026-1";
                        var matricula = matriculas.Inscribir(estId, asigId, periodo);
                        Console.WriteLine($"Matrícula creada: {matricula.Id}");
                        break;
                    case "7":
                        Console.Write("ID matrícula: "); var matId = Guid.TryParse(Console.ReadLine(), out var m) ? m : Guid.Empty;
                        Console.Write("Nota: "); var nota = decimal.TryParse(Console.ReadLine(), out var n) ? n : 0m;
                        matriculas.AsignarNota(matId, nota);
                        Console.WriteLine("Nota asignada.");
                        break;
                    case "8":
                        var resumen = reportes.ObtenerResumen();
                        Console.WriteLine($"Estudiantes: {resumen.Estudiantes}");
                        Console.WriteLine($"Docentes: {resumen.Docentes}");
                        Console.WriteLine($"Carreras: {resumen.Carreras}");
                        Console.WriteLine($"Asignaturas: {resumen.Asignaturas}");
                        Console.WriteLine($"Matrículas: {resumen.Matriculas}");
                        Console.WriteLine($"Pagos: {resumen.Pagos}");
                        Console.WriteLine($"Becas activas: {resumen.BecasActivas}");
                        Console.WriteLine("Mejores estudiantes:");
                        foreach (var mejor in reportes.MejoresEstudiantes())
                            Console.WriteLine($"- {mejor.Nombre} ({mejor.Carrera}) - Índice: {mejor.Indice:N2}");
                        break;
                    case "9":
                        DemostracionClase03.Herencia();
                        DemostracionClase03.Polimorfismo();
                        DemostracionClase03.Sobrecarga();
                        break;
                    case "0":
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Presione Enter para continuar...");
            Console.ReadLine();
        }
    }
}


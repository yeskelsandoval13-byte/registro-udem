using System.Data;
using RegistroUdeM.Application.Abstractions;
using RegistroUdeM.Domain;
using RegistroUdeM.Domain.Academico;
using RegistroUdeM.Domain.Financiero;
using RegistroUdeM.Domain.Personas;
using Microsoft.Data.SqlClient;

namespace RegistroUdeM.Infrastructure.Persistencia;

/// <summary>
/// Almacén que carga y guarda el registro universitario en SQL Server.
/// </summary>
public sealed class SqlAlmacenRegistro : IAlmacenRegistro
{
    private readonly string _cadenaConexion;
    public RegistroData Datos { get; private set; }

    public SqlAlmacenRegistro(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
        Datos = new RegistroData();
        Cargar();
        if (Datos.Carreras.Count == 0 && Datos.Estudiantes.Count == 0 && Datos.Docentes.Count == 0)
        {
            Datos = DatosDemo.Crear();
            Guardar();
        }
    }

    public void Guardar() => Guardar(Datos);

    public void Restablecer(RegistroData datos)
    {
        Datos = datos;
        Guardar(datos);
    }

    private void Guardar(RegistroData datos)
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();
        try
        {
            foreach (var carrera in datos.Carreras) UpsertCarrera(conexion, transaccion, carrera);
            foreach (var asignatura in datos.Asignaturas) UpsertAsignatura(conexion, transaccion, asignatura);

            foreach (var estudiante in datos.Estudiantes)
            {
                UpsertPersona(conexion, transaccion, estudiante, "E");
                UpsertEstudiante(conexion, transaccion, estudiante);
            }
            foreach (var docente in datos.Docentes)
            {
                UpsertPersona(conexion, transaccion, docente, "D");
                UpsertDocente(conexion, transaccion, docente);
            }
            foreach (var administrativo in datos.Administrativos)
            {
                UpsertPersona(conexion, transaccion, administrativo, "A");
                UpsertAdministrativo(conexion, transaccion, administrativo);
            }

            foreach (var matricula in datos.Matriculas) UpsertMatricula(conexion, transaccion, matricula);
            foreach (var pago in datos.Pagos) UpsertPago(conexion, transaccion, pago);
            foreach (var beca in datos.Becas) UpsertBeca(conexion, transaccion, beca);

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private void Cargar()
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();

        Datos = new RegistroData
        {
            Carreras = CargarCarreras(conexion),
            Asignaturas = CargarAsignaturas(conexion),
            Estudiantes = CargarEstudiantes(conexion),
            Docentes = CargarDocentes(conexion),
            Administrativos = CargarAdministrativos(conexion),
            Matriculas = CargarMatriculas(conexion),
            Pagos = CargarPagos(conexion),
            Becas = CargarBecas(conexion)
        };
    }

    private static List<Carrera> CargarCarreras(SqlConnection conexion)
    {
        var lista = new List<Carrera>();
        using var comando = new SqlCommand("SELECT CarreraId, Codigo, Nombre, Facultad, CreditosTotales, Activa FROM Carreras;", conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Carrera
            {
                Id = lector.GetGuid(0),
                Codigo = lector.GetString(1),
                Nombre = lector.GetString(2),
                Facultad = lector.GetString(3),
                CreditosTotales = lector.GetInt32(4),
                Activa = lector.GetBoolean(5)
            });
        }
        return lista;
    }

    private static List<Asignatura> CargarAsignaturas(SqlConnection conexion)
    {
        var lista = new List<Asignatura>();
        using var comando = new SqlCommand("SELECT AsignaturaId, Codigo, Nombre, Creditos, CarreraId, Activa FROM Asignaturas;", conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Asignatura
            {
                Id = lector.GetGuid(0),
                Codigo = lector.GetString(1),
                Nombre = lector.GetString(2),
                Creditos = lector.GetInt32(3),
                CarreraId = lector.GetGuid(4),
                Activa = lector.GetBoolean(5)
            });
        }
        return lista;
    }

    private static List<Estudiante> CargarEstudiantes(SqlConnection conexion)
    {
        var lista = new List<Estudiante>();
        const string sql = @"
            SELECT p.PersonaId, p.Nombre, p.Apellido, p.Documento, p.Telefono, p.Email,
                   e.CarreraId, e.Carnet, e.FechaIngreso, e.Estado, e.Indice
            FROM Personas p
            INNER JOIN Estudiantes e ON e.EstudianteId = p.PersonaId
            WHERE p.TipoPersona = 'E';";
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Estudiante
            {
                Id = lector.GetGuid(0),
                Nombre = lector.GetString(1),
                Apellido = lector.GetString(2),
                Documento = lector.GetString(3),
                Telefono = lector.GetString(4),
                Email = lector.IsDBNull(5) ? string.Empty : lector.GetString(5),
                CarreraId = lector.GetGuid(6),
                Carnet = lector.IsDBNull(7) ? string.Empty : lector.GetString(7),
                FechaIngreso = lector.GetDateTime(8).Date,
                Estado = Enum.TryParse<EstadoEstudiante>(lector.GetString(9), true, out var estado) ? estado : EstadoEstudiante.Activo,
                Indice = lector.GetDecimal(10)
            });
        }
        return lista;
    }

    private static List<Docente> CargarDocentes(SqlConnection conexion)
    {
        var lista = new List<Docente>();
        const string sql = @"
            SELECT p.PersonaId, p.Nombre, p.Apellido, p.Documento, p.Telefono, p.Email,
                   d.Especialidad, d.GradoAcademico, d.Activo
            FROM Personas p
            INNER JOIN Docentes d ON d.DocenteId = p.PersonaId
            WHERE p.TipoPersona = 'D';";
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Docente
            {
                Id = lector.GetGuid(0),
                Nombre = lector.GetString(1),
                Apellido = lector.GetString(2),
                Documento = lector.GetString(3),
                Telefono = lector.GetString(4),
                Email = lector.IsDBNull(5) ? string.Empty : lector.GetString(5),
                Especialidad = lector.GetString(6),
                GradoAcademico = lector.GetString(7),
                Activo = lector.GetBoolean(8)
            });
        }
        return lista;
    }

    private static List<Administrativo> CargarAdministrativos(SqlConnection conexion)
    {
        var lista = new List<Administrativo>();
        const string sql = @"
            SELECT p.PersonaId, p.Nombre, p.Apellido, p.Documento, p.Telefono, p.Email,
                   a.Cargo, a.Departamento
            FROM Personas p
            INNER JOIN Administrativos a ON a.AdministrativoId = p.PersonaId
            WHERE p.TipoPersona = 'A';";
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Administrativo
            {
                Id = lector.GetGuid(0),
                Nombre = lector.GetString(1),
                Apellido = lector.GetString(2),
                Documento = lector.GetString(3),
                Telefono = lector.GetString(4),
                Email = lector.IsDBNull(5) ? string.Empty : lector.GetString(5),
                Cargo = lector.GetString(6),
                Departamento = lector.GetString(7)
            });
        }
        return lista;
    }

    private static List<Matricula> CargarMatriculas(SqlConnection conexion)
    {
        var lista = new List<Matricula>();
        const string sql = "SELECT MatriculaId, EstudianteId, AsignaturaId, Periodo, Nota, Estado, Fecha FROM Matriculas;";
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Matricula
            {
                Id = lector.GetGuid(0),
                EstudianteId = lector.GetGuid(1),
                AsignaturaId = lector.GetGuid(2),
                Periodo = lector.GetString(3),
                Nota = lector.GetDecimal(4),
                Estado = Enum.TryParse<EstadoMatricula>(lector.GetString(5), true, out var estado) ? estado : EstadoMatricula.Inscrita,
                Fecha = lector.GetDateTime(6)
            });
        }
        return lista;
    }

    private static List<Pago> CargarPagos(SqlConnection conexion)
    {
        var lista = new List<Pago>();
        const string sql = "SELECT PagoId, EstudianteId, Monto, Concepto, Fecha, Pagado, Referencia FROM Pagos;";
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Pago
            {
                Id = lector.GetGuid(0),
                EstudianteId = lector.GetGuid(1),
                Monto = lector.GetDecimal(2),
                Concepto = Enum.TryParse<ConceptoPago>(lector.GetString(3), true, out var concepto) ? concepto : ConceptoPago.Otro,
                Fecha = lector.GetDateTime(4),
                Pagado = lector.GetBoolean(5),
                Referencia = lector.IsDBNull(6) ? string.Empty : lector.GetString(6)
            });
        }
        return lista;
    }

    private static List<Beca> CargarBecas(SqlConnection conexion)
    {
        var lista = new List<Beca>();
        const string sql = "SELECT BecaId, EstudianteId, Tipo, Porcentaje, Activa, Observacion FROM Becas;";
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Beca
            {
                Id = lector.GetGuid(0),
                EstudianteId = lector.GetGuid(1),
                Tipo = Enum.TryParse<TipoBeca>(lector.GetString(2), true, out var tipo) ? tipo : TipoBeca.Academica,
                Porcentaje = lector.GetDecimal(3),
                Activa = lector.GetBoolean(4),
                Observacion = lector.IsDBNull(5) ? string.Empty : lector.GetString(5)
            });
        }
        return lista;
    }

    private static void UpsertPersona(SqlConnection conexion, SqlTransaction transaccion, Persona persona, string tipo)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Personas WHERE PersonaId = @Id)
                UPDATE Personas SET Nombre=@Nombre, Apellido=@Apellido, Documento=@Documento, Telefono=@Telefono, Email=@Email, TipoPersona=@Tipo, CreadoEn=CreadoEn WHERE PersonaId=@Id;
            ELSE
                INSERT INTO Personas (PersonaId, Nombre, Apellido, Documento, Telefono, Email, TipoPersona, CreadoEn)
                VALUES (@Id, @Nombre, @Apellido, @Documento, @Telefono, @Email, @Tipo, SYSDATETIME());";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", persona.Id);
        comando.Parameters.AddWithValue("@Nombre", persona.Nombre);
        comando.Parameters.AddWithValue("@Apellido", persona.Apellido);
        comando.Parameters.AddWithValue("@Documento", persona.Documento);
        comando.Parameters.AddWithValue("@Telefono", persona.Telefono);
        comando.Parameters.AddWithValue("@Email", (object?)persona.Email ?? DBNull.Value);
        comando.Parameters.AddWithValue("@Tipo", tipo);
        comando.ExecuteNonQuery();
    }

    private static void UpsertEstudiante(SqlConnection conexion, SqlTransaction transaccion, Estudiante estudiante)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Estudiantes WHERE EstudianteId = @Id)
                UPDATE Estudiantes SET CarreraId=@CarreraId, Carnet=@Carnet, FechaIngreso=@FechaIngreso, Estado=@Estado, Indice=@Indice WHERE EstudianteId=@Id;
            ELSE
                INSERT INTO Estudiantes (EstudianteId, CarreraId, Carnet, FechaIngreso, Estado, Indice) VALUES (@Id, @CarreraId, @Carnet, @FechaIngreso, @Estado, @Indice);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", estudiante.Id);
        comando.Parameters.AddWithValue("@CarreraId", estudiante.CarreraId);
        comando.Parameters.AddWithValue("@Carnet", (object?)estudiante.Carnet ?? DBNull.Value);
        comando.Parameters.AddWithValue("@FechaIngreso", estudiante.FechaIngreso.Date);
        comando.Parameters.AddWithValue("@Estado", estudiante.Estado.ToString());
        comando.Parameters.AddWithValue("@Indice", estudiante.Indice);
        comando.ExecuteNonQuery();
    }

    private static void UpsertDocente(SqlConnection conexion, SqlTransaction transaccion, Docente docente)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Docentes WHERE DocenteId = @Id)
                UPDATE Docentes SET Especialidad=@Especialidad, GradoAcademico=@Grado, Activo=@Activo WHERE DocenteId=@Id;
            ELSE
                INSERT INTO Docentes (DocenteId, Especialidad, GradoAcademico, Activo) VALUES (@Id, @Especialidad, @Grado, @Activo);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", docente.Id);
        comando.Parameters.AddWithValue("@Especialidad", docente.Especialidad);
        comando.Parameters.AddWithValue("@Grado", docente.GradoAcademico);
        comando.Parameters.AddWithValue("@Activo", docente.Activo);
        comando.ExecuteNonQuery();
    }

    private static void UpsertAdministrativo(SqlConnection conexion, SqlTransaction transaccion, Administrativo administrativo)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Administrativos WHERE AdministrativoId = @Id)
                UPDATE Administrativos SET Cargo=@Cargo, Departamento=@Departamento WHERE AdministrativoId=@Id;
            ELSE
                INSERT INTO Administrativos (AdministrativoId, Cargo, Departamento) VALUES (@Id, @Cargo, @Departamento);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", administrativo.Id);
        comando.Parameters.AddWithValue("@Cargo", administrativo.Cargo);
        comando.Parameters.AddWithValue("@Departamento", administrativo.Departamento);
        comando.ExecuteNonQuery();
    }

    private static void UpsertCarrera(SqlConnection conexion, SqlTransaction transaccion, Carrera carrera)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Carreras WHERE CarreraId = @Id)
                UPDATE Carreras SET Codigo=@Codigo, Nombre=@Nombre, Facultad=@Facultad, CreditosTotales=@Creditos, Activa=@Activa WHERE CarreraId=@Id;
            ELSE
                INSERT INTO Carreras (CarreraId, Codigo, Nombre, Facultad, CreditosTotales, Activa) VALUES (@Id, @Codigo, @Nombre, @Facultad, @Creditos, @Activa);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", carrera.Id);
        comando.Parameters.AddWithValue("@Codigo", carrera.Codigo);
        comando.Parameters.AddWithValue("@Nombre", carrera.Nombre);
        comando.Parameters.AddWithValue("@Facultad", carrera.Facultad);
        comando.Parameters.AddWithValue("@Creditos", carrera.CreditosTotales);
        comando.Parameters.AddWithValue("@Activa", carrera.Activa);
        comando.ExecuteNonQuery();
    }

    private static void UpsertAsignatura(SqlConnection conexion, SqlTransaction transaccion, Asignatura asignatura)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Asignaturas WHERE AsignaturaId = @Id)
                UPDATE Asignaturas SET Codigo=@Codigo, Nombre=@Nombre, Creditos=@Creditos, CarreraId=@CarreraId, Activa=@Activa WHERE AsignaturaId=@Id;
            ELSE
                INSERT INTO Asignaturas (AsignaturaId, Codigo, Nombre, Creditos, CarreraId, Activa) VALUES (@Id, @Codigo, @Nombre, @Creditos, @CarreraId, @Activa);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", asignatura.Id);
        comando.Parameters.AddWithValue("@Codigo", asignatura.Codigo);
        comando.Parameters.AddWithValue("@Nombre", asignatura.Nombre);
        comando.Parameters.AddWithValue("@Creditos", asignatura.Creditos);
        comando.Parameters.AddWithValue("@CarreraId", asignatura.CarreraId);
        comando.Parameters.AddWithValue("@Activa", asignatura.Activa);
        comando.ExecuteNonQuery();
    }

    private static void UpsertMatricula(SqlConnection conexion, SqlTransaction transaccion, Matricula matricula)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Matriculas WHERE MatriculaId = @Id)
                UPDATE Matriculas SET EstudianteId=@EstudianteId, AsignaturaId=@AsignaturaId, Periodo=@Periodo, Nota=@Nota, Estado=@Estado, Fecha=@Fecha WHERE MatriculaId=@Id;
            ELSE
                INSERT INTO Matriculas (MatriculaId, EstudianteId, AsignaturaId, Periodo, Nota, Estado, Fecha) VALUES (@Id, @EstudianteId, @AsignaturaId, @Periodo, @Nota, @Estado, @Fecha);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", matricula.Id);
        comando.Parameters.AddWithValue("@EstudianteId", matricula.EstudianteId);
        comando.Parameters.AddWithValue("@AsignaturaId", matricula.AsignaturaId);
        comando.Parameters.AddWithValue("@Periodo", matricula.Periodo);
        comando.Parameters.AddWithValue("@Nota", matricula.Nota);
        comando.Parameters.AddWithValue("@Estado", matricula.Estado.ToString());
        comando.Parameters.AddWithValue("@Fecha", matricula.Fecha);
        comando.ExecuteNonQuery();
    }

    private static void UpsertPago(SqlConnection conexion, SqlTransaction transaccion, Pago pago)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Pagos WHERE PagoId = @Id)
                UPDATE Pagos SET EstudianteId=@EstudianteId, Monto=@Monto, Concepto=@Concepto, Fecha=@Fecha, Pagado=@Pagado, Referencia=@Referencia WHERE PagoId=@Id;
            ELSE
                INSERT INTO Pagos (PagoId, EstudianteId, Monto, Concepto, Fecha, Pagado, Referencia) VALUES (@Id, @EstudianteId, @Monto, @Concepto, @Fecha, @Pagado, @Referencia);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", pago.Id);
        comando.Parameters.AddWithValue("@EstudianteId", pago.EstudianteId);
        comando.Parameters.AddWithValue("@Monto", pago.Monto);
        comando.Parameters.AddWithValue("@Concepto", pago.Concepto.ToString());
        comando.Parameters.AddWithValue("@Fecha", pago.Fecha);
        comando.Parameters.AddWithValue("@Pagado", pago.Pagado);
        comando.Parameters.AddWithValue("@Referencia", (object?)pago.Referencia ?? DBNull.Value);
        comando.ExecuteNonQuery();
    }

    private static void UpsertBeca(SqlConnection conexion, SqlTransaction transaccion, Beca beca)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Becas WHERE BecaId = @Id)
                UPDATE Becas SET EstudianteId=@EstudianteId, Tipo=@Tipo, Porcentaje=@Porcentaje, Activa=@Activa, Observacion=@Observacion WHERE BecaId=@Id;
            ELSE
                INSERT INTO Becas (BecaId, EstudianteId, Tipo, Porcentaje, Activa, Observacion) VALUES (@Id, @EstudianteId, @Tipo, @Porcentaje, @Activa, @Observacion);";
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", beca.Id);
        comando.Parameters.AddWithValue("@EstudianteId", beca.EstudianteId);
        comando.Parameters.AddWithValue("@Tipo", beca.Tipo.ToString());
        comando.Parameters.AddWithValue("@Porcentaje", beca.Porcentaje);
        comando.Parameters.AddWithValue("@Activa", beca.Activa);
        comando.Parameters.AddWithValue("@Observacion", (object?)beca.Observacion ?? DBNull.Value);
        comando.ExecuteNonQuery();
    }
}

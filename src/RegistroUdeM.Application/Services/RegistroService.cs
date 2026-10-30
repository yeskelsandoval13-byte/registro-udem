using RegistroUdeM.Application.Abstractions;
using RegistroUdeM.Application.Dtos;
using RegistroUdeM.Domain;
using RegistroUdeM.Domain.Academico;
using RegistroUdeM.Domain.Common;
using RegistroUdeM.Domain.Financiero;
using RegistroUdeM.Domain.Personas;

namespace RegistroUdeM.Application.Services;

public sealed class MatriculaService
{
    private readonly IAlmacenRegistro _almacen;
    public MatriculaService(IAlmacenRegistro almacen) => _almacen = almacen;
    private RegistroData Datos => _almacen.Datos;

    public IReadOnlyList<Matricula> ObtenerMatriculas() => Datos.Matriculas.OrderByDescending(m => m.Fecha).ToList();

    public Matricula Inscribir(Guid estudianteId, Guid asignaturaId, string periodo)
    {
        var estudiante = Datos.Estudiantes.FirstOrDefault(e => e.Id == estudianteId) ?? throw new ReglaNegocioException("El estudiante no existe.");
        var asignatura = Datos.Asignaturas.FirstOrDefault(a => a.Id == asignaturaId) ?? throw new ReglaNegocioException("La asignatura no existe.");
        if (estudiante.CarreraId != asignatura.CarreraId) throw new ReglaNegocioException("La asignatura no pertenece a la carrera del estudiante.");
        if (Datos.Matriculas.Any(m => m.EstudianteId == estudianteId && m.AsignaturaId == asignaturaId && m.Periodo == periodo && m.Estado != EstadoMatricula.Cancelada))
            throw new ReglaNegocioException("El estudiante ya estÃ¡ inscrito en esa asignatura en el perÃ­odo indicado.");
        var matricula = new Matricula { EstudianteId = estudianteId, AsignaturaId = asignaturaId, Periodo = string.IsNullOrWhiteSpace(periodo) ? "2026-1" : periodo.Trim(), Estado = EstadoMatricula.Inscrita };
        Datos.Matriculas.Add(matricula);
        _almacen.Guardar();
        return matricula;
    }

    public void AsignarNota(Guid matriculaId, decimal nota)
    {
        var matricula = Datos.Matriculas.FirstOrDefault(m => m.Id == matriculaId) ?? throw new ReglaNegocioException("La matrÃ­cula no existe.");
        if (nota < 0m || nota > 100m) throw new ReglaNegocioException("La nota debe estar entre 0 y 100.");
        matricula.Nota = nota;
        matricula.Estado = nota >= 60m ? EstadoMatricula.Aprobada : EstadoMatricula.Reprobada;
        ActualizarIndice(matricula.EstudianteId);
        _almacen.Guardar();
    }

    public void Cancelar(Guid matriculaId)
    {
        var matricula = Datos.Matriculas.FirstOrDefault(m => m.Id == matriculaId) ?? throw new ReglaNegocioException("La matrÃ­cula no existe.");
        matricula.Estado = EstadoMatricula.Cancelada;
        _almacen.Guardar();
    }

    private void ActualizarIndice(Guid estudianteId)
    {
        var estudiante = Datos.Estudiantes.FirstOrDefault(e => e.Id == estudianteId);
        if (estudiante is null) return;
        var notas = Datos.Matriculas.Where(m => m.EstudianteId == estudianteId && m.Estado is EstadoMatricula.Aprobada or EstadoMatricula.Reprobada).ToList();
        if (notas.Count == 0) return;
        estudiante.Indice = Math.Round(notas.Average(m => m.Nota), 2);
    }
}

public sealed class PagoService
{
    private readonly IAlmacenRegistro _almacen;
    public PagoService(IAlmacenRegistro almacen) => _almacen = almacen;
    private RegistroData Datos => _almacen.Datos;

    public IReadOnlyList<Pago> ObtenerPagos() => Datos.Pagos.OrderByDescending(p => p.Fecha).ToList();
    public IReadOnlyList<Pago> PagosDeEstudiante(Guid estudianteId) => Datos.Pagos.Where(p => p.EstudianteId == estudianteId).OrderByDescending(p => p.Fecha).ToList();

    public Pago RegistrarPago(Guid estudianteId, decimal monto, ConceptoPago concepto, string referencia)
    {
        if (!Datos.Estudiantes.Any(e => e.Id == estudianteId)) throw new ReglaNegocioException("El estudiante no existe.");
        if (monto <= 0m) throw new ReglaNegocioException("El monto debe ser mayor que cero.");
        var pago = new Pago { EstudianteId = estudianteId, Monto = monto, Concepto = concepto, Referencia = referencia.Trim(), Pagado = true };
        Datos.Pagos.Add(pago);
        _almacen.Guardar();
        return pago;
    }

    public IReadOnlyList<Estudiante> EstudiantesMorosos() => Datos.Estudiantes.Where(e => !Datos.Pagos.Any(p => p.EstudianteId == e.Id && p.Pagado)).ToList();
}

public sealed class BecaService
{
    private readonly IAlmacenRegistro _almacen;
    public BecaService(IAlmacenRegistro almacen) => _almacen = almacen;
    private RegistroData Datos => _almacen.Datos;

    public IReadOnlyList<Beca> ObtenerBecas() => Datos.Becas.OrderByDescending(b => b.Activa).ToList();

    public Beca AsignarBeca(Guid estudianteId, TipoBeca tipo, decimal porcentaje, string observacion)
    {
        if (!Datos.Estudiantes.Any(e => e.Id == estudianteId)) throw new ReglaNegocioException("El estudiante no existe.");
        if (porcentaje <= 0m || porcentaje > 100m) throw new ReglaNegocioException("El porcentaje debe estar entre 1 y 100.");
        var beca = new Beca { EstudianteId = estudianteId, Tipo = tipo, Porcentaje = porcentaje, Activa = true, Observacion = observacion.Trim() };
        Datos.Becas.Add(beca);
        _almacen.Guardar();
        return beca;
    }
}

public sealed class ReporteService
{
    private readonly IAlmacenRegistro _almacen;
    public ReporteService(IAlmacenRegistro almacen) => _almacen = almacen;
    private RegistroData Datos => _almacen.Datos;

    public ResumenRegistro ObtenerResumen() => new(
        Estudiantes: Datos.Estudiantes.Count,
        Docentes: Datos.Docentes.Count,
        Carreras: Datos.Carreras.Count(a => a.Activa),
        Asignaturas: Datos.Asignaturas.Count(a => a.Activa),
        Matriculas: Datos.Matriculas.Count,
        Pagos: Datos.Pagos.Count(p => p.Pagado),
        BecasActivas: Datos.Becas.Count(b => b.Activa));

    public IReadOnlyList<EstudiantesPorCarrera> EstudiantesPorCarrera() =>
        Datos.Carreras.Select(c => new EstudiantesPorCarrera(c.Nombre, Datos.Estudiantes.Count(e => e.CarreraId == c.Id))).OrderByDescending(x => x.Cantidad).ToList();

    public IReadOnlyList<MejorEstudiante> MejoresEstudiantes(int cantidad = 5) =>
        Datos.Estudiantes.Where(e => e.Estado == EstadoEstudiante.Activo).OrderByDescending(e => e.Indice).Take(cantidad)
            .Select(e => new MejorEstudiante(e.NombreCompleto, Datos.Carreras.FirstOrDefault(c => c.Id == e.CarreraId)?.Nombre ?? "", e.Indice)).ToList();
}



public sealed class RegistroService
{
    private readonly IAlmacenRegistro _almacen;
    public RegistroService(IAlmacenRegistro almacen) => _almacen = almacen;
    private RegistroData Datos => _almacen.Datos;

    public IReadOnlyList<Estudiante> ObtenerEstudiantes() => Datos.Estudiantes.OrderBy(e => e.NombreCompleto).ToList();
    public IReadOnlyList<Docente> ObtenerDocentes() => Datos.Docentes.OrderBy(d => d.NombreCompleto).ToList();
    public IReadOnlyList<Carrera> ObtenerCarreras(bool soloActivas = true) => Datos.Carreras.Where(c => !soloActivas || c.Activa).OrderBy(c => c.Nombre).ToList();
    public IReadOnlyList<Asignatura> ObtenerAsignaturas(bool soloActivas = true) => Datos.Asignaturas.Where(a => !soloActivas || a.Activa).OrderBy(a => a.Nombre).ToList();

    public Estudiante RegistrarEstudiante(string nombre, string apellido, string documento, string telefono, string email, Guid carreraId, DateTime fechaIngreso)
    {
        Validador.TextoRequerido(nombre, "nombre");
        Validador.TextoRequerido(apellido, "apellido");
        Validador.TextoRequerido(documento, "documento");
        Validador.TextoRequerido(telefono, "teléfono");
        if (Datos.Estudiantes.Any(e => e.Documento.Equals(documento.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new ReglaNegocioException($"Ya existe un estudiante con el documento '{documento.Trim()}'.");
        if (!Datos.Carreras.Any(c => c.Id == carreraId))
            throw new ReglaNegocioException("La carrera seleccionada no existe.");
        var estudiante = new Estudiante
        {
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Documento = documento.Trim(),
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            CarreraId = carreraId,
            FechaIngreso = fechaIngreso.Date,
            Estado = EstadoEstudiante.Activo
        };
        Datos.Estudiantes.Add(estudiante);
        _almacen.Guardar();
        return estudiante;
    }

    public Docente RegistrarDocente(string nombre, string apellido, string documento, string telefono, string especialidad, string grado)
    {
        Validador.TextoRequerido(nombre, "nombre");
        Validador.TextoRequerido(apellido, "apellido");
        Validador.TextoRequerido(documento, "documento");
        var docente = new Docente
        {
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Documento = documento.Trim(),
            Telefono = telefono.Trim(),
            Especialidad = string.IsNullOrWhiteSpace(especialidad) ? "Docencia universitaria" : especialidad.Trim(),
            GradoAcademico = string.IsNullOrWhiteSpace(grado) ? "Licenciatura" : grado.Trim()
        };
        Datos.Docentes.Add(docente);
        _almacen.Guardar();
        return docente;
    }

    public Carrera RegistrarCarrera(string codigo, string nombre, string facultad, int creditos)
    {
        Validador.TextoRequerido(codigo, "código");
        Validador.TextoRequerido(nombre, "nombre");
        Validador.Rango(creditos, 1, 300, "créditos");
        var carrera = new Carrera { Codigo = codigo.Trim().ToUpperInvariant(), Nombre = nombre.Trim(), Facultad = string.IsNullOrWhiteSpace(facultad) ? "Ingeniería" : facultad.Trim(), CreditosTotales = creditos };
        Datos.Carreras.Add(carrera);
        _almacen.Guardar();
        return carrera;
    }

    public Asignatura RegistrarAsignatura(string codigo, string nombre, int creditos, Guid carreraId)
    {
        Validador.TextoRequerido(codigo, "código");
        Validador.TextoRequerido(nombre, "nombre");
        Validador.Rango(creditos, 1, 12, "créditos");
        if (!Datos.Carreras.Any(c => c.Id == carreraId)) throw new ReglaNegocioException("La carrera no existe.");
        var asignatura = new Asignatura { Codigo = codigo.Trim().ToUpperInvariant(), Nombre = nombre.Trim(), Creditos = creditos, CarreraId = carreraId };
        Datos.Asignaturas.Add(asignatura);
        _almacen.Guardar();
        return asignatura;
    }

    public Estudiante? ObtenerEstudiante(Guid id) => Datos.Estudiantes.FirstOrDefault(e => e.Id == id);
    public Carrera? ObtenerCarrera(Guid id) => Datos.Carreras.FirstOrDefault(c => c.Id == id);
    public Asignatura? ObtenerAsignatura(Guid id) => Datos.Asignaturas.FirstOrDefault(a => a.Id == id);
}

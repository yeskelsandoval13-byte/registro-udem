namespace RegistroUdeM.Domain.Academico;

/// <summary>
/// Carrera universitaria.
/// </summary>
public class Carrera : Common.EntidadBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Facultad { get; set; } = "Ingeniería";
    public int CreditosTotales { get; set; } = 180;
    public bool Activa { get; set; } = true;
}

/// <summary>
/// Asignatura asociada a una carrera.
/// </summary>
public class Asignatura : Common.EntidadBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Creditos { get; set; } = 4;
    public Guid CarreraId { get; set; }
    public bool Activa { get; set; } = true;
}

public enum EstadoMatricula
{
    Inscrita,
    Cursando,
    Aprobada,
    Reprobada,
    Cancelada
}

/// <summary>
/// Matrícula de un estudiante en una asignatura.
/// </summary>
public class Matricula : Common.EntidadBase
{
    public Guid EstudianteId { get; set; }
    public Guid AsignaturaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public decimal Nota { get; set; }
    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Inscrita;
    public DateTime Fecha { get; set; } = DateTime.Now;

    public bool EstaAprobada => Nota >= 60m;
}

using System.Text.Json.Serialization;

namespace RegistroUdeM.Domain.Personas;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(Estudiante), "estudiante")]
[JsonDerivedType(typeof(Docente), "docente")]
[JsonDerivedType(typeof(Administrativo), "administrativo")]
/// <summary>
/// Persona base del sistema universitario. No se registra una persona genérica.
/// </summary>
public abstract class Persona : Common.EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public abstract string Rol { get; }
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
}

public enum EstadoEstudiante
{
    Activo,
    Inactivo,
    Graduado,
    Suspendido
}

/// <summary>
/// Estudiante universitario. Guarda carrera, carnet, estado e índice académico.
/// </summary>
public sealed class Estudiante : Persona
{
    public string Carnet { get; set; } = string.Empty;
    public Guid CarreraId { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.Today;
    public EstadoEstudiante Estado { get; set; } = EstadoEstudiante.Activo;
    public decimal Indice { get; set; }
    public override string Rol => "Estudiante";
}

/// <summary>
/// Docente de la universidad.
/// </summary>
public sealed class Docente : Persona
{
    public string Especialidad { get; set; } = "Docencia universitaria";
    public string GradoAcademico { get; set; } = "Licenciatura";
    public bool Activo { get; set; } = true;
    public override string Rol => "Docente";
}

/// <summary>
/// Personal administrativo de registro.
/// </summary>
public sealed class Administrativo : Persona
{
    public string Cargo { get; set; } = "Administrativo";
    public string Departamento { get; set; } = "Registro";
    public override string Rol => "Administrativo";
}

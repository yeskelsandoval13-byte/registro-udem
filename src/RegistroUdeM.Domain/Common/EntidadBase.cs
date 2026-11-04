namespace RegistroUdeM.Domain.Common;

/// <summary>
/// Clase base de todas las entidades. Centraliza la identidad y la fecha de creación.
/// </summary>
public abstract class EntidadBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreadoEn { get; set; } = DateTime.Now;
}

/// <summary>
/// Excepción para errores esperados de negocio, con mensaje claro para el usuario.
/// </summary>
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje) { }
}

/// <summary>
/// Validaciones reutilizables para mantener reglas consistentes.
/// </summary>
public static class Validador
{
    public static void TextoRequerido(string? valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ReglaNegocioException($"El campo '{campo}' es obligatorio.");
    }

    public static void Rango(decimal valor, decimal minimo, decimal maximo, string campo)
    {
        if (valor < minimo || valor > maximo)
            throw new ReglaNegocioException($"El campo '{campo}' debe estar entre {minimo:N2} y {maximo:N2}.");
    }

    public static void Rango(int valor, int minimo, int maximo, string campo)
    {
        if (valor < minimo || valor > maximo)
            throw new ReglaNegocioException($"El campo '{campo}' debe estar entre {minimo} y {maximo}.");
    }

    public static void FechaNoFutura(DateTime fecha, string campo)
    {
        if (fecha.Date > DateTime.Today)
            throw new ReglaNegocioException($"El campo '{campo}' no puede ser una fecha futura.");
    }
}

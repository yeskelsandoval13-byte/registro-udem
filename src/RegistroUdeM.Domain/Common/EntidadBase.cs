namespace RegistroUdeM.Domain.Common;

public abstract class EntidadBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreadoEn { get; set; } = DateTime.Now;
}

public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje) { }
}

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

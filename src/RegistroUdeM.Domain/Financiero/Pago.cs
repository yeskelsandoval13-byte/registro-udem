namespace RegistroUdeM.Domain.Financiero;

public enum ConceptoPago
{
    Matricula,
    Mensualidad,
    Examen,
    Certificado,
    Otro
}

/// <summary>
/// Pago realizado por un estudiante.
/// </summary>
public class Pago : Common.EntidadBase
{
    public Guid EstudianteId { get; set; }
    public decimal Monto { get; set; }
    public ConceptoPago Concepto { get; set; } = ConceptoPago.Mensualidad;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool Pagado { get; set; }
    public string Referencia { get; set; } = string.Empty;
}

public enum TipoBeca
{
    Academica,
    Deportiva,
    Socioeconomica,
    Excelencia
}

/// <summary>
/// Beca asignada a un estudiante.
/// </summary>
public class Beca : Common.EntidadBase
{
    public Guid EstudianteId { get; set; }
    public TipoBeca Tipo { get; set; } = TipoBeca.Academica;
    public decimal Porcentaje { get; set; } = 25m;
    public bool Activa { get; set; } = true;
    public string Observacion { get; set; } = string.Empty;
}

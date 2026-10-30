namespace RegistroUdeM.Laboratorio.Clase03;

/// <summary>
/// Clase padre de la ilustraciÃ³n de herencia de la Clase 03.
/// Contiene los datos comunes y los mÃ©todos que heredan
/// Futbolista, Entrenador y Masajista.
/// </summary>
public class SeleccionFutbol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int Edad { get; set; }

    public virtual string Concentrarse() =>
        $"{Nombre} {Apellidos} se concentra con la selecciÃ³n.";

    public virtual string Viajar() =>
        $"{Nombre} {Apellidos} viaja con la selecciÃ³n.";
}

/// <summary>
/// Clase hija: futbolista. Hereda los datos personales y agrega
/// dorsal y demarcaciÃ³n, ademÃ¡s de sus propias acciones.
/// </summary>
public class Futbolista : SeleccionFutbol
{
    public int Dorsal { get; set; }
    public string Demarcacion { get; set; } = string.Empty;

    public override string Concentrarse() =>
        $"{Nombre} {Apellidos} se concentra para el partido como {Demarcacion}.";

    public override string Viajar() =>
        $"{Nombre} {Apellidos} viaja en el autobÃºs del equipo.";

    public string JugarPartido() =>
        $"{Nombre} {Apellidos} juega el partido con el dorsal {Dorsal}.";

    public string Entrenar() =>
        $"{Nombre} {Apellidos} entrena la definiciÃ³n y la resistencia.";
}

/// <summary>
/// Clase hija: entrenador. Agrega su licencia federativa y las
/// acciones propias de direcciÃ³n tÃ©cnica.
/// </summary>
public class Entrenador : SeleccionFutbol
{
    public string IdFederacion { get; set; } = string.Empty;

    public override string Concentrarse() =>
        $"{Nombre} {Apellidos} revisa la estrategia con el cuerpo tÃ©cnico.";

    public override string Viajar() =>
        $"{Nombre} {Apellidos} viaja con el cuerpo tÃ©cnico.";

    public string DirigirPartido() =>
        $"{Nombre} {Apellidos} dirige el partido desde la banca.";

    public string DirigirEntrenamiento() =>
        $"{Nombre} {Apellidos} dirige el entrenamiento tÃ¡ctico.";
}

/// <summary>
/// Clase hija: masajista. Agrega titulaciÃ³n y experiencia, y las
/// acciones de recuperaciÃ³n fÃ­sica del equipo.
/// </summary>
public class Masajista : SeleccionFutbol
{
    public string Titulacion { get; set; } = string.Empty;
    public int AniosExperiencia { get; set; }

    public override string Concentrarse() =>
        $"{Nombre} {Apellidos} prepara la camilla y los vendajes.";

    public override string Viajar() =>
        $"{Nombre} {Apellidos} viaja con el equipo mÃ©dico.";

    public string DarMasaje() =>
        $"{Nombre} {Apellidos} da un masaje de recuperaciÃ³n ({Titulacion}, {AniosExperiencia} aÃ±os de experiencia).";
}


using System.Text.Json;
using RegistroUdeM.Application.Abstractions;
using RegistroUdeM.Domain;

namespace RegistroUdeM.Infrastructure.Persistencia;

/// <summary>
/// Almacén JSON con guardado atómico y recuperación de archivos dañados.
/// </summary>
public sealed class JsonAlmacenRegistro : IAlmacenRegistro
{
    private readonly string _ruta;
    public RegistroData Datos { get; private set; }

    public JsonAlmacenRegistro(string ruta)
    {
        _ruta = ruta;
        Datos = Cargar();
    }

    public void Guardar()
    {
        var directorio = Path.GetDirectoryName(_ruta);
        if (!string.IsNullOrWhiteSpace(directorio)) Directory.CreateDirectory(directorio);
        var temporal = _ruta + ".tmp";
        var json = JsonSerializer.Serialize(Datos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(temporal, json);
        if (File.Exists(_ruta)) File.Delete(_ruta);
        File.Move(temporal, _ruta);
    }

    public void Restablecer(RegistroData datos)
    {
        Datos = datos;
        Guardar();
    }

    private RegistroData Cargar()
    {
        if (!File.Exists(_ruta)) return DatosDemo.Crear();
        try
        {
            var json = File.ReadAllText(_ruta);
            return JsonSerializer.Deserialize<RegistroData>(json) ?? DatosDemo.Crear();
        }
        catch
        {
            var copia = _ruta + ".corrupto-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
            File.Copy(_ruta, copia, true);
            return DatosDemo.Crear();
        }
    }
}


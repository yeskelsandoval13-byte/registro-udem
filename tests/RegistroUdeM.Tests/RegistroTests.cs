using RegistroUdeM.Application.Services;
using RegistroUdeM.Domain;
using RegistroUdeM.Infrastructure.Persistencia;

namespace RegistroUdeM.Tests;

public class RegistroTests
{
    [Fact]
    public void RegistrarEstudiante_CreaEstudiante()
    {
        var ruta = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json"); File.WriteAllText(ruta, "{}"); var almacen = new JsonAlmacenRegistro(ruta);
        var registro = new RegistroService(almacen);
        var carrera = registro.RegistrarCarrera("ING", "Ingeniería", "Ingeniería", 180);
        var estudiante = registro.RegistrarEstudiante("Ana", "Lopez", "DOC", "123", "ana@udem.edu.ni", carrera.Id, DateTime.Today);
        Assert.Equal("Ana", estudiante.Nombre);
        Assert.Single(registro.ObtenerEstudiantes());
    }

    [Fact]
    public void Matricula_AsignarNota_ActualizaIndice()
    {
        var ruta = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json"); File.WriteAllText(ruta, "{}"); var almacen = new JsonAlmacenRegistro(ruta);
        var registro = new RegistroService(almacen);
        var matriculas = new MatriculaService(almacen);
        var carrera = registro.RegistrarCarrera("ING", "Ingeniería", "Ingeniería", 180);
        var asignatura = registro.RegistrarAsignatura("POO", "POO", 4, carrera.Id);
        var estudiante = registro.RegistrarEstudiante("Ana", "Lopez", "DOC", "123", "ana@udem.edu.ni", carrera.Id, DateTime.Today);
        var matricula = matriculas.Inscribir(estudiante.Id, asignatura.Id, "2026-1");
        matriculas.AsignarNota(matricula.Id, 90m);
        Assert.Equal(90m, registro.ObtenerEstudiante(estudiante.Id)!.Indice);
    }

    [Fact]
    public void Clase03_Herencia_Funciona()
    {
        var futbolista = new RegistroUdeM.Laboratorio.Clase03.Futbolista { Nombre = "Ana", Apellidos = "Lopez", Edad = 20, Dorsal = 10, Demarcacion = "Delantera" };
        Assert.Equal("Ana", futbolista.Nombre);
        Assert.Equal(10, futbolista.Dorsal);
    }
}


using RegistroUdeM.Domain;
using RegistroUdeM.Domain.Academico;
using RegistroUdeM.Domain.Financiero;
using RegistroUdeM.Domain.Personas;

namespace RegistroUdeM.Infrastructure.Persistencia;

public static class DatosDemo
{
    public static RegistroData Crear()
    {
        var datos = new RegistroData();
        var ing = new Carrera { Codigo = "ING", Nombre = "Ingeniería en Sistemas", Facultad = "Ingeniería", CreditosTotales = 180 };
        var adm = new Carrera { Codigo = "ADM", Nombre = "Administración de Empresas", Facultad = "Ciencias Económicas", CreditosTotales = 160 };
        datos.Carreras.AddRange([ing, adm]);

        var poo = new Asignatura { Codigo = "POO", Nombre = "Programación Orientada a Objetos", Creditos = 4, CarreraId = ing.Id };
        var bd = new Asignatura { Codigo = "BD", Nombre = "Base de Datos", Creditos = 4, CarreraId = ing.Id };
        var cont = new Asignatura { Codigo = "CONT", Nombre = "Contabilidad I", Creditos = 3, CarreraId = adm.Id };
        datos.Asignaturas.AddRange([poo, bd, cont]);

        var est1 = new Estudiante { Nombre = "María", Apellido = "González", Documento = "001-010101-0001A", Telefono = "8888-1111", Email = "maria@udem.edu.ni", Carnet = "2024-001", CarreraId = ing.Id, FechaIngreso = new DateTime(2024, 2, 1) };
        var est2 = new Estudiante { Nombre = "Carlos", Apellido = "Pérez", Documento = "001-020202-0002B", Telefono = "8888-2222", Email = "carlos@udem.edu.ni", Carnet = "2023-002", CarreraId = adm.Id, FechaIngreso = new DateTime(2023, 8, 1) };
        datos.Estudiantes.AddRange([est1, est2]);

        datos.Docentes.Add(new Docente { Nombre = "Ana", Apellido = "Martínez", Documento = "001-030303-0003C", Telefono = "8888-3333", Especialidad = "Ingeniería de Software", GradoAcademico = "Maestría" });

        datos.Matriculas.Add(new Matricula { EstudianteId = est1.Id, AsignaturaId = poo.Id, Periodo = "2026-1", Nota = 92m, Estado = EstadoMatricula.Aprobada });
        datos.Matriculas.Add(new Matricula { EstudianteId = est1.Id, AsignaturaId = bd.Id, Periodo = "2026-1", Nota = 88m, Estado = EstadoMatricula.Aprobada });
        datos.Matriculas.Add(new Matricula { EstudianteId = est2.Id, AsignaturaId = cont.Id, Periodo = "2026-1", Nota = 75m, Estado = EstadoMatricula.Aprobada });

        est1.Indice = 90m;
        est2.Indice = 75m;

        datos.Pagos.Add(new Pago { EstudianteId = est1.Id, Monto = 1500m, Concepto = ConceptoPago.Mensualidad, Pagado = true, Referencia = "REC-001" });
        datos.Becas.Add(new Beca { EstudianteId = est1.Id, Tipo = TipoBeca.Excelencia, Porcentaje = 50m, Activa = true, Observacion = "Promedio sobresaliente" });

        return datos;
    }
}

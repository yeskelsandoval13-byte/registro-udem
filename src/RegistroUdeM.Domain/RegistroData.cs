using RegistroUdeM.Domain.Academico;
using RegistroUdeM.Domain.Financiero;
using RegistroUdeM.Domain.Personas;

namespace RegistroUdeM.Domain;

public sealed class RegistroData
{
    public List<Estudiante> Estudiantes { get; set; } = [];
    public List<Docente> Docentes { get; set; } = [];
    public List<Administrativo> Administrativos { get; set; } = [];
    public List<Carrera> Carreras { get; set; } = [];
    public List<Asignatura> Asignaturas { get; set; } = [];
    public List<Matricula> Matriculas { get; set; } = [];
    public List<Pago> Pagos { get; set; } = [];
    public List<Beca> Becas { get; set; } = [];
}

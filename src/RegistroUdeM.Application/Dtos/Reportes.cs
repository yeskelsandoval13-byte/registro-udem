namespace RegistroUdeM.Application.Dtos;

public sealed record ResumenRegistro(int Estudiantes, int Docentes, int Carreras, int Asignaturas, int Matriculas, int Pagos, int BecasActivas);
public sealed record EstudiantesPorCarrera(string Carrera, int Cantidad);
public sealed record MejorEstudiante(string Nombre, string Carrera, decimal Indice);

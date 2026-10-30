# RegistroUdeM

**Sistema de registro universitario**  
Avance del proyecto final · Programación Orientada a Objetos (FI-134)

> Universidad de Managua (UdeM) · "Formando profesionales con valores."

RegistroUdeM es un sistema académico ficticio para gestionar estudiantes, docentes, carreras, asignaturas, matrículas, calificaciones, pagos y becas. El proyecto demuestra herencia, polimorfismo y sobrecarga en un dominio real, con arquitectura por capas y persistencia JSON.

## Módulos

| Módulo | Funcionalidades |
|---|---|
| Estudiantes | Registro, búsqueda, estado académico e índice. |
| Docentes | Registro y datos académicos. |
| Carreras | Alta de carreras y créditos. |
| Asignaturas | Alta de asignaturas por carrera. |
| Matrículas | Inscripción, asignación de nota, aprobación/reprobación. |
| Pagos | Registro de pagos y detección de morosos. |
| Becas | Asignación de becas por tipo y porcentaje. |
| Reportes | Resumen, estudiantes por carrera y mejores índices. |
| Laboratorio Clase 03 | `SeleccionFutbol`, `Forma`, sobrecarga. |

## Persistencia

- JSON local con guardado atómico.
- **SQL Server** con `SqlAlmacenRegistro` (ADO.NET), carga y guardado en tiempo real.

```powershell
dotnet restore
dotnet build
dotnet run --project src/RegistroUdeM.Console
dotnet run --project src/RegistroUdeM.Console -- --demo
dotnet run --project src/RegistroUdeM.WinForms
dotnet test
```

## Documentación
- `docs/AVANCE.md`
- `docs/AVANCE.docx` (versión Word para entrega/estudio)
- `docs/ARQUITECTURA.md`
- `docs/CLASE_03.md`
- `docs/GUION_DEFENSA.md`
- `docs/RUBRICA.md`
- `docs/GUIA_ESTUDIO.md`
- `docs/bd/README.md`

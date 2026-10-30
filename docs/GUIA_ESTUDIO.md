# Guía de estudio · RegistroUdeM

## Resumen
Sistema de registro universitario en C#/.NET 10 con POO, Windows Forms, consola, JSON y diseño de base de datos.

## POO
- Herencia: `Persona → Estudiante, Docente, Administrativo`.
- Polimorfismo: `Rol`, reportes y servicios.
- Sobrecarga: laboratorio Clase 03 y métodos de servicios.
- Encapsulamiento: `Indice`, `Estado`, validaciones.

## Base de datos
Tablas: `Personas`, `Estudiantes`, `Docentes`, `Administrativos`, `Carreras`, `Asignaturas`, `Matriculas`, `Pagos`, `Becas`.

## Comandos
```powershell
dotnet build RegistroUdeM.slnx
dotnet test
dotnet run --project src/RegistroUdeM.Console
dotnet run --project src/RegistroUdeM.WinForms
```

## Preguntas probables
- ¿Dónde está la herencia? En `Persona`.
- ¿Dónde está el polimorfismo? En `Rol` y reportes.
- ¿Por qué JSON? Porque es el avance; SQL queda diseñado.
- ¿Qué falta? CRUD completo, SQL integrado, autenticación.

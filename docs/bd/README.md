# Diseño de base de datos · RegistroUdeM

## Fases
1. Requisitos.
2. Modelo conceptual.
3. Modelo lógico.
4. Normalización.
5. Modelo físico.
6. Implementación.

## Modelo conceptual (Mermaid)

```mermaid
erDiagram
    PERSONAS ||--o{ ESTUDIANTES : es
    PERSONAS ||--o{ DOCENTES : es
    PERSONAS ||--o{ ADMINISTRATIVOS : es
    CARRERAS ||--o{ ASIGNATURAS : contiene
    CARRERAS ||--o{ ESTUDIANTES : tiene
    ESTUDIANTES ||--o{ MATRICULAS : realiza
    ASIGNATURAS ||--o{ MATRICULAS : recibe
    ESTUDIANTES ||--o{ PAGOS : realiza
    ESTUDIANTES ||--o{ BECAS : recibe
```

## Tablas principales
- `Personas`
- `Estudiantes`
- `Docentes`
- `Administrativos`
- `Carreras`
- `Asignaturas`
- `Matriculas`
- `Pagos`
- `Becas`

## Script SQL (resumen)
```sql
CREATE TABLE Personas (...);
CREATE TABLE Estudiantes (...);
CREATE TABLE Carreras (...);
CREATE TABLE Asignaturas (...);
CREATE TABLE Matriculas (...);
CREATE TABLE Pagos (...);
CREATE TABLE Becas (...);
```

La implementación completa puede generarse siguiendo el modelo de HuellaVet adaptado a RegistroUdeM.

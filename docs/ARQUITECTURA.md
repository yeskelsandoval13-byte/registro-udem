# Arquitectura de RegistroUdeM

## Capas

- `RegistroUdeM.Domain`: entidades y reglas.
- `RegistroUdeM.Application`: casos de uso y reportes.
- `RegistroUdeM.Infrastructure`: persistencia JSON.
- `RegistroUdeM.Console`: interfaz de consola.
- `RegistroUdeM.WinForms`: interfaz gráfica.
- `RegistroUdeM.Laboratorio`: Clase 03.

## Regla principal
El dominio no conoce la interfaz ni la persistencia.

## Persistencia
`IAlmacenRegistro` permite cambiar JSON por SQL Server sin modificar los servicios. La implementación SQL es `SqlAlmacenRegistro`.

## Futuro
- SQL Server + ADO.NET.
- CRUD completo.
- Autenticación y roles.
- Reportes exportables.

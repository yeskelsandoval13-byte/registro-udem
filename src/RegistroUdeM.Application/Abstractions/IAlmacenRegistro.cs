using RegistroUdeM.Domain;

namespace RegistroUdeM.Application.Abstractions;

/// <summary>
/// Contrato de persistencia para el registro universitario.
/// Permite cambiar JSON por SQL Server sin modificar los servicios.
/// </summary>
public interface IAlmacenRegistro
{
    RegistroData Datos { get; }
    void Guardar();
    void Restablecer(RegistroData datos);
}

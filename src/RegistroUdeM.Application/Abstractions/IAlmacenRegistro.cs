using RegistroUdeM.Domain;

namespace RegistroUdeM.Application.Abstractions;

public interface IAlmacenRegistro
{
    RegistroData Datos { get; }
    void Guardar();
    void Restablecer(RegistroData datos);
}

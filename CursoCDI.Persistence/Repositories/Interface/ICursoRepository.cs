using CursoCDI.Domain.Dtos.Requests;
using CursoCDI.Domain.Dtos.Responses;
using CursoCDI.Domain.Entities;

namespace CursoCDI.Persistence.Repositories.Interface;
public interface ICursoRepository
{
    bool ValidarCreacionCurso(string nombreCurso);
    bool Borrar(string id);
    bool Actualizar(string id, ActualizarCursoRequest request);
    void Crear(Curso curso);
    ConsultarCursoResponse? Consultar(string id);
    IEnumerable<ConsultarCursoResponse> Consultar(ConsultarCursoRequest consltarCurso);

}

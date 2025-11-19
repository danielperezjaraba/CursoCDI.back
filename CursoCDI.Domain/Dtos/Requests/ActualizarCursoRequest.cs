using CursoCDI.Domain.Enums;

namespace CursoCDI.Domain.Dtos.Requests;

public record ActualizarCursoRequest(
    int DocenteId, 
    EstadoCurso Estado
);

namespace CursoCDI.Domain.Dtos.Requests;

public record CrearCursoRequest(
    
    string NombreCurso,
    int DocenteId,
    DateOnly FechaInicio,
    DateOnly FechaFinalizacion

);

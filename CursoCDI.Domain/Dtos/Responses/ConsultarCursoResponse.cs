using CursoCDI.Domain.Enums;

namespace CursoCDI.Domain.Dtos.Responses;
public record ConsultarCursoResponse(string Id, string Nombre, DateOnly FechaInicio, DateOnly FechaFinalizacion, EstadoCurso Estado, string DocenteNombre);

using CursoCDI.Domain.Enums;

namespace CursoCDI.Domain.Dtos.Requests;

public record ConsultarCursoRequest(string? Nombre, long? DocenteId, EstadoCurso? Estado, DateOnly? FechaInicio);
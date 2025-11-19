using CursoCDI.Application.Extensions;
using CursoCDI.Application.Options;
using CursoCDI.Domain.Dtos.Requests;
using CursoCDI.Domain.Dtos.Responses;
using CursoCDI.Domain.Entities;
using CursoCDI.Domain.Enums;
using CursoCDI.Dominio.Exceptions;
using CursoCDI.Persistence.Repositories.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CursoCDI.Application.Services;

public class CursoService
{
    private readonly ICursoRepository _cursoRepository;
    private readonly DateOnly _fechaFinClases;
    private readonly string _periodoActual;
    private readonly NotasOption _notasOption;


    public CursoService(ICursoRepository cursoRepository, IConfiguration configuration, IOptions<NotasOption> options)
    {
        _cursoRepository = cursoRepository;
        _fechaFinClases = DateOnly.Parse(configuration.GetSection("FechaFinClases").Value!);
        _periodoActual = configuration.GetSection("PeriodoActual").Value!;
        _notasOption = options.Value;
    }

    public CursoResponse? Crear(CrearCursoRequest request)
    {

        var cursosConElMismoNombre = _cursoRepository.ValidarCreacionCurso(request.NombreCurso);

        //Lanzo excepciones propias
        if (!cursosConElMismoNombre) {

            throw new BusinessRoleException("Ya existe un curso abierto con el nombre ingresado");
        }

        if (request.FechaFinalizacion > _fechaFinClases)
        {
            throw new BusinessRoleException($"La fecha de finalización no puede ser mayor a la fecha de fin de clases del período {_periodoActual} {_fechaFinClases.ToString("dd/MM/yyyy")}");
        }


        var curso = new Curso {
            Id = DateTime.Now.Ticks.ToString(),
            Nombre = request.NombreCurso.Sanitize().RemoveAccents(),
            FechaInicio = request.FechaInicio,
            FechaFinalizacion = request.FechaFinalizacion,
            DocenteId = request.DocenteId,
            Estado = EstadoCurso.Abierto,
            FechaCreacion = DateTime.Now
        
        };
     

        _cursoRepository.Crear(curso);

        return new CursoResponse(curso.Id, curso.Nombre, "");

    }

    public bool Borrar(string id)
    {
        return _cursoRepository.Borrar(id);
    }

    public ConsultarCursoResponse? Consultar(string id)
    {
        return _cursoRepository.Consultar(id);
        
    }

    public bool Actualizar(string id, ActualizarCursoRequest request)
    {

        return _cursoRepository.Actualizar(id, request);

    }

    public IEnumerable<ConsultarCursoResponse> Consultar(ConsultarCursoRequest request)
    {
        return _cursoRepository.Consultar(request);
    }

}

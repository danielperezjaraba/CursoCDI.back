using CursoCDI.Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;
using CursoCDI.Domain.Dtos.Requests;
using CursoCDI.Application.Services;


namespace CursoCDI.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CursosController : ControllerBase
{
    //Recordar que REST es sin estado
    private readonly CursoService _cursoService;

    public CursosController(CursoService cursoService)
    {
        _cursoService = cursoService;

    }

    [HttpPost]
    public ActionResult Crear(CrearCursoRequest request)
    {
        try
        {
            var curso = _cursoService.Crear(request);

            if (curso is null)
            {
                return BadRequest();
            }
            return Created(string.Empty, curso);
        }
        catch (BusinessRoleException exb)
        {
            return UnprocessableEntity(exb.Message);
        }
        catch (Exception exg)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, exg.Message);
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Borrar(string id)
    {
        var borrado = _cursoService.Borrar(id);
        if (!borrado)
        {
            return NotFound();
        }
        return NoContent();
    }


    [HttpGet("{id}")]
    public ActionResult Consultar(string id)
    {
        var curso = _cursoService.Consultar(id);
        if (curso is null)
        {
            return NotFound();
        }
        return Ok(curso);
    }
    [HttpGet]
    public ActionResult Consultar([FromQuery] ConsultarCursoRequest request)
    {
        return Ok(_cursoService.Consultar(request));
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar(string id, ActualizarCursoRequest request)
    {
        var curso = _cursoService.Actualizar(id, request);

        if (!curso)
        {

            return NotFound();
        }
        return NoContent();
    }
}


using CursoCDI.Domain.Dtos.Requests;
using CursoCDI.Domain.Dtos.Responses;
using CursoCDI.Domain.Entities;
using CursoCDI.Domain.Enums;
using CursoCDI.Persistence.DbContexts;
using CursoCDI.Persistence.Repositories.Interface;

namespace CursoCDI.Persistence.Repositories;

public class CursoEfcRepository(CursosDbContext context) : ICursoRepository
{

    private readonly CursosDbContext _context = context;
    public void Crear(Curso curso)
    {

        /*
         
        INSERT INTO cursos VALUES(@id,@nombre,@fechaInicio,@fechaFinalizacion,@fechaCreacion,@docenteId,@estado,@horasSemanales)
         */

        _context.Cursos.Add(curso);
        _context.SaveChanges();
    }

    public bool Borrar(string id)
    {
        /*
        
            SELECT * FROM cursos WHERE id = @id

         */
        var curso = _context.Cursos.Find(id);

        if (curso is not null)
        {
            /*
             DELETE FROM cursos WHERE id = @id
             */
            _context.Cursos.Remove(curso);
        }
        return _context.SaveChanges() > 0;
    }

    public ConsultarCursoResponse? Consultar(string id)
    {

        /*
                SELECT c.id,
	                       c.nombre,
                           c.fecha_inicio,
                           c.fecha_finalizacion,
                           c.estado,
                           concat(d.nombre,' ' ,d.apellido) as docente_nombre
                    FROM cursos c 
	                    INNER JOIN docentes d ON c.docente_id = d.id 
                    WHERE c.id =@id";
         
         */


        return _context.Cursos
            .Where(curso => curso.Id == id) // Filtramos por el id del curso
            .Join(_context.Docentes,
                  curso => curso.DocenteId,      // Clave foránea en Cursos
                  docente => docente.Id,         // Clave primaria en Docentes
                  (curso, docente) => new ConsultarCursoResponse(
                      curso.Id,
                      curso.Nombre,
                      curso.FechaInicio,
                      curso.FechaFinalizacion,
                      curso.Estado,
                      $"{docente.Nombre} {docente.Apellido}"
                  ))
            .FirstOrDefault(); // Obtener el primer resultado, o null si no existe
    }

    public bool ValidarCreacionCurso(string nombre)
    {
        /*
         SELECT COUNT(id) 
         FROM cursos 
         WHERE nombre =@nombre  AND estado = 1
         */

        return !_context.Cursos.Any(c => c.Nombre == nombre && c.Estado == EstadoCurso.Abierto);

    }
    public bool Actualizar(string id, ActualizarCursoRequest request)
    {
        var curso = _context.Cursos.Find(id);

        if (curso is not null)
        {
            curso.Estado = request.Estado;
            curso.DocenteId = request.DocenteId;
        }


        /*
        UPDATE cursos
        SET estado = @estado, docente_id =@docenteId 
        WHERE id =@id
         */

        return _context.SaveChanges() > 0;
    }



    public IEnumerable<ConsultarCursoResponse> Consultar(ConsultarCursoRequest consultarCurso)
    {
        var consulta = _context.Cursos.AsQueryable();

        // Filtramos según los parámetros de la solicitud
        if (consultarCurso.Estado is not null)
        {
            consulta = consulta.Where(c => c.Estado == consultarCurso.Estado);
        }

        if (consultarCurso.DocenteId is not null)
        {
            consulta = consulta.Where(c => c.DocenteId == consultarCurso.DocenteId);
        }

        if (!string.IsNullOrWhiteSpace(consultarCurso.Nombre))
        {
            consulta = consulta.Where(c => c.Nombre.Contains(consultarCurso.Nombre));
        }

        if (consultarCurso.FechaInicio is not null)
        {
            consulta = consulta.Where(c => c.FechaInicio == consultarCurso.FechaInicio);
        }

        // Realizamos el JOIN con los docentes
        var resultados = consulta
            .Join(_context.Docentes,
                  curso => curso.DocenteId,  // Clave foránea en Cursos
                  docente => docente.Id,     // Clave primaria en Docentes
                  (curso, docente) => new ConsultarCursoResponse(
                      curso.Id,
                      curso.Nombre,
                      curso.FechaInicio,
                      curso.FechaFinalizacion,
                      curso.Estado, // Suponiendo que EstadoCurso es un enum o clase
                      $"{docente.Nombre} {docente.Apellido}"
                  ))
            .AsEnumerable();  // Convierte a IEnumerable para su uso posterior

        return resultados;
    }


}

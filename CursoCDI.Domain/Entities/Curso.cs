using CursoCDI.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoCDI.Domain.Entities;

[Table("cursos")]
public class Curso
{


    [Key]
    [Column("id")]
    public string Id { get; init; }
    [Column("nombre")]
    public string Nombre { get; set; }
    [Column("Fecha_inicio")]
    public DateOnly FechaInicio { get; init; }
    [Column("Fecha_finalizacion")]
    public DateOnly FechaFinalizacion { get; init; }
    [Column("Fecha_creacion")]
    public DateTime FechaCreacion { get; init; }
    [Column("docente_id")]
    public int DocenteId { get; set; }
    public Docente Docente { get; set; }
    public EstadoCurso Estado { get; set; }
    [Column("Horas_semanales")]
    public int HorasSemanales { get; init; }

    /*   
    public Curso(string nombre, DateOnly fechaInicio, DateOnly fechaFinalizacion, int docenteId)
    {
        // Tomo la fecha del sistema y la convierto a long y luego a string
        Id = DateTime.Now.Ticks.ToString();

        Nombre = nombre;
        FechaInicio = fechaInicio;
        FechaFinalizacion = fechaFinalizacion;
        DocenteId = docenteId;
        //Defino un estado por defecto
        Estado = EstadoCurso.Abierto;
        // Tomo la fecha del sistema
        FechaCreacion = DateTime.Now;
    }

    */

    

}

using System.ComponentModel.DataAnnotations.Schema;

namespace CursoCDI.Domain.Entities;

[Table("docentes")]
public class Docente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }

    [Column("fecha_contratacion")]
    public DateOnly FechaContratacion { get; set; }
    public string Area { get; set; }
    public bool Activo { get; set; } = true;  // Valor por defecto en true
}


using CursoCDI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CursoCDI.Persistence.DbContexts;

public class CursosDbContext(DbContextOptions<CursosDbContext> options) : DbContext(options)
{
    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Docente> Docentes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        /*
         Evitar error 

        System.InvalidCastException: 'Unable to cast object of type 'System.DateTime' to type 'System.DateOnly'.'

         */

        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            d => d.ToDateTime(TimeOnly.MinValue),
            d => DateOnly.FromDateTime(d)
        );

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.Property(e => e.FechaInicio).HasConversion(dateOnlyConverter);
            entity.Property(e => e.FechaFinalizacion).HasConversion(dateOnlyConverter);

        });
    }

}

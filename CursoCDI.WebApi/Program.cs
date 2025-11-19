using CursoCDI.Application.Options;
using CursoCDI.Application.Services;
using CursoCDI.Persistence.DbContexts;
using CursoCDI.Persistence.Repositories;
using CursoCDI.Persistence.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registro mis dependencias en el contenedor de dependencias
var connectionString = builder.Configuration.GetConnectionString("cursos_cdi");

builder.Services.AddDbContext<CursosDbContext>(opt=>opt.UseMySQL(connectionString));

builder.Services.Configure<NotasOption>(builder.Configuration.GetSection("NotasOption"));

//builder.Services.AddScoped<ICursoRepository,CursoAdoRepository>();
builder.Services.AddScoped<ICursoRepository, CursoEfcRepository>();
builder.Services.AddScoped<CursoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();

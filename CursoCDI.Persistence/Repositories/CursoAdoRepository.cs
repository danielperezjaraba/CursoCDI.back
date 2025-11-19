using CursoCDI.Domain.Dtos.Requests;
using CursoCDI.Domain.Dtos.Responses;
using CursoCDI.Domain.Entities;
using CursoCDI.Domain.Enums;
using CursoCDI.Persistence.Repositories.Interface;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;


namespace CursoCDI.Persistence.Repositories;

public class CursoAdoRepository : ICursoRepository
{

    private readonly string _connectionString;

    public CursoAdoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("cursos_cdi");
    }


    private string Sql { get; set; }

    private const string FORMATO_FECHA = "yyyy-MM-dd";


    public bool  ValidarCreacionCurso(string nombreCurso) {

        using(var conexion = new MySqlConnection(_connectionString) ){
        
            conexion.Open();

            Sql = @"SELECT COUNT(id) 
                    FROM cursos 
                    WHERE nombre =@nombre  AND estado = 1";


            using (var cmd = new MySqlCommand(Sql,conexion)) {

                cmd.Parameters.AddWithValue("@nombre", nombreCurso);

                 return (long)cmd.ExecuteScalar()==0;

            }
        }

    }

    public bool Borrar(string id)
    {
        //1 Establecer conexión

        using (var conexion = new MySqlConnection(_connectionString))
        {

            //2 Abrir la conexión
            conexion.Open();
            //3 El comando SQL
            Sql = "DELETE FROM cursos WHERE id = @id";

            //4 Crear comando
            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                //5 Remplazar valores
                cmd.Parameters.AddWithValue("@id", id);

                Sql = string.Empty;
                //06 Ejecurar
                return cmd.ExecuteNonQuery() > 0;
        

            }

        }

    }

    public bool Actualizar(string id,ActualizarCursoRequest request )
    {
        //1 Establecer conexión

        using (var conexion = new MySqlConnection(_connectionString))
        {

            //2 Abrir la conexión
            conexion.Open();
            //3 El comando SQL
            Sql = @"UPDATE cursos
                   SET estado = @estado, docente_id =@docenteId 
                   WHERE id =@id";

            //4 Crear comando
            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                //5 Remplazar valores
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@docenteId", request.DocenteId);
                cmd.Parameters.AddWithValue("@estado", request.Estado);


                //06 Ejecurar
                return cmd.ExecuteNonQuery() > 0;
  

            }

        }

    }


    public void Crear(Curso curso)
    {
        //1 Establecer conexión

        using (var conexion = new MySqlConnection(_connectionString))
        {

            //2 Abrir la conexión
            conexion.Open();
            //3 El comando SQL
            Sql = @"INSERT INTO cursos VALUES(@id,@nombre,@fechaInicio,@fechaFinalizacion,@fechaCreacion,@docenteId,@estado,@horasSemanales)";

            //4 Crear comando
            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                //5 Remplazar valores
                cmd.Parameters.AddWithValue("@id", curso.Id);
                cmd.Parameters.AddWithValue("@nombre", curso.Nombre);
                cmd.Parameters.AddWithValue("@fechaInicio", curso.FechaInicio.ToString(FORMATO_FECHA));
                cmd.Parameters.AddWithValue("@fechaFinalizacion", curso.FechaFinalizacion.ToString(FORMATO_FECHA));
                cmd.Parameters.AddWithValue("@fechaCreacion", curso.FechaCreacion);
                cmd.Parameters.AddWithValue("@docenteId", curso.DocenteId);
                cmd.Parameters.AddWithValue("@estado", curso.Estado);
                cmd.Parameters.AddWithValue("@horasSemanales", 4);

                //06 Ejecurar
                cmd.ExecuteNonQuery();


            }

        }

    }

    public ConsultarCursoResponse? Consultar(string id)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();
            Sql = @"SELECT c.id,
	                       c.nombre,
                           c.fecha_inicio,
                           c.fecha_finalizacion,
                           c.estado,
                           concat(d.nombre,' ' ,d.apellido) as docente_nombre
                    FROM cursos c 
	                    INNER JOIN docentes d ON c.docente_id = d.id 
                    WHERE c.id =@id";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {

                    return null;
                }

                reader.Read();
                //Mapeo de la tabla al objeto


                var nombre = reader["nombre"].ToString();
                var docenteNombre = reader["docente_nombre"].ToString();
                var fechaInicio = DateOnly.FromDateTime(Convert.ToDateTime(reader["fecha_inicio"]));
                var fechaFinalizacion = DateOnly.FromDateTime(Convert.ToDateTime(reader["fecha_finalizacion"]));
                var estado = (EstadoCurso)Convert.ToInt32(reader["estado"]);


                return new ConsultarCursoResponse(

                    id,
                    nombre,
                    fechaInicio,
                    fechaFinalizacion,
                    estado,
                    docenteNombre
                );

            }

        }
    }


    public IEnumerable<ConsultarCursoResponse> Consultar(ConsultarCursoRequest consltarCurso)
    {
        var condiciones = new List<string>();
        var parametros = new List<MySqlParameter>();
        var cursos = new List<ConsultarCursoResponse>();

        using (var con = new MySqlConnection(_connectionString))
        {
            con.Open();
            Sql = @"SELECT c.*, concat(d.Nombre,' ',d.apellido) AS docente_nombre 
                    FROM cursos c 
                    JOIN docentes d ON c.docente_id = d.id
                    WHERE 1 = 1";

            if (consltarCurso.Estado is not null)
            {
                condiciones.Add("estado = @estado");
                parametros.Add(new MySqlParameter("@estado", consltarCurso.Estado));
            }
            if (!string.IsNullOrWhiteSpace(consltarCurso.Nombre))
            {
                condiciones.Add("nombre LIKE @nombre");
                parametros.Add(new MySqlParameter("@nombre", $"%{consltarCurso.Nombre}%"));
            }
            if (condiciones.Count != 0)
            {
                Sql += " AND " + string.Join(" AND ", condiciones);
            }

            using (var cmd = new MySqlCommand(Sql, con))
            {
                cmd.Parameters.AddRange(parametros.ToArray());
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader["id"].ToString();
                    var nombre = reader["nombre"].ToString();
                    var fechaInicio = DateOnly.FromDateTime(Convert.ToDateTime(reader["fecha_inicio"]));
                    var fechaFinalizacion = DateOnly.FromDateTime(Convert.ToDateTime(reader["fecha_finalizacion"]));
                    var fechaCreacion = Convert.ToDateTime(reader["fecha_finalizacion"]);
                    var docente_id = Convert.ToInt32(reader["docente_id"]);
                    var estado = (EstadoCurso)Convert.ToInt32(reader["estado"]);
                    var horasSemanales = Convert.ToInt32(reader["horas_semanales"]);
                    var docenteNombre = reader["docente_nombre"].ToString();

                    var curso = new ConsultarCursoResponse(

                                id,
                                nombre,
                                fechaInicio,
                                fechaFinalizacion,
                                estado,
                                docenteNombre
                            );

                    cursos.Add(curso);      
                }       
            }
            return cursos;
        }
    }

}

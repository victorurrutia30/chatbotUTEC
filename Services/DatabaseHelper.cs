using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ChatbotUTEC.Services
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<string> GetHorariosPorCarnet(string carnet)
        {
            var resultados = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var query = @"
                SELECT M.Nombre, H.Dia, H.HoraInicio, H.HoraFin, H.Aula 
                FROM Horario H 
                JOIN Materia M ON H.CodigoMateriaID = M.CodigoMateriaID 
                WHERE H.CarnetID = @Carnet";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Carnet", carnet);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var linea = $"{reader["Nombre"]}: {reader["Dia"]} de {reader["HoraInicio"]} a {reader["HoraFin"]} en {reader["Aula"]}";
                resultados.Add(linea);
            }

            return resultados;
        }

        public List<string> GetHorarioParcial(string materia)
        {
            var resultados = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var query = @"
                SELECT HP.Diainicio, HP.Diafin, HP.HoraInicio, HP.HoraFin 
                FROM Horarioparciales HP 
                JOIN Materia M ON HP.CodigoMateriaID = M.CodigoMateriaID 
                WHERE M.Nombre LIKE @Materia";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Materia", $"%{materia}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var info = $"Parcial: del {reader["Diainicio"]} al {reader["Diafin"]}, de {reader["HoraInicio"]} a {reader["HoraFin"]}";
                resultados.Add(info);
            }

            return resultados;
        }

        public List<string> GetDocentesPorFacultad(string facultad)
        {
            var resultados = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var query = @"
                SELECT D.Nombre, D.Apellido, D.Correo 
                FROM Docente D 
                JOIN Facultad F ON D.CodigoFacultadID = F.CodigoFacultadID 
                WHERE F.Nombre LIKE @Facultad";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Facultad", $"%{facultad}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var info = $"Docente: {reader["Nombre"]} {reader["Apellido"]}, Correo: {reader["Correo"]}";
                resultados.Add(info);
            }

            return resultados;
        }

        public List<string> GetTramites(string carnet)
        {
            var resultados = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var query = @"SELECT Tipo FROM Tramites WHERE CarnetID = @Carnet";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Carnet", carnet);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                resultados.Add($"Trámite: {reader["Tipo"]}");
            }

            return resultados;
        }

        /// <summary>
        /// Inserta en la tabla Interaccion los datos de cada consulta/respuesta del bot.
        /// </summary>
        public void InsertarInteraccion(
            string usuarioId,
            string queryText,
            string intent,
            string entitiesJson,
            int responseTimeMs)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("sp_InsertarInteraccion", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
            cmd.Parameters.AddWithValue("@QueryText", queryText);
            cmd.Parameters.AddWithValue("@Intent", intent);
            cmd.Parameters.AddWithValue("@Entities", entitiesJson);
            cmd.Parameters.AddWithValue("@ResponseTimeMs", responseTimeMs);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Ejecuta cualquier consulta SQL y devuelve un DataTable.
        /// </summary>
        public DataTable EjecutarConsultaATabla(string sql)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
    }
}

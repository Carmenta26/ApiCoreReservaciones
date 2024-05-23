using System.Data;
using System.Data.SqlClient;
using WebApplication1.Objects;
using WebApplication1.Services;

namespace WebApplication1;

public class RecervationController
{
    string connectionString = DatabaseSettings.ConnectionString;

    public void AgregarRecervacion(string nombreCliente, string contacto, string fecha, string hora,
        string tipoServicio, string duracionServicio, string requisitosEspeciales)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string storedProcedure = "AddReservation";

            // Crear un SqlCommand para el procedimiento almacenado
            SqlCommand command = new SqlCommand(storedProcedure, connection);

            // Indicar que el comando utilizará un procedimiento almacenado
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Agregar los parámetros al comando
            command.Parameters.AddWithValue("@NombreCliente", nombreCliente);
            command.Parameters.AddWithValue("@Contacto", contacto);
            command.Parameters.AddWithValue("@Fecha", fecha);
            command.Parameters.AddWithValue("@Hora", hora);
            command.Parameters.AddWithValue("@TipoServicio", tipoServicio);
            command.Parameters.AddWithValue("@DuracionServicio", duracionServicio);
            command.Parameters.AddWithValue("@RequisitosEspeciales", requisitosEspeciales);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public void DeleteReservation(int reservacionID)
    {
        string
            connectionString =
                DatabaseSettings
                    .ConnectionString; // Asegúrate de tener la cadena de conexión configurada adecuadamente.

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string storedProcedure = "DeleteReservation";

            // Crear un SqlCommand para el procedimiento almacenado
            SqlCommand command = new SqlCommand(storedProcedure, connection);

            // Indicar que el comando utilizará un procedimiento almacenado
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Agregar los parámetros al comando
            command.Parameters.AddWithValue("@ReservacionID", reservacionID);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }


    public List<Reservacion> ObtenerReservaciones()
    {
        List<Reservacion> reservaciones = new List<Reservacion>();

        // Nombre del procedimiento almacenado
        string storedProcedureName = "ObtenerReservaciones";

        string connectionString = DatabaseSettings.ConnectionString;
        // Crear una conexión a la base de datos
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear un comando para ejecutar el procedimiento almacenado
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    // Especificar que se está llamando a un procedimiento almacenado
                    command.CommandType = CommandType.StoredProcedure;

                    // Ejecutar el comando y obtener un lector de datos
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Leer los datos
                        while (reader.Read())
                        {
                            Reservacion reservacion = new Reservacion
                            {
                                ReservacionID = reader.GetInt32(0),
                                NombreCliente = reader.GetString(1),
                                Contacto = reader.GetString(2),
                                Fecha = reader.GetDateTime(3)
                                    .ToString("yyyy-MM-dd"), // Convertir a cadena de texto si es necesario
                                Hora = reader.GetTimeSpan(4).ToString(), // Convertir a cadena de texto si es necesario
                                TipoServicio = reader.GetString(5),
                                DuracionServicio = reader.GetString(6),
                                RequisitosEspeciales = reader.IsDBNull(7) ? null : reader.GetString(7)
                            };

                            // Agregar la reservación a la lista
                            reservaciones.Add(reservacion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores (lanzar una excepción adecuada para ser manejada en la capa superior)
                throw new Exception("Error al obtener las reservaciones", ex);
            }
        }

        return reservaciones;
    }


    public async Task UpdateReservation(Reservacion reservation)
    {
        string connectionString = DatabaseSettings.ConnectionString;
        string storedProcedureName = "ActualizarReservacion";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@ReservacionID", reservation.ReservacionID);
                    command.Parameters.AddWithValue("@NombreCliente", reservation.NombreCliente);
                    command.Parameters.AddWithValue("@Contacto", reservation.Contacto);
                    command.Parameters.AddWithValue("@Fecha", reservation.Fecha);
                    command.Parameters.AddWithValue("@Hora", reservation.Hora);
                    command.Parameters.AddWithValue("@TipoServicio", reservation.TipoServicio);
                    command.Parameters.AddWithValue("@DuracionServicio", reservation.DuracionServicio);
                    command.Parameters.AddWithValue("@RequisitosEspeciales", reservation.RequisitosEspeciales);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la reservación", ex);
            }
        }
    }
}
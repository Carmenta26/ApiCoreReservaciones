using System.Data.SqlClient;
using WebApplication1.Objects;
using WebApplication1.Services;

namespace WebApplication1;

public class UserController
{
    string connectionString = DatabaseSettings.ConnectionString;

    public void InsertUser(string firstName, string lastName, string phone, int age, string correo, string contraseña)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string storedProcedure = "InsertUser";
            SqlCommand command = new SqlCommand(storedProcedure, connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@Phone", phone);
            command.Parameters.AddWithValue("@Age", age);
            command.Parameters.AddWithValue("@IsActive", true);
            command.Parameters.AddWithValue("@Correo", correo);
            command.Parameters.AddWithValue("@Contraseña", contraseña);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }


    public User GetUserById(int userId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserID = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Phone = reader.GetString(3),
                    Age = reader.GetInt32(4),
                    IsActive = reader.GetBoolean(5)
                };
            }

            return null;
        }
    }

    public void DeactivateUser(int userId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // Nombre del procedimiento almacenado
            string storedProcedure = "DeactivateUser";

            // Crear un SqlCommand para el procedimiento almacenado
            SqlCommand command = new SqlCommand(storedProcedure, connection);

            // Indicar que el comando utilizará un procedimiento almacenado
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Agregar los parámetros al comando
            command.Parameters.AddWithValue("@UserID", userId);

            // Abrir la conexión y ejecutar el comando
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public List<User> GetActiveUsers()
    {
        var users = new List<User>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Users WHERE IsActive = 1";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    UserID = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Phone = reader.GetString(3),
                    Age = reader.GetInt32(4),
                    IsActive = reader.GetBoolean(5)
                });
            }
        }

        return users;
    }

    public User loginUser(string email, string password, out string errorMessage)
    {
        using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
        {
            string storedProcedure = "LoginUser";

            SqlCommand command = new SqlCommand(storedProcedure, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Correo", email);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                // Si el usuario existe, verificar la contraseña
                string storedPassword = reader.GetString(reader.GetOrdinal("Contraseña"));
                if (storedPassword == password)
                {
                    errorMessage = null;
                    return new User
                    {
                        UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Age = reader.GetInt32(reader.GetOrdinal("Age")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        Correo = reader.GetString(reader.GetOrdinal("Correo")),
                        Contraseña = storedPassword
                    };
                }
                else
                {
                    // Contraseña incorrecta
                    errorMessage = "Contraseña incorrecta.";
                    return null;
                }
            }
            else
            {
                // Si no se encuentra el usuario, devolver null
                errorMessage = "Correo no encontrado.";
                return null;
            }
        }
    }
}
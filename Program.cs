using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using WebApplication1;
using WebApplication1.Objects;
using WebApplication1.Requests;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

var userController = new UserController();
var recervation = new RecervationController();


app.MapPost("/insertuser", async (HttpContext httpContext, [FromBody] User userRequest) =>
    {
        try
        {
            userController.InsertUser(userRequest.FirstName, userRequest.LastName, userRequest.Phone, userRequest.Age,
                userRequest.Correo, userRequest.Contraseña);
            return Results.Ok(new { Message = "User inserted successfully." });
        }
        catch (SqlException ex)
        {
            string errorMessage =
                $"SQL Error: {ex.Message}. Procedure or function InsertUser has too many arguments specified.";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Database Error");
        }
        catch (Exception ex)
        {
            string errorMessage = $"Internal Server Error: {ex.Message}";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Internal Server Error");
        }
    })
    .WithName("InsertUser")
    .WithOpenApi();


// Endpoint para obtener usuarios activos
app.MapGet("/activeusers", () =>
    {
        var users = userController.GetActiveUsers();
        return Results.Ok(users);
    })
    .WithName("GetActiveUsers")
    .WithOpenApi();


// Endpoint para obtener un usuario por ID
app.MapGet("/getuser/{id}", (int id) =>
    {
        var user = userController.GetUserById(id);
        return user != null ? Results.Ok(user) : Results.NotFound();
    })
    .WithName("GetUserById")
    .WithOpenApi();


app.MapPut("/deactivateuser/{id}", (int id) =>
    {
        userController.DeactivateUser(id);
        return Results.Ok();
    })
    .WithName("DeactivateUser")
    .WithOpenApi();


app.MapPost("/login", async (HttpContext httpContext, [FromBody] UserLoginRequest loginRequest) =>
    {
        try
        {
            string errorMessage;
            var user = userController.loginUser(loginRequest.Email, loginRequest.Password, out errorMessage);
            if (user != null)
            {
                return Results.Ok(user);
            }
            else if (errorMessage == "Correo no encontrado.")
            {
                return Results.Json(new { Message = "Correo no encontrado." }, statusCode: 404);
            }
            else if (errorMessage == "Contraseña incorrecta.")
            {
                return Results.Json(new { Message = "Contraseña incorrecta." }, statusCode: 401);
            }
            else
            {
                return Results.Unauthorized();
            }
        }
        catch (SqlException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500, title: "Database Error");
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500, title: "Internal Server Error");
        }
    })
    .WithName("LoginUser")
    .WithOpenApi();


app.MapPost("/addreserv", async (HttpContext httpContext, [FromBody] ReservacionRequest eventoRequest) =>
    {
        try
        {
            recervation.AgregarRecervacion(
                eventoRequest.NombreCliente,
                eventoRequest.Contacto,
                eventoRequest.Fecha,
                eventoRequest.Hora,
                eventoRequest.TipoServicio,
                eventoRequest.DuracionServicio,
                eventoRequest.RequisitosEspeciales
            );
            return Results.Ok(new { Message = "Evento agregado exitosamente." });
        }
        catch (SqlException ex)
        {
            string errorMessage = $"Error SQL: {ex.Message}.";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error de Base de Datos");
        }
        catch (Exception ex)
        {
            string errorMessage = $"Error interno del servidor: {ex.Message}";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error Interno del Servidor");
        }
    })
    .WithName("AddReservation")
    .WithOpenApi();

app.MapDelete("/deletereservation/{reservacionID:int}", async (int reservacionID) =>
    {
        try
        {
            recervation.DeleteReservation(reservacionID);
            return Results.Ok(new { Message = "Reserva eliminada exitosamente." });
        }
        catch (SqlException ex)
        {
            string errorMessage = $"Error SQL: {ex.Message}.";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error de Base de Datos");
        }
        catch (Exception ex)
        {
            string errorMessage = $"Error interno del servidor: {ex.Message}";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error Interno del Servidor");
        }
    })
    .WithName("DeleteReservation")
    .WithOpenApi();


app.MapGet("/reservations", async () =>
    {
        try
        {
            // Llama al método para obtener todas las reservaciones
            var reservaciones = recervation.ObtenerReservaciones();

            // Retorna las reservaciones obtenidas como resultado
            return Results.Ok(reservaciones);
        }
        catch (SqlException ex)
        {
            // Manejo de errores específicos de SQL Server
            string errorMessage = $"Error SQL: {ex.Message}.";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error de Base de Datos");
        }
        catch (Exception ex)
        {
            // Manejo de otros errores
            string errorMessage = $"Error interno del servidor: {ex.Message}";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error Interno del Servidor");
        }
    })
    .WithName("ObtenerReservaciones") // Asigna un nombre al endpoint
    .WithOpenApi(); // Genera la documentación OpenAPI para el endpoint


app.MapPut("/updatereservation/{id}", async (int id, Reservacion reservation) =>
    {
        try
        {
            // Asigna el ID de la reserva desde la ruta al objeto Reservation
            reservation.ReservacionID = id;
        
            // Llama al método para actualizar la reserva
            await recervation.UpdateReservation(reservation);

            // Retorna un mensaje de éxito
            return Results.Ok("Reserva actualizada correctamente");
        }
        catch (SqlException ex)
        {
            // Manejo de errores específicos de SQL Server
            string errorMessage = $"Error SQL: {ex.Message}.";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error de Base de Datos");
        }
        catch (Exception ex)
        {
            // Manejo de otros errores
            string errorMessage = $"Error interno del servidor: {ex.Message}";
            return Results.Problem(detail: errorMessage, statusCode: 500, title: "Error Interno del Servidor");
        }
    })
    .WithName("ActualizarReserva") // Asigna un nombre al endpoint
    .WithOpenApi(); // Genera la documentación OpenAPI para el endpoint


app.Run();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
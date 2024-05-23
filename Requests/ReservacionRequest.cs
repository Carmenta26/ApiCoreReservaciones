namespace WebApplication1.Requests;

public class ReservacionRequest
{
    public string NombreCliente { get; set; }
    public string Contacto { get; set; }
    public string Fecha { get; set; }
    public string Hora { get; set; }
    public string TipoServicio { get; set; }
    public string DuracionServicio { get; set; }
    public string RequisitosEspeciales { get; set; }
}
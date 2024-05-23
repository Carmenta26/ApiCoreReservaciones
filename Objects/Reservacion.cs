namespace WebApplication1.Objects;

public class Reservacion
{
    public int ReservacionID { get; set; }
    public string NombreCliente { get; set; }
    public string Contacto { get; set; }
    public string Fecha { get; set; }
    public string Hora { get; set; }
    public string TipoServicio { get; set; }
    public string DuracionServicio { get; set; }
    public string RequisitosEspeciales { get; set; }
}
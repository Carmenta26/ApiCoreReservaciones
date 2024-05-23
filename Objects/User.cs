namespace WebApplication1.Objects;

public class User
{
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public int Age { get; set; }
    public bool IsActive { get; set; }
    
    public string Correo { get; set; }
    
    public string Contraseña { get; set; }
}
namespace MarmolesOeste.Web.Data.Entities;

// Ficha de datos del empleado (nombre, contacto, puesto), separada a propósito de
// ApplicationUser/Identity: esto es un registro de negocio para asignar tareas y
// llevar sus datos, no una cuenta de acceso al sistema. La gestión de cuentas de
// login (quién puede entrar a la app) es un tema aparte, todavía pendiente.
public class Empleado : ISoftDelete
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Puesto { get; set; }
    public DateTime? FechaBaja { get; set; }

    public bool Activo => FechaBaja is null;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}

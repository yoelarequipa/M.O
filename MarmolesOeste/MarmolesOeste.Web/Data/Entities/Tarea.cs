using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

// Tarea suelta del día a día del taller, asignable a un Empleado y, opcionalmente,
// ligada a una venta puntual (ej. "Pulir mesada" de la venta Nº 10).
public class Tarea
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }

    public int? EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }

    public int? VentaId { get; set; }
    public Venta? Venta { get; set; }

    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    public DateTime? FechaCompletada { get; set; }

    // Instante real en que se cargó la tarea (no confundir con Fecha, que es el día
    // calendario para el que está programada). Se usa en el feed de "Última
    // actividad" del Home.
    public DateTime FechaCreacion { get; set; }
}

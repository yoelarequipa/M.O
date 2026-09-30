using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class Entrega
{
    public int Id { get; set; }

    public int VentaId { get; set; }
    public Venta Venta { get; set; } = null!;

    public DateTime FechaProgramada { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public EstadoEntrega Estado { get; set; } = EstadoEntrega.Programada;

    // Momento real en que se marcó como entregada (no la fecha programada). Se usa
    // para ordenar la lista por "última entrega hecha", no por cuándo se planeó.
    public DateTime? FechaEntregada { get; set; }
}

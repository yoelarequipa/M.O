using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class OrdenProduccion
{
    public int Id { get; set; }

    public int VentaId { get; set; }
    public Venta Venta { get; set; } = null!;

    public EstadoOrdenProduccion Estado { get; set; } = EstadoOrdenProduccion.Pendiente;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

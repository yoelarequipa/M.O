using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class Presupuesto
{
    public int Id { get; set; }
    public int NumeroCorrelativo { get; set; }

    // Nulo mientras el presupuesto es solo una consulta sin comprometerse: no toda
    // visita termina en venta, y pedir los datos del cliente antes de tiempo genera
    // fricción o clientes cargados a las apuradas. NombreReferencia sirve para
    // ubicar el presupuesto en la lista mientras tanto (ej. "Sra. Pérez - mostrador").
    // Generar una Venta exige tener el Cliente asignado.
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public string? NombreReferencia { get; set; }

    public DateTime Fecha { get; set; }
    public int DiasValidez { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public EstadoPresupuesto Estado { get; set; } = EstadoPresupuesto.Borrador;

    public decimal Subtotal { get; set; }
    public decimal DescuentoPorcentaje { get; set; }
    public decimal Descuento { get; set; }
    public decimal IvaPorcentaje { get; set; } = 21m;
    public decimal Iva { get; set; }
    public decimal Total { get; set; }

    public ICollection<PresupuestoPieza> Piezas { get; set; } = new List<PresupuestoPieza>();
    public ICollection<PresupuestoAdicional> Adicionales { get; set; } = new List<PresupuestoAdicional>();
    public Venta? Venta { get; set; }
}

namespace MarmolesOeste.Web.Data.Entities;

public class Venta
{
    public int Id { get; set; }

    public int PresupuestoId { get; set; }
    public Presupuesto Presupuesto { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    public OrdenProduccion? OrdenProduccion { get; set; }
    public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}

using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class Adicional : ISoftDelete, IAuditableActualizacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public UnidadMedidaAdicional UnidadMedida { get; set; }
    public DateTime? FechaBaja { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    public string? MotivoUltimaActualizacion { get; set; }

    public bool Activo => FechaBaja is null;

    public ICollection<PresupuestoAdicional> PresupuestoAdicionales { get; set; } = new List<PresupuestoAdicional>();
}

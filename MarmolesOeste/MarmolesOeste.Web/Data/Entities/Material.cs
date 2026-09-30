using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class Material : ISoftDelete, IAuditableActualizacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoMaterial Tipo { get; set; }
    public decimal Espesor { get; set; }
    public decimal PrecioPorM2 { get; set; }
    public decimal PrecioPorMetroLinealZocalo { get; set; }
    public string? ImagenUrl { get; set; }
    public string ColorHex { get; set; } = "#cfe3d8";
    public DateTime? FechaBaja { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    public string? MotivoUltimaActualizacion { get; set; }

    public bool AlertaStockActiva { get; set; }
    public decimal? AlertaStockCantidad { get; set; }

    public bool Activo => FechaBaja is null;

    public ICollection<PresupuestoPieza> PresupuestoPiezas { get; set; } = new List<PresupuestoPieza>();
    public ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
}

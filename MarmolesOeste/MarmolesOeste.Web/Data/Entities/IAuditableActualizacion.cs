namespace MarmolesOeste.Web.Data.Entities;

// Igual que ISoftDelete: entidades de catálogo que cambian poco (precios de
// materiales, terminaciones, adicionales) y donde vale la pena saber cuándo fue
// la última edición y por qué, a diferencia de Cliente que se toca todo el tiempo.
public interface IAuditableActualizacion
{
    DateTime? FechaUltimaActualizacion { get; set; }
    string? MotivoUltimaActualizacion { get; set; }
}

namespace MarmolesOeste.Web.Data.Entities;

// Marca las entidades de catálogo que se dan de baja lógica en vez de borrarse.
// El ApplicationDbContext usa esta interfaz para aplicar un query filter global
// a todas las entidades que la implementan, sin repetir la condición en cada una.
public interface ISoftDelete
{
    DateTime? FechaBaja { get; set; }
}

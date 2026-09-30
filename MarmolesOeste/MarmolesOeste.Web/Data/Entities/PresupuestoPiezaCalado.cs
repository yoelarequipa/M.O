using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

// Corte de bacha o anafe dentro de una pieza, solo para que el dibujo del
// presupuesto sea más real para el cliente. No afecta precios ni stock (eso lo
// sigue cobrando el Adicional "Calado de bacha/anafe" ya existente).
//
// La posición se mide en metros desde dos bordes del tramo al que pertenece:
// - DistanciaLateral: desde el inicio del tramo (a lo largo de su longitud).
// - DistanciaFrontal: desde el borde frontal (exterior) de la mesada hacia el fondo.
public class PresupuestoPiezaCalado
{
    public int Id { get; set; }

    public int PresupuestoPiezaId { get; set; }
    public PresupuestoPieza PresupuestoPieza { get; set; } = null!;

    public TipoCalado Tipo { get; set; }

    // Orden (1-based) del tramo de la pieza al que pertenece este calado.
    public int TramoOrden { get; set; } = 1;

    public decimal Ancho { get; set; }
    public decimal Fondo { get; set; }
    public decimal DistanciaLateral { get; set; }
    public decimal DistanciaFrontal { get; set; }
}

using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarmolesOeste.Web.Data;

// Toda medida lineal (Largo de un tramo, Profundidad de una pieza) se carga en
// metros, no en centímetros, porque los precios de catálogo son por m² y por
// metro lineal. Esto se explica en las pantallas con el label del campo.
public static class PresupuestoCalculos
{
    // Altura estándar asumida para el zócalo, solo para estimar cuánto material de
    // la placa consume (el precio del zócalo sigue siendo por metro lineal, esto no
    // lo toca). 10cm es la altura más común; si en la práctica se corta distinto,
    // esto va a sub o sobreestimar el consumo real de stock.
    public const decimal AlturaZocaloEstimadaM = 0.10m;

    public static decimal LargoTotal(PresupuestoPieza pieza) => pieza.Tramos.Sum(t => t.Largo);

    public static decimal AreaM2(PresupuestoPieza pieza) => LargoTotal(pieza) * pieza.Profundidad;

    public static decimal AreaZocaloEstimada(PresupuestoPieza pieza) =>
        pieza.LlevaZocalo ? LargoTotal(pieza) * AlturaZocaloEstimadaM : 0m;

    public static decimal SubtotalPieza(PresupuestoPieza pieza)
    {
        var largoTotal = LargoTotal(pieza);
        var subtotal = AreaM2(pieza) * pieza.PrecioMaterialCongelado
            + largoTotal * pieza.PrecioTerminacionCongelado;

        if (pieza.LlevaZocalo)
        {
            subtotal += largoTotal * pieza.PrecioZocaloCongelado;
        }

        return subtotal;
    }

    public static decimal SubtotalAdicional(PresupuestoAdicional adicional) =>
        adicional.Cantidad * adicional.PrecioAdicionalCongelado;

    // Recalcula y persiste Subtotal/Descuento/Iva/Total. Se llama después de
    // cualquier alta, baja o edición de piezas/adicionales, y después de tocar
    // los porcentajes de descuento o IVA, para que el presupuesto nunca quede
    // con totales desactualizados en la base.
    public static async Task RecalcularTotalesAsync(ApplicationDbContext db, int presupuestoId)
    {
        var presupuesto = await db.Presupuestos
            .Include(p => p.Piezas).ThenInclude(pz => pz.Tramos)
            .Include(p => p.Adicionales)
            .FirstAsync(p => p.Id == presupuestoId);

        var subtotal = presupuesto.Piezas.Sum(SubtotalPieza) + presupuesto.Adicionales.Sum(SubtotalAdicional);
        var descuento = Math.Round(subtotal * presupuesto.DescuentoPorcentaje / 100m, 2);
        var baseImponible = subtotal - descuento;
        var iva = Math.Round(baseImponible * presupuesto.IvaPorcentaje / 100m, 2);

        presupuesto.Subtotal = subtotal;
        presupuesto.Descuento = descuento;
        presupuesto.Iva = iva;
        presupuesto.Total = baseImponible + iva;

        await db.SaveChangesAsync();
    }
}

using MarmolesOeste.Web.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarmolesOeste.Web.Data;

// Lo pagado de una venta se calcula sumando sus MovimientoCaja, igual que el stock
// se calcula sumando movimientos: no hay un campo "MontoPagado" que se pisa, así se
// puede reconstruir el historial completo de pagos en cualquier momento.
public static class VentaCalculos
{
    public static async Task<decimal> MontoPagadoAsync(ApplicationDbContext db, int ventaId) =>
        await db.MovimientosCaja
            .Where(m => m.VentaId == ventaId)
            .SumAsync(m => m.Tipo == TipoMovimientoCaja.Ingreso ? m.Monto : -m.Monto);

    public static async Task<Dictionary<int, decimal>> MontoPagadoPorVentaAsync(ApplicationDbContext db) =>
        await db.MovimientosCaja
            .Where(m => m.VentaId != null)
            .GroupBy(m => m.VentaId!.Value)
            .Select(g => new
            {
                VentaId = g.Key,
                Pagado = g.Sum(m => m.Tipo == TipoMovimientoCaja.Ingreso ? m.Monto : -m.Monto),
            })
            .ToDictionaryAsync(x => x.VentaId, x => x.Pagado);
}

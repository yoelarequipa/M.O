namespace MarmolesOeste.Web.Data.Enums;

// Identifica de qué tabla viene el DocumentoOrigenId de un MovimientoStock.
// No es una FK real: cada movimiento puede originarse en una tabla distinta
// (compra a proveedor, venta, ajuste manual), y no vale la pena una tabla
// intermedia solo para eso.
public enum TipoDocumentoOrigenStock
{
    Compra = 1,
    Venta = 2,
    AjusteManual = 3,
}

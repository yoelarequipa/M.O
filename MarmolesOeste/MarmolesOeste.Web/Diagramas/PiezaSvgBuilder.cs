using System.Globalization;
using System.Net;
using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Diagramas;

// Datos de un calado (corte de bacha o anafe) desacoplados de la entidad EF, para
// poder generar la vista previa en vivo del diálogo antes de guardar nada en la
// base. Todas las medidas están en metros.
public sealed record CaladoInfo(TipoCalado Tipo, int TramoOrden, decimal Ancho, decimal Fondo, decimal DistanciaLateral, decimal DistanciaFrontal);

// Genera el mismo dibujo esquemático de una pieza (SVG) para dos consumidores: la
// vista previa en vivo del diálogo de piezas (Blazor) y el PDF del presupuesto
// (QuestPDF sabe embeber SVG crudo vía container.Svg(...)). Una sola implementación
// de la geometría evita que el dibujo del diálogo y el del PDF diverjan con el tiempo.
//
// No es un plano técnico. Para "U" con tramos laterales de largo distinto, dibuja
// los dos brazos con la altura del primer tramo (simplificación: una mesada en U
// real casi siempre tiene los brazos iguales); igual muestra el número real cargado
// en la etiqueta, aunque el dibujo no sea exacto en ese caso borde.
//
// Los calados (bacha/anafe) se posicionan en dos ejes relativos al tramo al que
// pertenecen: "lateral" (a lo largo del tramo, desde su inicio) y "frontal" (desde
// el borde exterior/frontal de la mesada hacia el fondo). Como la vista previa se
// actualiza en vivo mientras se cargan los números, el usuario termina de ajustar
// la posición mirando el dibujo, así que no hace falta que estos ejes coincidan
// con una convención real de "adelante/atrás" de cocina.
public static class PiezaSvgBuilder
{
    private const double Escala = 110; // px por metro
    private const double Padding = 45;
    private const double Inset = 10;

    public static string Build(
        FormaPieza forma,
        IReadOnlyList<decimal> tramos,
        decimal profundidad,
        bool llevaZocalo,
        string? colorHex = null,
        IReadOnlyList<CaladoInfo>? calados = null)
    {
        var color = string.IsNullOrWhiteSpace(colorHex) ? "#cfe3d8" : colorHex;
        var profPx = (double)profundidad * Escala;
        var dimensiones = new List<DimLine>();
        var caladosList = calados ?? [];

        var (contorno, zocalo, w, h, caladosRects) = forma switch
        {
            FormaPieza.L when tramos.Count >= 2 => ComponerL(tramos, profPx, llevaZocalo, dimensiones, caladosList),
            FormaPieza.U when tramos.Count >= 3 => ComponerU(tramos, profPx, llevaZocalo, dimensiones, caladosList),
            _ => ComponerRecta(tramos, profPx, profundidad, llevaZocalo, dimensiones, caladosList),
        };

        var viewBox = $"{Inv(-Padding)} {Inv(-Padding)} {Inv(w + Padding * 2)} {Inv(h + Padding * 2)}";

        var zocaloSvg = zocalo is null
            ? string.Empty
            : $"<polyline points=\"{zocalo}\" fill=\"none\" stroke=\"#2f4f3f\" stroke-width=\"1.5\" stroke-dasharray=\"6 4\" />";

        var lineasSvg = string.Concat(dimensiones.Select(BuildDimLinesSvg));

        var etiquetasSvg = string.Concat(dimensiones.Select(d =>
            $"<text x=\"{Inv(d.LabelX)}\" y=\"{Inv(d.LabelY)}\" font-size=\"11\" text-anchor=\"middle\" fill=\"#333\">{WebUtility.HtmlEncode(d.Label)}</text>"));

        var caladosSvg = string.Concat(caladosRects.Select(BuildCaladoSvg));

        return $"""
            <svg viewBox="{viewBox}" xmlns="http://www.w3.org/2000/svg">
                <polygon points="{contorno}" fill="{color}" stroke="#2f4f3f" stroke-width="3" />
                {zocaloSvg}
                {caladosSvg}
                {lineasSvg}
                {etiquetasSvg}
            </svg>
            """;
    }

    private static (string Contorno, string? Zocalo, double W, double H, List<CaladoRect> Calados) ComponerRecta(
        IReadOnlyList<decimal> tramos, double profPx, decimal profundidad, bool llevaZocalo, List<DimLine> dimensiones, IReadOnlyList<CaladoInfo> calados)
    {
        var w = (double)tramos[0] * Escala;
        var h = profPx;

        var contorno = Poly((0, 0), (w, 0), (w, h), (0, h));
        var zocalo = llevaZocalo ? Poly((0, Inset), (w, Inset)) : null;

        dimensiones.Add(new DimLine(0, -15, w, -15, w / 2, -22, $"{tramos[0]:N2} m", false));
        dimensiones.Add(new DimLine(w + 15, 0, w + 15, h, w + 25, h / 2, $"{profundidad:N2} m", true));

        var caladosRects = calados
            .Where(c => c.TramoOrden == 1)
            .Select(c => CaladoHorizontal(c, 0, 0))
            .ToList();

        return (contorno, zocalo, w, h, caladosRects);
    }

    private static (string Contorno, string? Zocalo, double W, double H, List<CaladoRect> Calados) ComponerL(
        IReadOnlyList<decimal> tramos, double profPx, bool llevaZocalo, List<DimLine> dimensiones, IReadOnlyList<CaladoInfo> calados)
    {
        var w = (double)tramos[0] * Escala;
        var hgt = (double)tramos[1] * Escala;
        var p = profPx;

        var contorno = Poly((0, 0), (w, 0), (w, p), (p, p), (p, hgt), (0, hgt));
        var zocalo = llevaZocalo ? Poly((w, Inset), (Inset, Inset), (Inset, hgt)) : null;

        dimensiones.Add(new DimLine(0, -15, w, -15, w / 2, -22, $"{tramos[0]:N2} m", false));
        dimensiones.Add(new DimLine(-15, 0, -15, hgt, -25, hgt / 2, $"{tramos[1]:N2} m", true));
        dimensiones.Add(new DimLine(w + 15, 0, w + 15, p, w + 25, p / 2, $"{profPx / Escala:N2} m", true));

        var caladosRects = new List<CaladoRect>();
        caladosRects.AddRange(calados.Where(c => c.TramoOrden == 1).Select(c => CaladoHorizontal(c, 0, 0)));
        caladosRects.AddRange(calados.Where(c => c.TramoOrden == 2).Select(c => CaladoVertical(c, 0, 0)));

        return (contorno, zocalo, w, hgt, caladosRects);
    }

    private static (string Contorno, string? Zocalo, double W, double H, List<CaladoRect> Calados) ComponerU(
        IReadOnlyList<decimal> tramos, double profPx, bool llevaZocalo, List<DimLine> dimensiones, IReadOnlyList<CaladoInfo> calados)
    {
        var h = (double)tramos[0] * Escala; // altura de ambos brazos (ver nota en el encabezado)
        var w = (double)tramos[1] * Escala; // ancho total (barra inferior)
        var p = profPx;

        var contorno = Poly((0, 0), (p, 0), (p, h - p), (w - p, h - p), (w - p, 0), (w, 0), (w, h), (0, h));
        var zocalo = llevaZocalo
            ? Poly((Inset, 0), (Inset, h - Inset), (w - Inset, h - Inset), (w - Inset, 0))
            : null;

        dimensiones.Add(new DimLine(-15, 0, -15, h, -25, h / 2, $"{tramos[0]:N2} m", true));
        dimensiones.Add(new DimLine(0, h + 15, w, h + 15, w / 2, h + 25, $"{tramos[1]:N2} m", false));
        dimensiones.Add(new DimLine(w + 15, 0, w + 15, h, w + 25, h / 2, $"{tramos[2]:N2} m", true));
        dimensiones.Add(new DimLine(p / 2, -15, p / 2, 0, p / 2, -22, $"{profPx / Escala:N2} m", false));

        var caladosRects = new List<CaladoRect>();
        // Brazo izquierdo: lateral = y (desde arriba), frontal = x (desde el borde izquierdo).
        caladosRects.AddRange(calados.Where(c => c.TramoOrden == 1).Select(c => CaladoVertical(c, 0, 0)));
        // Barra inferior: lateral = x (desde la izquierda), frontal = y desde el borde exterior (y = h).
        caladosRects.AddRange(calados.Where(c => c.TramoOrden == 2).Select(c => CaladoHorizontalInvertida(c, 0, h)));
        // Brazo derecho: lateral = y (desde arriba), frontal = x desde el borde exterior (x = w).
        caladosRects.AddRange(calados.Where(c => c.TramoOrden == 3).Select(c => CaladoVerticalInvertida(c, w, 0)));

        return (contorno, zocalo, w, h, caladosRects);
    }

    // Tramo horizontal: lateral corre en X desde originX, frontal corre en Y desde originY (borde exterior arriba).
    private static CaladoRect CaladoHorizontal(CaladoInfo c, double originX, double originY)
    {
        var x = originX + (double)c.DistanciaLateral * Escala;
        var y = originY + (double)c.DistanciaFrontal * Escala;
        return new CaladoRect(x, y, (double)c.Ancho * Escala, (double)c.Fondo * Escala, c.Tipo);
    }

    // Tramo horizontal cuyo borde exterior (frontal = 0) está abajo (originY), no arriba.
    private static CaladoRect CaladoHorizontalInvertida(CaladoInfo c, double originX, double bordeExteriorY)
    {
        var x = originX + (double)c.DistanciaLateral * Escala;
        var fondoPx = (double)c.Fondo * Escala;
        var y = bordeExteriorY - (double)c.DistanciaFrontal * Escala - fondoPx;
        return new CaladoRect(x, y, (double)c.Ancho * Escala, fondoPx, c.Tipo);
    }

    // Tramo vertical: lateral corre en Y desde originY, frontal corre en X desde originX (borde exterior a la izquierda).
    private static CaladoRect CaladoVertical(CaladoInfo c, double originX, double originY)
    {
        var x = originX + (double)c.DistanciaFrontal * Escala;
        var y = originY + (double)c.DistanciaLateral * Escala;
        return new CaladoRect(x, y, (double)c.Fondo * Escala, (double)c.Ancho * Escala, c.Tipo);
    }

    // Tramo vertical cuyo borde exterior (frontal = 0) está a la derecha (bordeExteriorX), no a la izquierda.
    private static CaladoRect CaladoVerticalInvertida(CaladoInfo c, double bordeExteriorX, double originY)
    {
        var fondoPx = (double)c.Fondo * Escala;
        var x = bordeExteriorX - (double)c.DistanciaFrontal * Escala - fondoPx;
        var y = originY + (double)c.DistanciaLateral * Escala;
        return new CaladoRect(x, y, fondoPx, (double)c.Ancho * Escala, c.Tipo);
    }

    private static string BuildCaladoSvg(CaladoRect r)
    {
        var esBacha = r.Tipo == TipoCalado.Bacha;
        var fill = esBacha ? "#d7ecf7" : "#4a4a4a";
        var stroke = esBacha ? "#4a90b8" : "#222222";
        var rx = esBacha ? 10 : 2;
        var etiqueta = esBacha ? "Bacha" : "Anafe";
        var textColor = esBacha ? "#2f6b8a" : "#f2f2f2";
        var cx = r.X + r.W / 2;
        var cy = r.Y + r.H / 2;

        var rect = $"<rect x=\"{Inv(r.X)}\" y=\"{Inv(r.Y)}\" width=\"{Inv(r.W)}\" height=\"{Inv(r.H)}\" rx=\"{rx}\" fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"1.5\" />";
        var texto = $"<text x=\"{Inv(cx)}\" y=\"{Inv(cy)}\" font-size=\"10\" text-anchor=\"middle\" dominant-baseline=\"middle\" fill=\"{textColor}\">{etiqueta}</text>";

        return rect + texto;
    }

    private static string BuildDimLinesSvg(DimLine d)
    {
        var main = $"<line x1=\"{Inv(d.X1)}\" y1=\"{Inv(d.Y1)}\" x2=\"{Inv(d.X2)}\" y2=\"{Inv(d.Y2)}\" stroke=\"#666\" stroke-width=\"1\" />";

        string tick1, tick2;
        if (d.Vertical)
        {
            tick1 = $"<line x1=\"{Inv(d.X1 - 5)}\" y1=\"{Inv(d.Y1)}\" x2=\"{Inv(d.X1 + 5)}\" y2=\"{Inv(d.Y1)}\" stroke=\"#666\" stroke-width=\"1\" />";
            tick2 = $"<line x1=\"{Inv(d.X2 - 5)}\" y1=\"{Inv(d.Y2)}\" x2=\"{Inv(d.X2 + 5)}\" y2=\"{Inv(d.Y2)}\" stroke=\"#666\" stroke-width=\"1\" />";
        }
        else
        {
            tick1 = $"<line x1=\"{Inv(d.X1)}\" y1=\"{Inv(d.Y1 - 5)}\" x2=\"{Inv(d.X1)}\" y2=\"{Inv(d.Y1 + 5)}\" stroke=\"#666\" stroke-width=\"1\" />";
            tick2 = $"<line x1=\"{Inv(d.X2)}\" y1=\"{Inv(d.Y2 - 5)}\" x2=\"{Inv(d.X2)}\" y2=\"{Inv(d.Y2 + 5)}\" stroke=\"#666\" stroke-width=\"1\" />";
        }

        return main + tick1 + tick2;
    }

    private static string Inv(double value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Poly(params (double X, double Y)[] points) =>
        string.Join(" ", points.Select(p => $"{Inv(p.X)},{Inv(p.Y)}"));

    private sealed record DimLine(double X1, double Y1, double X2, double Y2, double LabelX, double LabelY, string Label, bool Vertical);

    private sealed record CaladoRect(double X, double Y, double W, double H, TipoCalado Tipo);
}

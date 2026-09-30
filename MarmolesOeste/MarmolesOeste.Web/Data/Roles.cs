namespace MarmolesOeste.Web.Data;

// Nombres de rol sin tildes: Identity los usa como claims y como parte de
// nombres de columna/índice, y evitar caracteres especiales ahí es más simple
// que lidiar con problemas de codificación más adelante.
public static class Roles
{
    public const string Dueno = "Dueno";
    public const string Empleado = "Empleado";
}

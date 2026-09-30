# Mármoles Oeste — Sistema de gestión

Sistema de gestión integral para una marmolería/negocio de mesadas de piedra, desarrollado con .NET 10 y Blazor Web App. Cubre el flujo completo del negocio: presupuesto → venta → producción → entrega → facturación, además de stock, caja, clientes y equipo de trabajo.

## Tecnologías

- **.NET 10** / **Blazor Web App** (render mode Interactive Server)
- **MudBlazor** (componentes de UI)
- **PostgreSQL** + **Entity Framework Core** (driver Npgsql)
- **ASP.NET Core Identity** (roles: Dueño / Empleado)
- **QuestPDF** (generación de PDFs: presupuestos, facturación)
- **ClosedXML** (exportación a Excel)

## Funcionalidades principales

- **Catálogo**: materiales, terminaciones de canto y adicionales, con seguimiento de última actualización (fecha y motivo).
- **Presupuestos**: piezas rectas, en L o en U, con un diagrama SVG a escala generado dinámicamente (mismo dibujo en la vista previa en vivo y en el PDF exportado), color según el material elegido, y ubicación configurable de calados de bacha/anafe.
- **Ventas**: conversión de presupuesto a venta, generación automática de orden de producción y descuento de stock, registro de pagos.
- **Producción** y **Entregas** (con calendario visual de entregas programadas).
- **Stock**: movimientos de entrada/salida por material, llegadas de camión programadas, y alertas configurables de stock mínimo por material (visibles en el Home).
- **Caja**: ingresos y egresos.
- **Facturación**: resumen de facturación (hoy/semana/mes), historial de ventas por mes y ranking de materiales más vendidos (gráficos), exportación a Excel/PDF de la facturación de un mes elegido.
- **Clientes**: alta/edición, historial de compras y buscador (nombre, teléfono, email, dirección).
- **Empleados y Tareas**: asignación de tareas con 3 estados (pendiente / en curso / completada), con la regla de que un empleado solo puede tener una tarea "en curso" a la vez.
- **Home**: indicadores del día, calendario de entregas, tareas de hoy, alertas de stock y un feed de "última actividad" con los eventos más recientes de todo el sistema.

## Estructura del proyecto

```
MarmolesOeste.Web/
├── Components/
│   ├── Pages/        # Páginas (una por sección: Ventas, Stock, Facturación, etc.)
│   ├── Dialogs/       # Diálogos de alta/edición (MudDialog)
│   ├── Shared/        # Componentes reutilizables (diagrama de pieza, calendario, etc.)
│   └── Account/       # Páginas de Identity (login, registro, etc.)
├── Data/
│   ├── Entities/       # Entidades de EF Core
│   ├── Configurations/ # Fluent API (IEntityTypeConfiguration por entidad)
│   ├── Enums/
│   ├── Migrations/
│   └── Seed/           # Datos de desarrollo (catálogo inicial, usuarios demo, ventas ficticias)
├── Diagramas/           # Generador del SVG de piezas (compartido entre UI y PDF)
├── Pdf/                 # Documentos QuestPDF y exportador de Excel
└── Program.cs
```

## Cómo correrlo localmente

### Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL corriendo localmente (o accesible por red)
- Herramienta `dotnet-ef` (si no la tenés: `dotnet tool install --global dotnet-ef`)

### Pasos

1. Cloná el repositorio.

2. Configurá la cadena de conexión con [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) (no se commitea, así que hay que cargarla en cada máquina donde corras el proyecto):

   ```bash
   cd MarmolesOeste.Web
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=marmolesoeste;Username=postgres;Password=TU_PASSWORD"
   ```

3. Aplicá las migraciones (esto crea la base de datos si no existe):

   ```bash
   dotnet ef database update
   ```

4. Corré la aplicación:

   ```bash
   dotnet run
   ```

5. Abrí `http://localhost:5036` (o `https://localhost:7053`).

### Usuarios de prueba

En el primer arranque, si la base está vacía, se cargan automáticamente un catálogo inicial (materiales, terminaciones, adicionales) y dos usuarios de desarrollo:

| Rol      | Email                        | Contraseña   |
|----------|-------------------------------|--------------|
| Dueño    | dueno@marmolesoeste.com      | `Dueno123!`  |
| Empleado | empleado@marmolesoeste.com   | `Empleado123!` |

> ⚠️ Son credenciales de desarrollo para poder probar la app localmente, no están pensadas para un entorno de producción.

### Datos de ejemplo para los gráficos

En la sección **Facturación** (solo visible con `ASPNETCORE_ENVIRONMENT=Development`) hay un botón "Generar datos de ejemplo" que crea ventas ficticias de los últimos 12 meses — útil para ver los gráficos con datos sin tener que cargar ventas reales a mano. Todo lo que genera queda atado a clientes marcados `[DEMO]` y se puede borrar por completo con el botón "Borrar datos de ejemplo".

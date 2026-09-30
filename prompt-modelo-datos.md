# Prompt para Claude Code — Paso 1: modelo de datos

## Contexto

Estoy desarrollando un sistema de gestión para una marmolería llamada Mármoles Oeste, ubicada en La Reja, Buenos Aires. El negocio fabrica y coloca mesadas de granito, mármol y cuarzo (bajo mesada, cocinas, baños).

Soy un solo desarrollador y este es mi primer sistema de gestión completo. Explicame las decisiones que tomes, no solo el código.

## Stack

- .NET 9, Blazor Web App con render interactivo del lado del servidor
- PostgreSQL con Entity Framework Core y el driver Npgsql
- MudBlazor para el panel interno, Tailwind CSS para las pantallas de presupuesto
- ASP.NET Core Identity para autenticación, con roles de dueño y empleado
- QuestPDF para generar presupuestos en PDF

## Tarea

Necesito **solamente el modelo de datos y las migraciones**. Todavía no quiero pantallas, componentes ni lógica de interfaz.

Concretamente:

1. La estructura de carpetas del proyecto
2. Las clases de entidad en C#
3. El `DbContext` con las configuraciones usando Fluent API
4. La primera migración generada y aplicada
5. Un seeder con datos de prueba (materiales, terminaciones de canto, adicionales)

## Entidades que necesito

**Material** — cada tipo de piedra que vende. Nombre, tipo (granito, mármol, cuarzo), espesor, precio por m², precio por metro lineal de zócalo, imagen, y si está activo.

**TerminacionCanto** — pulido recto, boleado, media caña, bisel, laminado. Cada una con su precio por metro lineal.

**Adicional** — colocación, flete, medición en obra, calado de bacha, calado de anafe, perforación de grifería. Con precio y unidad de medida.

**Cliente** — nombre, teléfono, email, dirección de la obra.

**Presupuesto** — número correlativo, cliente, fecha, días de validez, fecha de vencimiento, estado, subtotal, descuento, IVA y total.

**PresupuestoPieza** — cada pieza dentro de un presupuesto (mesada de cocina, isla, mesada de baño). Forma (recta, L, U), medidas de cada tramo, profundidad, material, terminación de canto, y si lleva zócalo.

**PresupuestoAdicional** — los adicionales aplicados a un presupuesto.

**MovimientoStock** — entradas y salidas de placas. Material, cantidad, tipo de movimiento, motivo, fecha y referencia al documento que lo originó.

**Venta** — generada cuando se acepta un presupuesto.

**OrdenProduccion** — el trabajo de taller que sale de una venta, con su estado.

**Entrega** — fecha programada, dirección, estado, y a qué venta pertenece.

**MovimientoCaja** — ingresos y egresos, con concepto, medio de pago y fecha.

Si te parece que falta alguna entidad o que alguna sobra, decímelo antes de escribir el código.

## Reglas que tenés que respetar

**Precios congelados.** Cada línea de un presupuesto guarda una copia del nombre y del precio del material vigente ese día, no solo el `MaterialId`. Si el dueño actualiza el precio del granito, los presupuestos viejos no pueden cambiar de total.

**Bajas lógicas.** Nada se borra físicamente. Usá un campo de estado o fecha de baja, y configurá query filters globales en el `DbContext` para que las consultas ignoren los dados de baja por defecto.

**Dinero.** Todos los importes en `decimal`, nunca `double` ni `float`. Configurados en EF Core como `decimal(18,2)`.

**Fechas en UTC.** Npgsql exige UTC en columnas con zona horaria. Definí desde ahora que todo se persiste en UTC y la conversión a hora argentina se hace solo al mostrar.

**Stock como movimientos.** El stock disponible se calcula sumando movimientos, nunca es un número que se pisa. Quiero poder reconstruir el historial completo.

**Estados como enums**, no como strings sueltos. El presupuesto va de borrador a enviado, y de ahí a aceptado, rechazado o vencido.

## Lo que no quiero

- Nada de interfaz de usuario todavía
- No agregues paquetes que no estén en el stack de arriba sin avisarme primero
- No generes código de más "por si acaso"

## Cómo quiero trabajar

Primero mostrame el diseño de las entidades y sus relaciones para que lo revise. Recién cuando lo apruebe, escribí el código.

Cuando termines, decime qué comandos tengo que correr para aplicar la migración y cómo verifico que quedó bien.

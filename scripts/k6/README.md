# Pruebas de carga de Ventas (k6)

Scripts para probar el flujo de **ventas (POS)** del ERP con [k6](https://k6.io).

## Qué prueba

El script cubre los **dos caminos reales de descuento de inventario** que tiene
`SaleService` / `StockHelper`:

| Escenario | Camino | Endpoint(s) |
|---|---|---|
| `crear_noserial` | Producto **sin serial**: descuenta `WarehouseStock.CurrentStock` y consume capas **FIFO** de entradas (`Movement.RemainingQuantity`), más antiguas primero. | `POST /api/v1/pos/ventas` |
| `crear_serial` | Producto que **requiere serial**: pide la unidad disponible **más antigua** (FIFO estricto) y la vende por `SerialNumber` con `quantity=1`. | `GET /api/v1/dev/ventas/next-serial` + `POST /api/v1/pos/ventas` |
| `listar_ventas` | Lectura paginada. | `GET /api/v1/pos/ventas` |

> Con `Parametros.ForzarFifoEstricto = true` (valor del seed), vender un serial
> **fuera de orden** devuelve `400`. Por eso `crear_serial` corre con **1 VU**
> (secuencial): el FIFO estricto obliga a despachar siempre el más antiguo.

Los endpoints viven en `ERP.DATASET/Api/VentasApiExtensions.cs` (registrados en `Program.cs`).

## Requisitos

1. **App compilada con estos cambios y corriendo en Development** (los endpoints `/api/v1/dev/*` solo existen ahí):
   ```bash
   dotnet run --project ERP.DATASET/ERP.DATASET.csproj
   ```
   Si la tenías corriendo, **reiníciala** (Rider incluido): el proceso antiguo no tiene `next-serial` ni el nuevo `prepare`.
2. **PostgreSQL** accesible con la cadena de `appsettings.json`.
3. **k6** instalado (`k6 version`). El k6 real es un binario de Go, **no** el paquete npm `k6` (ese es un dummy de autocompletado).

## Uso

```bash
# Defaults: http://localhost:8080, 5 ventas/s sin serial, 30 s
k6 run scripts/k6/ventas.js

# Apuntando a "dotnet run" (perfil http)
BASE_URL=http://localhost:5264 k6 run scripts/k6/ventas.js

# Simular N cajeros concurrentes (cada uno vende y espera THINK segundos)
BASE_URL=http://localhost:5264 CAJEROS=5 THINK=1 DURATION=30s k6 run scripts/k6/ventas.js

# Llegada constante a una tasa concreta
BASE_URL=http://localhost:5264 RATE=20 DURATION=1m SERIAL_UNITS=5000 k6 run scripts/k6/ventas.js
```

### Modos de carga

- **Cajeros** (`CAJEROS > 0`): `N` usuarios concurrentes. Cada VU hace una venta y
  luego `sleep(THINK)`. Modela *"5 cajeros vendiendo a la vez"*. Con `THINK=0` mide
  la concurrencia máxima (cada cajero vende sin pausa).
- **Tasa** (por defecto): llegada constante de `RATE` ventas/s (`constant-arrival-rate`).

> ⚠️ **Ejecuta la app en modo RUN, no Debug.** Con el debugger de Rider enganchado
> el endpoint de venta se vuelve ~**270×** más lento (≈ 10 s vs ≈ 15 ms por venta) y
> los resultados no reflejan el rendimiento real del sistema.

### Variables de entorno

| Variable | Default | Descripción |
|---|---|---|
| `BASE_URL` | `http://localhost:8080` | URL base del ERP |
| `RATE` | `5` | Ventas/s del escenario **sin serial** (modo tasa) |
| `CAJEROS` | `0` | Si `>0`, activa el modo cajeros con esa cantidad de VUs concurrentes |
| `THINK` | `1` | Segundos que "piensa" cada cajero entre ventas (modo cajeros) |
| `MAX_VUS` | `max(5, min(20, RATE*2))` | Tope de VUs del modo tasa (si la app se degrada, k6 descarta iteraciones) |
| `HTTP_TIMEOUT` | `60s` | Timeout por petición. Bajarlo aborta clientes a mitad de venta (para diagnosticar transacciones) |
| `DURATION` | `30s` | Duración de los escenarios |
| `SERIAL_VUS` | `1` | VUs del escenario serializado (subirlo rompe FIFO estricto) |
| `LIST_VUS` | `5` | VUs del listado |
| `QTY` | `1` | Cantidad por línea (sin serial) |
| `STOCK` | `1000000` | Stock agregado que se prepara para la variante sin serial |
| `SERIAL_UNITS` | `2000` | Unidades físicas FIFO que se preparan para la variante serializada |
| `USER_ID` / `USER_ROLE` | `1` / `Admin` | Identidad del token (auditoría) |

### Autenticación

Por defecto el script **firma su propio JWT** (HS256) con la misma clave de
`appsettings.json` (`Jwt:Key`) y lo envía en la cookie `auth_token` (así lee el
token el API). No necesitas contraseñas.

Login real contra la base:

```bash
USE_LOGIN=true ADMIN_EMAIL=admin@dataset-software.com ADMIN_PASSWORD=tu_clave \
  k6 run scripts/k6/ventas.js
```

Si cambiaste la clave JWT: `JWT_KEY=...`.

### Datos de prueba

`setup()` llama a `POST /api/v1/dev/ventas/prepare?stock=...&serialUnits=...`, que
garantiza tienda, bodega, cliente y **dos variantes**: una sin serial (stock
agregado alto) y una que requiere serial (con N unidades físicas disponibles y
stock agregado = N).

## Umbrales (thresholds)

- `http_req_failed < 5%`
- p95 creación **sin serial** < 2000 ms
- p95 creación **serial** < 3000 ms
- p95 listado < 1000 ms
- `ventas_error_rate < 5%`

Métricas propias: `ventas_noserial`, `ventas_serial`, `venta_noserial_ms`,
`venta_serial_ms`, `ventas_error_rate`.

## Resultados medidos (instancia sin debugger, base de desarrollo)

| Escenario | Latencia mediana | p95 | Errores | Ventas/s reales |
|---|---|---|---|---|
| 5 cajeros, `THINK=1s` | 10 ms | 20 ms | 0 | ~5/s |
| 5 cajeros, `THINK=0` | 34 ms | 80 ms | 0 | — |
| 15 cajeros, `THINK=0` | 95 ms | 334 ms | 0 | — |
| `RATE=10/s` | 9.9 ms | 15.8 ms | 0 | 9.98/s |
| `RATE=20/s` | 9.6 ms | 15.1 ms | 0 | 19.68/s |

**Conclusiones:**

- **5 cajeros concurrentes es trivial** (~10–35 ms por venta, 0 errores). Incluso
  15 cajeros simultáneos se mantienen sanos (~95 ms).
- El sistema sostiene **≥ 20 ventas/s** sin degradarse.
- El camino **serializado en orden FIFO** es rápido pero intrínsecamente
  **secuencial** por la regla de FIFO estricto (usar `SERIAL_VUS=1`).
- Los números catastróficos de las primeras corridas (p95 de decenas de segundos)
  eran **el debugger de Rider**, no el sistema.

> La venta escribe filas reales (`Sales`, `SaleLineItems`, `SalePayments`,
> `Movements`) y descuenta stock. Úsala sobre la base de **desarrollo**.

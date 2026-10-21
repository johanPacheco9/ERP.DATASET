// ============================================================================
// Prueba de carga de VENTAS (POS) con k6
// ----------------------------------------------------------------------------
// Ejercita los endpoints HTTP que expone el host ERP.DATASET y cubre los DOS
// caminos reales de descuento de inventario en SaleService/StockHelper:
//
//   1) crear_noserial  -> producto SIN serial. La venta descuenta el stock
//      agregado (WarehouseStock.CurrentStock) y consume capas FIFO de entradas
//      (Movement.RemainingQuantity), más antiguas primero.
//
//   2) crear_serial    -> producto que REQUIERE serial. Antes de vender, pide
//      la unidad disponible MÁS ANTIGUA (FIFO estricto: Parametros.ForzarFifoEstricto)
//      y la vende por SerialNumber con quantity=1. Como el FIFO estricto obliga
//      a despachar siempre el más antiguo, este escenario corre secuencial
//      (SERIAL_VUS=1) para no provocar rechazos por orden FIFO.
//
//   3) listar_ventas   -> lectura paginada.
//
// Autenticación: firma un JWT HS256 con la misma clave de appsettings.json
// (Jwt:Key) y lo envía en la cookie "auth_token". Alternativa con credenciales:
// USE_LOGIN=true ADMIN_EMAIL=... ADMIN_PASSWORD=...
//
// Requiere ASPNETCORE_ENVIRONMENT=Development (endpoints /api/v1/dev/*).
//
// Uso:
//   k6 run scripts/k6/ventas.js
//   BASE_URL=http://localhost:5264 RATE=5 DURATION=1m k6 run scripts/k6/ventas.js
// ============================================================================

import http from 'k6/http';
import crypto from 'k6/crypto';
import encoding from 'k6/encoding';
import { check, sleep } from 'k6';
import { Counter, Rate, Trend } from 'k6/metrics';

// ---------------------------------------------------------------------------
// Configuración (sobreescribible por variables de entorno)
// ---------------------------------------------------------------------------
const BASE_URL = __ENV.BASE_URL || 'http://localhost:8080';
const RATE = Number(__ENV.RATE || 5);            // ventas/segundo (sin serial)
const DURATION = __ENV.DURATION || '30s';
const SERIAL_VUS = Number(__ENV.SERIAL_VUS || 1); // VUs para el camino serializado
const LIST_VUS = Number(__ENV.LIST_VUS || 5);
const QTY = Number(__ENV.QTY || 1);              // cantidad por línea (sin serial)
const STOCK = Number(__ENV.STOCK || 1000000);    // stock agregado a preparar
const SERIAL_UNITS = Number(__ENV.SERIAL_UNITS || 2000); // unidades físicas FIFO
// Tope de VUs para el escenario sin serial. Deliberadamente bajo: si la app se
// degrada, k6 debe DESCARTAR iteraciones (dropped_iterations) en vez de disparar
// más peticiones concurrentes y tumbar la instancia.
const MAX_VUS = Number(__ENV.MAX_VUS || Math.max(5, Math.min(20, RATE * 2)));

// Modo "N cajeros concurrentes": si CAJEROS > 0, cada VU es un cajero que vende
// y luego "piensa" THINK segundos (escanear, cobrar, imprimir) antes de vender otra vez.
const CAJEROS = Number(__ENV.CAJEROS || 0);
const THINK = Number(__ENV.THINK || 1);

// Timeout por petición HTTP. Bajarlo permite ABORTAR clientes a mitad de una venta
// (útil para reproducir y diagnosticar transacciones que queden abiertas).
const HTTP_TIMEOUT = __ENV.HTTP_TIMEOUT || '60s';

// Auth self-signed JWT
const JWT_KEY = __ENV.JWT_KEY || 'una-clave-larga-y-secreta-de-al-menos-32-caracteres';
const JWT_ISSUER = __ENV.JWT_ISSUER || 'ERP.DATASET';
const JWT_AUDIENCE = __ENV.JWT_AUDIENCE || 'ERP.DATASET';
const USER_ID = Number(__ENV.USER_ID || 1);
const USER_ROLE = __ENV.USER_ROLE || 'Admin';

// Auth por login real (opcional)
const USE_LOGIN = (__ENV.USE_LOGIN || 'false') === 'true';
const ADMIN_EMAIL = __ENV.ADMIN_EMAIL || 'admin@dataset-software.com';
const ADMIN_PASSWORD = __ENV.ADMIN_PASSWORD || '12345';

// ---------------------------------------------------------------------------
// Métricas
// ---------------------------------------------------------------------------
const ventasNoSerial = new Counter('ventas_noserial');
const ventasSerial = new Counter('ventas_serial');
const ventaNoSerialMs = new Trend('venta_noserial_ms', true);
const ventaSerialMs = new Trend('venta_serial_ms', true);
const ventasErrorRate = new Rate('ventas_error_rate');

// ---------------------------------------------------------------------------
// Opciones / escenarios
// ---------------------------------------------------------------------------
export const options = {
  scenarios: construirEscenarios(),
  thresholds: {
    http_req_failed: ['rate<0.05'],
    'http_req_duration{endpoint:crear_noserial}': ['p(95)<2000'],
    'http_req_duration{endpoint:crear_serial}': ['p(95)<3000'],
    'http_req_duration{endpoint:listar}': ['p(95)<1000'],
    ventas_error_rate: ['rate<0.05'],
  },
};

// Dos formas de cargar el sistema:
//  - Modo "cajeros" (CAJEROS > 0): N usuarios concurrentes, cada uno vende y
//    espera THINK s. Modela "5 cajeros vendiendo a la vez".
//  - Modo "rate" (por defecto): llegada constante de RATE ventas/s.
function construirEscenarios() {
  if (CAJEROS > 0) {
    return {
      cajeros: {
        executor: 'constant-vus',
        vus: CAJEROS,
        duration: DURATION,
        gracefulStop: '10s',
        exec: 'cajeroVenta',
      },
      crear_serial: {
        executor: 'constant-vus',
        vus: SERIAL_VUS,
        duration: DURATION,
        gracefulStop: '10s',
        exec: 'crearVentaSerial',
      },
      listar_ventas: {
        executor: 'constant-vus',
        vus: LIST_VUS,
        duration: DURATION,
        gracefulStop: '10s',
        exec: 'listarVentas',
      },
    };
  }

  return {
    crear_noserial: {
      executor: 'constant-arrival-rate',
      rate: RATE,
      timeUnit: '1s',
      duration: DURATION,
      preAllocatedVUs: Math.max(2, Math.min(5, RATE)),
      maxVUs: MAX_VUS,
      gracefulStop: '10s',
      exec: 'crearVentaNoSerial',
    },
    crear_serial: {
      executor: 'constant-vus',
      vus: SERIAL_VUS,
      duration: DURATION,
      gracefulStop: '10s',
      exec: 'crearVentaSerial',
    },
    listar_ventas: {
      executor: 'constant-vus',
      vus: LIST_VUS,
      duration: DURATION,
      gracefulStop: '10s',
      exec: 'listarVentas',
    },
  };
}

// ---------------------------------------------------------------------------
// JWT / auth
// ---------------------------------------------------------------------------
function b64url(obj) {
  return encoding.b64encode(JSON.stringify(obj), 'rawurl');
}

function firmarToken() {
  const ahora = Math.floor(Date.now() / 1000);
  const header = { alg: 'HS256', typ: 'JWT' };
  const payload = {
    nameid: String(USER_ID),
    unique_name: `loadtest-user-${USER_ID}`,
    role: USER_ROLE,
    iss: JWT_ISSUER,
    aud: JWT_AUDIENCE,
    iat: ahora,
    exp: ahora + 3600,
  };

  const sinFirma = `${b64url(header)}.${b64url(payload)}`;
  const firma = crypto.hmac('sha256', JWT_KEY, sinFirma, 'base64rawurl');
  return `${sinFirma}.${firma}`;
}

function obtenerToken() {
  if (USE_LOGIN) {
    const res = http.post(
      `${BASE_URL}/api/v1/dev/token`,
      JSON.stringify({ email: ADMIN_EMAIL, password: ADMIN_PASSWORD }),
      { headers: { 'Content-Type': 'application/json' } }
    );
    if (res.status !== 200) {
      throw new Error(`Login falló (HTTP ${res.status}): ${res.body}`);
    }
    return res.json('token');
  }
  return firmarToken();
}

function cabecerasAuth(token, extra) {
  return Object.assign(
    {
      'Content-Type': 'application/json',
      Cookie: `auth_token=${token}`,
    },
    extra || {}
  );
}

// ---------------------------------------------------------------------------
// setup(): prepara datos (ambos caminos) y devuelve el contexto
// ---------------------------------------------------------------------------
export function setup() {
  const token = obtenerToken();

  const res = http.post(
    `${BASE_URL}/api/v1/dev/ventas/prepare?stock=${STOCK}&serialUnits=${SERIAL_UNITS}`,
    null,
    { headers: cabecerasAuth(token) }
  );

  if (res.status !== 200) {
    throw new Error(
      `No se pudo preparar datos de prueba (HTTP ${res.status}): ${res.body}\n` +
      `Verifica que la app corra con ASPNETCORE_ENVIRONMENT=Development.`
    );
  }

  const ctx = res.json();
  check(ctx, {
    'contexto sin serial válido': (c) => !!c && c.noSerial && c.noSerial.productoVarianteId > 0,
    'contexto serial válido': (c) => !!c && c.serial && c.serial.productoVarianteId > 0,
    'stock sin serial preparado': (c) => c && c.noSerial && c.noSerial.stock > 0,
    'unidades FIFO preparadas': (c) => c && c.serial && c.serial.availableUnits > 0,
  });

  return { token, ...ctx };
}

// ---------------------------------------------------------------------------
// Escenario 1: venta de producto SIN serial
// ---------------------------------------------------------------------------
export function crearVentaNoSerial(data) {
  const ns = data.noSerial;
  const total = ns.unitPrice * (1 + ns.taxRate) * QTY;
  const payload = {
    clientId: data.clientId,
    warehouseId: data.warehouseId,
    storeId: data.storeId,
    paymentAmount: total,
    paymentMethod: 40, // BankTransfer
    notes: 'carga k6 (sin serial)',
    lines: [{ productoVarianteId: ns.productoVarianteId, quantity: QTY }],
  };

  const res = http.post(
    `${BASE_URL}/api/v1/pos/ventas`,
    JSON.stringify(payload),
    {
      headers: cabecerasAuth(data.token),
      tags: { endpoint: 'crear_noserial', name: 'POST /api/v1/pos/ventas (sin serial)' },
      timeout: HTTP_TIMEOUT,
    }
  );

  ventaNoSerialMs.add(res.timings.duration);

  const ok = check(res, {
    'venta sin serial creada (201)': (r) => r.status === 201,
    'sin error de servidor': (r) => r.status < 500,
  });

  ventasErrorRate.add(!ok);
  if (ok) ventasNoSerial.add(1);
}

// ---------------------------------------------------------------------------
// Modo "cajeros": un cajero = una venta + tiempo de "pensar" (THINK s)
// ---------------------------------------------------------------------------
export function cajeroVenta(data) {
  crearVentaNoSerial(data);
  sleep(THINK);
}

// ---------------------------------------------------------------------------
// Escenario 2: venta de producto CON serial (FIFO estricto)
// ---------------------------------------------------------------------------
export function crearVentaSerial(data) {
  const s = data.serial;

  // 2.1 Pedir la unidad disponible más antigua (el "siguiente" en FIFO).
  const lookup = http.get(
    `${BASE_URL}/api/v1/dev/ventas/next-serial?warehouseId=${data.warehouseId}&productoVarianteId=${s.productoVarianteId}`,
    {
      headers: cabecerasAuth(data.token),
      tags: { endpoint: 'serial_lookup', name: 'GET next-serial' },
      timeout: HTTP_TIMEOUT,
    }
  );

  const info = lookup.json();
  if (!info || !info.serialNumber) {
    // Se agotaron las unidades serializadas preparadas.
    sleep(0.5);
    return;
  }

  // 2.2 Venderla por serial (quantity SIEMPRE 1).
  const total = s.unitPrice * (1 + s.taxRate);
  const payload = {
    clientId: data.clientId,
    warehouseId: data.warehouseId,
    storeId: data.storeId,
    paymentAmount: total,
    paymentMethod: 40,
    notes: 'carga k6 (serial FIFO)',
    lines: [
      {
        productoVarianteId: s.productoVarianteId,
        serialNumber: info.serialNumber,
        quantity: 1,
      },
    ],
  };

  const res = http.post(
    `${BASE_URL}/api/v1/pos/ventas`,
    JSON.stringify(payload),
    {
      headers: cabecerasAuth(data.token),
      tags: { endpoint: 'crear_serial', name: 'POST /api/v1/pos/ventas (serial)' },
      timeout: HTTP_TIMEOUT,
    }
  );

  ventaSerialMs.add(res.timings.duration);

  const ok = check(res, {
    'venta serial creada (201)': (r) => r.status === 201,
    'sin error de servidor': (r) => r.status < 500,
  });

  ventasErrorRate.add(!ok);
  if (ok) ventasSerial.add(1);
}

// ---------------------------------------------------------------------------
// Escenario 3: listar ventas
// ---------------------------------------------------------------------------
export function listarVentas(data) {
  const res = http.get(
    `${BASE_URL}/api/v1/pos/ventas?pageNumber=1&pageSize=20`,
    {
      headers: cabecerasAuth(data.token),
      tags: { endpoint: 'listar', name: 'GET /api/v1/pos/ventas' },
      timeout: HTTP_TIMEOUT,
    }
  );

  check(res, {
    'listado ok (200)': (r) => r.status === 200,
  });
}

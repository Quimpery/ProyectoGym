# Gym Rutinas — Frontend

Aplicación web (SPA) hecha con **Angular 21** para gestionar rutinas de gimnasio: listar y buscar
rutinas en tiempo real, ver el detalle con los ejercicios organizados por día de la semana, y crear,
editar y eliminar rutinas con sus ejercicios.

Consume la API REST del backend .NET (ver [`../Backend/README.md`](../Backend/README.md)).

Trabajo Final — Programación 4 — UTN.

---

## 1. Requisitos previos

| Herramienta | Versión | Cómo verificar |
|---|---|---|
| Node.js | **20.19+**, **22.12+** o **24+** (lo exige Angular 21) | `node --version` |
| npm | 10 o superior (viene con Node.js) | `npm --version` |
| Backend GymApi | corriendo en `http://localhost:5000` | abrir http://localhost:5000/api/rutinas |

No hace falta instalar Angular CLI de forma global: el proyecto lo trae como dependencia y se usa a
través de los scripts de `npm`.

> **Windows / PowerShell:** si `npm` muestra *"no se puede cargar el archivo npm.ps1 porque la ejecución
> de scripts está deshabilitada"*, ejecutá una sola vez
> `Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned`, o usá `npm.cmd` en lugar de `npm`.

---

## 2. Instalación

Desde la carpeta `frontend/`:

```bash
npm install
```

Descarga todas las dependencias declaradas en `package.json` dentro de `node_modules/`
(equivale a `dotnet restore` o `pip install -r requirements.txt`).

---

## 3. Configuración (URL del backend)

La URL de la API se configura en los archivos de **environments**:

| Archivo | Se usa con | Valor por defecto |
|---|---|---|
| [`src/environments/environment.development.ts`](src/environments/environment.development.ts) | `npm start` (desarrollo) | `http://localhost:5000/api` |
| [`src/environments/environment.ts`](src/environments/environment.ts) | `npm run build` (producción) | `http://localhost:5000/api` |

```ts
// src/environments/environment.development.ts
export const environment = {
  apiUrl: 'http://localhost:5000/api',
};
```

**Cómo cambiar la URL de la API:**

1. Editá `apiUrl` en el archivo que corresponda (por ejemplo, si el backend corre en otro puerto:
   `'http://localhost:5050/api'`).
2. Reiniciá `npm start`.
3. Si cambia el puerto u origen del **frontend**, hay que permitirlo también en la política CORS del
   backend (`Backend/GymApi/Program.cs`, método `WithOrigins`).

En el código siempre se importa `environments/environment`; al compilar en modo desarrollo Angular lo
reemplaza por `environment.development.ts` (configurado en `angular.json` → `fileReplacements`).

---

## 4. Ejecución

### Modo desarrollo

```bash
npm start
```

- Abre el servidor de desarrollo en **http://localhost:4200** (equivale a `ng serve`).
- Se recarga solo cada vez que guardás un archivo.
- Para usar otro puerto: `npm start -- --port 4300` (y agregarlo en CORS del backend).

### Compilar para producción

```bash
npm run build
```

- Genera los archivos optimizados en `dist/frontend/browser/`.
- Esa carpeta se puede publicar en cualquier servidor de archivos estáticos (IIS, Nginx, Netlify...).

### Orden recomendado para levantar todo

1. SQL Server corriendo.
2. Backend: `dotnet run --project GymApi` (desde `Backend/`) → http://localhost:5000
3. Frontend: `npm start` (desde `frontend/`) → http://localhost:4200

---

## 5. Tecnologías utilizadas

| Tecnología | Versión | Para qué |
|---|---|---|
| **Angular** | 21.2 | Framework de la aplicación (componentes standalone, signals, modo *zoneless*) |
| **TypeScript** | 5.9 | Lenguaje (JavaScript con tipos), en modo `strict` |
| **Angular Router** | 21.2 | Navegación entre pantallas sin recargar, con *lazy loading* |
| **Angular HttpClient** | 21.2 | Peticiones HTTP a la API (reemplaza a Axios / Fetch) |
| **Angular Reactive Forms** | 21.2 | Formularios con validaciones del lado del cliente |
| **RxJS** | 7.8 | Manejo de flujos asíncronos (búsqueda con `debounceTime` y `switchMap`) |
| **CSS** | — | Estilos propios con variables CSS, Flexbox y Grid (sin librería de componentes) |

---

## 6. Funcionalidades

- **Listado** de rutinas con información resumida (cantidad de ejercicios, días, fecha).
- **Búsqueda en tiempo real** por nombre (parcial y sin distinguir mayúsculas), con mensaje si no hay resultados.
- **Detalle** de una rutina con los ejercicios agrupados por día, mostrando solo los días con ejercicios.
- **Alta y edición** de rutinas: nombre, descripción y ejercicios (agregar, quitar, reordenar dentro del día).
- **Eliminación** con confirmación previa.
- **Validaciones** en el cliente (campos obligatorios, números enteros y positivos, largos máximos) y
  muestra de los errores devueltos por la API (por ejemplo, nombre de rutina repetido).
- **Feedback visual**: estados de carga, mensajes de error con "Reintentar" y avisos de éxito/error.
- **Diseño responsive** (celular, tablet y escritorio) y atributos de accesibilidad (labels, `aria-*`).

---

## 7. Estructura del proyecto

```
frontend/
├── angular.json                 # Configuración del build (environments, estilos, assets)
├── package.json                 # Dependencias y scripts (start, build)
├── tsconfig.json                # Configuración de TypeScript (modo strict)
├── public/                      # Archivos estáticos (favicon)
└── src/
    ├── index.html               # Único HTML real; contiene <app-root>
    ├── main.ts                  # Punto de entrada: arranca la app
    ├── styles.css               # Estilos GLOBALES: variables, botones, campos, estados
    ├── environments/            # URL de la API por entorno
    │   ├── environment.ts
    │   └── environment.development.ts
    └── app/
        ├── app.ts / app.html / app.css   # Componente raíz: barra de navegación + <router-outlet>
        ├── app.config.ts        # Providers globales: router, HttpClient, idioma es-AR
        ├── app.routes.ts        # Tabla de rutas (con lazy loading)
        ├── models/
        │   └── rutina.ts        # Interfaces y tipos (Rutina, Ejercicio, DiaSemana...)
        ├── services/
        │   ├── rutina.service.ts    # Llamadas HTTP a /api/rutinas
        │   └── mensaje.service.ts   # Avisos de éxito/error compartidos (signals)
        ├── components/          # Componentes reutilizables
        │   ├── buscador/        # Barra de búsqueda (input/output)
        │   ├── ejercicio-card/  # Muestra un ejercicio
        │   └── mensajes/        # Dibuja los avisos (toasts)
        └── pages/               # Una carpeta por pantalla
            ├── lista-rutinas/       # Listado + búsqueda + eliminar
            ├── detalle-rutina/      # Detalle agrupado por día
            └── formulario-rutina/   # Alta y edición (mismo componente)
```

### Rutas

| URL | Pantalla |
|---|---|
| `/` | Redirige a `/rutinas` |
| `/rutinas` | `ListaRutinas` — listado y búsqueda |
| `/rutinas/nueva` | `FormularioRutina` — alta |
| `/rutinas/:id` | `DetalleRutina` — detalle |
| `/rutinas/:id/editar` | `FormularioRutina` — edición |
| cualquier otra | Redirige a `/rutinas` |

### Organización de los componentes

- **Páginas (`pages/`)**: se muestran según la URL. Inyectan los servicios, piden los datos y
  manejan el estado de la pantalla con `signal()` y `computed()`.
- **Componentes reutilizables (`components/`)**: no conocen la API. Reciben datos con `input()` y
  avisan eventos con `output()`.
- **Servicios (`services/`)**: singletons (`providedIn: 'root'`). `RutinaService` concentra todas las
  llamadas HTTP; `MensajeService` guarda el estado compartido de los avisos.
- **Modelos (`models/`)**: interfaces TypeScript que reflejan los DTOs de la API.

**Flujo de datos:** `Página` → `RutinaService` → `HttpClient` → `API .NET` → respuesta JSON →
`signal.set(...)` → Angular vuelve a dibujar la pantalla.

---

## 8. Problemas frecuentes

| Síntoma | Causa probable | Solución |
|---|---|---|
| "No se pudieron cargar las rutinas. ¿Está corriendo la API...?" | El backend no está levantado | Iniciar el backend (sección 4) |
| Error de **CORS** en la consola del navegador (F12) | El origen del frontend no está permitido | Agregarlo en `WithOrigins` de `Program.cs` |
| `npm.ps1 no se puede cargar` | Política de ejecución de PowerShell | Ver nota de la sección 1 |
| `Port 4200 is already in use` | Otro `ng serve` abierto | Cerrarlo o usar `npm start -- --port 4300` |

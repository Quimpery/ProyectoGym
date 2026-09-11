# GymApi — Backend del Sistema de Gestión de Rutinas de Gimnasio

API REST hecha con **ASP.NET Core (.NET 10)**, **Entity Framework Core 10** y **SQL Server** para
crear, listar, buscar, modificar y eliminar rutinas de entrenamiento con sus ejercicios organizados
por día de la semana.

Trabajo Final — Programación 4 — UTN.

---

## 1. Requisitos previos

| Herramienta | Versión | Cómo verificar |
|---|---|---|
| .NET SDK | **10.0** o superior | `dotnet --list-sdks` |
| SQL Server | 2019 o superior (sirve **SQL Server Express** o **LocalDB**) | `sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@VERSION"` |
| Herramienta `dotnet-ef` | 10.x (solo para manejar migraciones a mano) | `dotnet ef --version` |

Descargas: [.NET 10](https://dotnet.microsoft.com/download) ·
[SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)

---

## 2. Instalación

Desde la carpeta `Backend/`:

```bash
# 1) Restaurar los paquetes NuGet (equivalente a "pip install -r requirements.txt")
dotnet restore

# 2) (Opcional) Instalar la herramienta de migraciones de Entity Framework
dotnet tool install --global dotnet-ef --version "10.*"
```

Los paquetes que usa el proyecto están declarados en [`GymApi/GymApi.csproj`](GymApi/GymApi.csproj)
(es el equivalente de `requirements.txt` en .NET):

| Paquete | Para qué |
|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | ORM + proveedor de SQL Server |
| `Microsoft.EntityFrameworkCore.Design` | Necesario para `dotnet ef` (migraciones) |
| `Microsoft.AspNetCore.OpenApi` | Genera el documento OpenAPI de los endpoints |
| `Swashbuckle.AspNetCore.SwaggerUI` | Interfaz web Swagger para probar la API |

---

## 3. Configuración de la base de datos

### 3.1 String de conexión

Se configura en [`GymApi/appsettings.json`](GymApi/appsettings.json), en la sección `ConnectionStrings`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=GymRutinasDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

> En JSON la barra invertida se escribe doble (`\\`). El valor real es `localhost\SQLEXPRESS`.

### 3.2 Formato del string de conexión

```
Server=<servidor>;Database=<nombre_base>;<autenticación>;TrustServerCertificate=True
```

| Parte | Significado |
|---|---|
| `Server` | Instancia de SQL Server. `localhost\SQLEXPRESS` = SQL Server Express local. |
| `Database` | Nombre de la base. **No hace falta crearla**: la API la crea sola (ver 3.4). |
| `Trusted_Connection=True` | Entra con tu usuario de Windows (sin usuario/contraseña). |
| `User Id=...;Password=...` | Alternativa: autenticación de SQL Server con usuario y contraseña. |
| `TrustServerCertificate=True` | Acepta el certificado autofirmado de un SQL Server local. |

Ejemplos según tu instalación:

```text
# SQL Server Express con usuario de Windows (el que viene configurado)
Server=localhost\SQLEXPRESS;Database=GymRutinasDb;Trusted_Connection=True;TrustServerCertificate=True

# SQL Server "completo" (instancia por defecto)
Server=localhost;Database=GymRutinasDb;Trusted_Connection=True;TrustServerCertificate=True

# LocalDB (viene con Visual Studio)
Server=(localdb)\MSSQLLocalDB;Database=GymRutinasDb;Trusted_Connection=True;TrustServerCertificate=True

# Usuario y contraseña de SQL Server (ej. SQL Server en Docker)
Server=localhost,1433;Database=GymRutinasDb;User Id=sa;Password=TuPassword123;TrustServerCertificate=True
```

### 3.3 Variables de entorno (alternativa a editar appsettings.json)

.NET permite **pisar** cualquier valor de `appsettings.json` con una variable de entorno.
El `:` de las secciones se reemplaza por **doble guion bajo** `__`:

| Variable de entorno | Reemplaza a |
|---|---|
| `ConnectionStrings__DefaultConnection` | `ConnectionStrings:DefaultConnection` |

```powershell
# PowerShell (vale solo para esa terminal)
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=GymRutinasDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project GymApi
```

```bash
# Git Bash / Linux / macOS
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=GymRutinasDb;User Id=sa;Password=TuPassword123;TrustServerCertificate=True"
dotnet run --project GymApi
```

### 3.4 Crear la base de datos y las tablas

Hay tres formas; **con la primera alcanza**.

**A) Automática (recomendada).** Al iniciar, la API ejecuta `Database.Migrate()` (ver `Program.cs`):
si la base `GymRutinasDb` no existe la crea, y aplica las migraciones pendientes. Solo hay que
correr la API (sección 4).

**B) Con migraciones de Entity Framework**, desde `Backend/GymApi/`:

```bash
# Aplica todas las migraciones (crea la base y las tablas)
dotnet ef database update

# Si cambiás un modelo (Models/*.cs), generás una nueva migración y la aplicás:
dotnet ef migrations add NombreDelCambio
dotnet ef database update
```

**C) Con el script SQL** [`database/script.sql`](database/script.sql) (crea las tablas e inserta
datos de ejemplo). Primero hay que crear la base vacía:

```bash
sqlcmd -S .\SQLEXPRESS -E -Q "CREATE DATABASE GymRutinasDb"
sqlcmd -S .\SQLEXPRESS -E -d GymRutinasDb -f 65001 -i database\script.sql
```

> `-f 65001` le indica a `sqlcmd` que lea el archivo como UTF-8 (si no, las tildes se guardan mal).
> El script se puede correr varias veces: no duplica tablas ni datos de ejemplo.

### 3.5 Modelo de datos

```
Rutinas                                   Ejercicios
-------------------------------           ----------------------------------------
Id             int  PK (identity)   1───N Id            int  PK (identity)
Nombre         nvarchar(100) ÚNICO        RutinaId      int  FK -> Rutinas.Id (ON DELETE CASCADE)
Descripcion    nvarchar(500) NULL         Nombre        nvarchar(100)
FechaCreacion  datetime2                  Dia           nvarchar(10)  (Lunes..Domingo)
                                          Series        int
                                          Repeticiones  int
                                          Peso          decimal(6,2) NULL
                                          Notas         nvarchar(500) NULL
                                          Orden         int
```

- **Índice único** en `Rutinas.Nombre`: impide nombres repetidos (sin distinguir mayúsculas con la collation por defecto).
- **Borrado en cascada**: al eliminar una rutina se eliminan sus ejercicios.
- **Índice** en `Ejercicios.RutinaId` para acelerar la carga de los ejercicios de una rutina.

---

## 4. Ejecución

Desde la carpeta `Backend/`:

```bash
dotnet run --project GymApi
```

| Qué | URL |
|---|---|
| API | http://localhost:5000/api/rutinas |
| **Swagger UI** (documentación y pruebas) | **http://localhost:5000/swagger** |
| Documento OpenAPI (JSON) | http://localhost:5000/openapi/v1.json |

- El puerto se configura en [`GymApi/Properties/launchSettings.json`](GymApi/Properties/launchSettings.json) (`applicationUrl`).
- Swagger solo está habilitado en el entorno `Development` (el que usa `dotnet run` por defecto).
- CORS permite peticiones desde el frontend en `http://localhost:4200` (se cambia en `Program.cs`).
- También se puede abrir `GymApi.slnx` con Visual Studio y ejecutar con F5.

---

## 5. Endpoints disponibles

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/rutinas` | Lista todas las rutinas (resumen) | 200 |
| GET | `/api/rutinas/{id}` | Detalle de una rutina con sus ejercicios | 200, 404 |
| GET | `/api/rutinas/buscar?nombre={texto}` | Búsqueda parcial por nombre, sin distinguir mayúsculas | 200 (lista vacía si no hay resultados) |
| POST | `/api/rutinas` | Crea una rutina con sus ejercicios | 201, 400, 409 |
| PUT | `/api/rutinas/{id}` | Modifica la rutina y **reemplaza** su lista de ejercicios | 200, 400, 404, 409 |
| DELETE | `/api/rutinas/{id}` | Elimina la rutina y sus ejercicios | 204, 404 |

> Los endpoints opcionales de ejercicios no se implementaron: el `PUT` recibe la rutina completa, así que con
> una sola operación se pueden agregar, modificar, eliminar y reordenar ejercicios.

### Body de POST / PUT

```json
{
  "nombre": "Fuerza 3 días",
  "descripcion": "Full body para principiantes",
  "ejercicios": [
    { "nombre": "Sentadilla", "dia": "Lunes", "series": 4, "repeticiones": 10, "peso": 60, "notas": null, "orden": 1 },
    { "nombre": "Dominadas", "dia": "Lunes", "series": 3, "repeticiones": 6, "peso": null, "notas": "Peso corporal", "orden": 2 }
  ]
}
```

- `dia`: `Lunes`, `Martes`, `Miercoles`, `Jueves`, `Viernes`, `Sabado` o `Domingo` (sin tilde).
- `orden`: posición del ejercicio dentro de su día (empieza en 1).

### Validaciones

| Campo | Regla |
|---|---|
| `nombre` (rutina) | Obligatorio, máx. 100 caracteres, **único** (409 si ya existe) |
| `descripcion` | Opcional, máx. 500 caracteres |
| `ejercicios[].nombre` | Obligatorio, máx. 100 caracteres |
| `ejercicios[].dia` | Obligatorio, debe ser un día válido |
| `ejercicios[].series` | Entero entre 1 y 100 |
| `ejercicios[].repeticiones` | Entero entre 1 y 1000 |
| `ejercicios[].peso` | Opcional; si viene, positivo (0.01 a 9999.99) |
| `ejercicios[].notas` | Opcional, máx. 500 caracteres |
| `ejercicios[].orden` | Entero mayor o igual a 1 |

### Formato de errores

Todos los errores (400, 404, 409, 500) responden con el mismo formato:

```json
{
  "mensaje": "Los datos enviados no son válidos.",
  "errores": [
    "Las series deben ser un número entre 1 y 100.",
    "El campo 'ejercicios[0].dia' tiene un valor inválido."
  ]
}
```

---

## 6. Estructura del proyecto

```
Backend/
├── GymApi.slnx                  # Solución (se abre con Visual Studio)
├── README.md
├── database/
│   └── script.sql               # Script de creación de tablas + datos de ejemplo
└── GymApi/
    ├── Program.cs               # Configuración: servicios, EF, CORS, Swagger, errores
    ├── appsettings.json         # String de conexión y logging
    ├── GymApi.csproj            # Proyecto y paquetes NuGet
    ├── Properties/
    │   └── launchSettings.json  # Puerto (http://localhost:5000)
    ├── Controllers/
    │   └── RutinasController.cs # Endpoints HTTP: recibe requests y devuelve códigos de estado
    ├── Services/
    │   └── RutinaService.cs     # Lógica de negocio y acceso a datos con EF Core
    ├── Data/
    │   └── GymDbContext.cs      # DbContext: tablas, relación y cascada
    ├── Models/                  # Entidades = tablas de la base
    │   ├── Rutina.cs
    │   ├── Ejercicio.cs
    │   └── DiaSemana.cs
    ├── Dtos/                    # Objetos de entrada/salida de la API (con validaciones)
    │   ├── RutinaGuardarDto.cs
    │   ├── EjercicioGuardarDto.cs
    │   ├── RutinaResumenDto.cs
    │   ├── RutinaDetalleDto.cs
    │   ├── EjercicioDto.cs
    │   └── ErrorRespuesta.cs
    └── Migrations/              # Migraciones generadas por EF Core
```

**Flujo de una petición:** `HTTP` → `RutinasController` (valida DTO y decide el código de respuesta) →
`RutinaService` (lógica y consultas) → `GymDbContext` (EF Core) → `SQL Server`.

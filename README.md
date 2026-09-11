# Sistema de Gestión de Rutinas de Gimnasio

Trabajo Final — Programación 4 — UTN.

Sistema web para crear, visualizar, buscar, modificar y eliminar rutinas de entrenamiento con sus
ejercicios organizados por día de la semana.

| Capa | Tecnología | Carpeta | Documentación |
|---|---|---|---|
| Base de datos | SQL Server (Express) | `Backend/database/` | [Backend/README.md](Backend/README.md#3-configuración-de-la-base-de-datos) |
| Backend (API REST) | ASP.NET Core .NET 10 + Entity Framework Core 10 | [`Backend/`](Backend/) | [Backend/README.md](Backend/README.md) |
| Frontend (SPA) | Angular 21 + TypeScript | [`frontend/`](frontend/) | [frontend/README.md](frontend/README.md) |

## Inicio rápido

Requisitos: .NET SDK 10, SQL Server (Express sirve), Node.js 20.19+ / 22.12+ / 24+.

```bash
# 1) Backend: crea la base GymRutinasDb automáticamente al arrancar
cd Backend
dotnet run --project GymApi
# API en http://localhost:5000  ·  Swagger en http://localhost:5000/swagger

# 2) (Opcional) Cargar datos de ejemplo, en otra terminal desde Backend/
sqlcmd -S .\SQLEXPRESS -E -d GymRutinasDb -f 65001 -i database\script.sql

# 3) Frontend, en otra terminal
cd frontend
npm install
npm start
# App en http://localhost:4200
```

Si tu SQL Server no es `localhost\SQLEXPRESS`, primero ajustá el string de conexión en
`Backend/GymApi/appsettings.json` (ver [Backend/README.md](Backend/README.md#31-string-de-conexión)).

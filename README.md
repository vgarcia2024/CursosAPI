# CursosAPI

Plataforma de cursos online (venta y cursada) desarrollada como Trabajo Práctico de Programación 4 — UTN San Nicolás.

El proyecto es un monorepo con dos partes:

```
CursosAPI/
├── Backend/     API REST en .NET 8 (C#) + SQL Server
├── Frontend/    SPA en React + Vite
├── Dockerfile          # build de la imagen del backend
└── docker-compose.yml  # levanta backend + SQL Server juntos
```

## Stack

| Capa       | Tecnología |
|------------|------------|
| Backend    | ASP.NET Core 8 Web API, Entity Framework Core 9, SQL Server, AutoMapper, autenticación por cookie |
| Frontend   | React 18, Vite, Tailwind CSS v4, Zustand, React Hook Form + Zod, Axios, wouter |
| Infra      | Docker / Docker Compose |
| Email      | Resend (recupero de contraseña) |

## Funcionalidad principal

- Registro, login y sesión persistente por **cookie httpOnly** (no JWT).
- Roles: `Admin`, `UserGratis` y `UserPremium`.
- ABM de cursos (solo Admin), con distinción entre cursos gratuitos y pagos.
- Los usuarios `UserGratis` (y anónimos) solo pueden acceder al detalle de cursos gratuitos; `Admin` y `UserPremium` tienen acceso total.
- Panel de administración: gestión de cursos, usuarios y roles (incluye pasar un usuario de Free a Premium).
- Recupero de contraseña por email (Resend + plantilla HTML con Handlebars).
- Documentación interactiva de la API vía Swagger (`/swagger`) en desarrollo.

## Cómo levantar todo el proyecto

### Opción 1: Docker Compose (recomendada)

Desde la raíz del repo, esto levanta el backend + SQL Server en un solo comando:

```bash
docker compose up --build
```

- API disponible en `http://localhost:5000`
- SQL Server disponible en `localhost:1433`

Las migraciones de EF Core se aplican automáticamente al arrancar el contenedor de la API (con reintentos, por si SQL Server todavía no terminó de levantar).

Después, corré el frontend aparte (ver `Frontend/README.md`), apuntando `VITE_API_URL` a `http://localhost:5000`.

### Opción 2: correr cada parte por separado

Ver las instrucciones detalladas en:

- [`Backend/README.md`](./Backend/README.md)
- [`Frontend/README.md`](./Frontend/README.md)

## Repositorios / links

- Backend: API REST documentada con Swagger.
- Frontend: consumido en `http://localhost:5173` (puerto fijo por el CORS del backend).

## Licencia

Proyecto académico, sin licencia específica.

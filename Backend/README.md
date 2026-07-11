# CursosAPI — Backend

API REST en **ASP.NET Core 8** para la plataforma de cursos. Expone autenticación por cookie, gestión de cursos, usuarios y roles, con SQL Server como base de datos vía Entity Framework Core.

## Stack

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core 9** + **SQL Server**
- **AutoMapper** — mapeo de entidades a DTOs
- **BCrypt.Net-Next** — hasheo de contraseñas
- **Resend** + **Handlebars.Net** — envío de emails (recupero de contraseña) con plantilla HTML
- **Swashbuckle (Swagger)** — documentación interactiva de la API
- Autenticación por **cookie httpOnly** (no JWT)

## Estructura

```
Tp_Programacion/
├── Controllers/     AuthController, CursoController, RoleController, UserController
├── Services/        lógica de negocio (Auth, Curso, Role, User, Email, Encoder)
├── Repository/      acceso a datos (patrón repositorio genérico + específicos)
├── Models/          entidades (Curso, User, Role) y sus DTOs
├── Config/          AppDbContext y perfiles de AutoMapper (Mapping.cs)
├── Enums/           ROLES (Admin, UserGratis, UserPremium)
├── Migrations/       migraciones de EF Core
├── Templates/       plantilla HTML (reset-password.html)
├── Utils/           manejo de errores y respuestas (ErrorResponse, ResponseMessage, ResponseValidation)
└── Program.cs        configuración de servicios, CORS, auth y pipeline
```

## Roles y accesos

Definidos en `Enums/ROLES.cs`:

- `Admin` — acceso total, ABM de cursos, usuarios y roles.
- `UserPremium` — acceso a todos los cursos (gratuitos y pagos).
- `UserGratis` — solo acceso a cursos gratuitos.

Los usuarios anónimos pueden ver el detalle de un curso solo si es gratuito (`GET /api/cursos/{id}` está en `[AllowAnonymous]`, pero valida el acceso puertas adentro del servicio).

## Endpoints principales

### Auth (`/api/auth`)

| Método | Ruta | Descripción | Acceso |
|---|---|---|---|
| POST | `/register` | Registro de usuario | Público |
| POST | `/login` | Login (setea cookie de sesión) | Público |
| POST | `/logout` | Cierra sesión | Autenticado |
| GET | `/me` | Datos del usuario autenticado | Autenticado |
| GET | `/check-auth` | Verifica sesión activa | Autenticado |
| GET | `/health` | Health check | Público |
| PUT | `/update-roles/{userId}` | Reemplaza los roles de un usuario | Admin |
| PUT | `/generate-pwdtoken` | Genera token de recupero y envía email | Autenticado |
| GET | `/verify-pwdtoken` | Verifica el token de recupero | Autenticado |

### Cursos (`/api/cursos`)

| Método | Ruta | Descripción | Acceso |
|---|---|---|---|
| GET | `/` | Lista todos los cursos (marca a cuáles tiene acceso el usuario) | Autenticado |
| GET | `/{id}` | Detalle de un curso | Público (con restricción de contenido si es pago) |
| POST | `/` | Crea un curso | Admin |
| PUT | `/{id}` | Edita un curso | Admin |
| DELETE | `/{id}` | Elimina un curso | Admin |

### Usuarios (`/api/users`) y Roles (`/api/roles`)

ABM completo (`GET`, `GET /{id}`, `PUT /{id}`, `DELETE /{id}`, y `POST` para roles), reservado a `Admin`.

La documentación completa e interactiva de todos los endpoints está disponible en Swagger (`/swagger`) al correr el proyecto en modo desarrollo.

## Configuración

### Connection string

En `appsettings.json`:

```json
"ConnectionStrings": {
  "devConnection": "Server=db;Database=CursosDB;User Id=sa;Password=Programacion42026;TrustServerCertificate=True;"
}
```

Si corrés SQL Server fuera de Docker, cambiá `Server=db` por `Server=localhost` (o el host que corresponda).

### Variable de entorno para email

El envío de emails (recupero de contraseña) usa [Resend](https://resend.com) y requiere:

```bash
RESEND_APITOKEN=tu_api_token
```

### CORS

El backend solo acepta requests con credenciales desde:

- `http://localhost:5173` (frontend en desarrollo)
- `https://cursos-api.vercel.app` (producción)

Si desplegás el frontend en otro dominio, hay que agregarlo en `Program.cs` (`app.UseCors`).

> ⚠️ La cookie de sesión se configura con `SecurePolicy = Always` y `SameSite = None`, por lo que **el backend debe servirse por HTTPS** (o `localhost`) para que el navegador la guarde y reenvíe correctamente.

## Cómo correrlo

### Con Docker Compose (recomendado)

Desde la raíz del repo (no desde `Backend/`):

```bash
docker compose up --build
```

Levanta SQL Server + la API. Las migraciones se aplican automáticamente al iniciar. La API queda expuesta en `http://localhost:5000`.

### Local, sin Docker

Requiere una instancia de SQL Server accesible y el SDK de .NET 8.

```bash
cd Backend/Tp_Programacion
dotnet restore
dotnet ef database update   # aplica las migraciones
dotnet run
```

Por defecto levanta en `http://localhost:5023` (perfil `http` en `launchSettings.json`) y abre Swagger automáticamente en `/swagger`.

## Migraciones

Para agregar una nueva migración después de modificar un modelo:

```bash
cd Backend/Tp_Programacion
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

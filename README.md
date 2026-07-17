# DevBoard — API

Backend de DevBoard, un hub personal para desarrolladores en búsqueda laboral. Permite registrar postulaciones, hacer seguimiento de entrevistas y contactos, y visualizar el progreso mediante un dashboard con estadísticas.

---

## Stack tecnológico

| Capa | Tecnología |
|---|---|
| Framework | .NET 10, ASP.NET Core |
| Base de datos | PostgreSQL 16 |
| ORM | Entity Framework Core con Fluent API |
| Arquitectura | Clean Architecture + CQRS con MediatR |
| Autenticación | JWT + Refresh Tokens |
| Documentación | Scalar |
| Contenedores | Docker + Docker Compose *(Fase 3)* |
| Mensajería | RabbitMQ *(Fase 3)* |
| Caché | Redis *(Fase 3)* |

---

## Arquitectura

El proyecto sigue los principios de Clean Architecture, dividido en cuatro capas con dependencias estrictamente unidireccionales:

```
DevBoard.Domain          → entidades y contratos, sin dependencias externas
DevBoard.Application     → lógica de negocio, CQRS, Result pattern
DevBoard.Infrastructure  → EF Core, repositorios, JWT, BCrypt
DevBoard.API             → controllers, middleware, DI
```

**Regla de dependencia:** las capas externas dependen de las internas. `Domain` no conoce a nadie. `Application` solo conoce a `Domain`. `Infrastructure` y `API` implementan los contratos definidos en las capas internas.

### Decisiones de diseño

- **Result pattern** en lugar de excepciones para errores de negocio. Los handlers devuelven `Result<T>`, haciendo explícito que una operación puede fallar.
- **Respuestas estandarizadas** con estructura `{ success, data, error }` en todos los endpoints.
- **UnitOfWork** como abstracción sobre `DbContext`, manteniendo `Application` desacoplada de EF Core.
- **Fluent API** para toda la configuración del modelo, sin data annotations en las entidades.
- **Soft delete en contactos** — los contactos eliminados se marcan como `IsDeleted = true` y se preserva el nombre en las postulaciones históricas mediante el campo desnormalizado `ContactName`.
- **Variables de entorno** para toda la configuración sensible, preparado para Docker desde el día uno.
- **HTTPS** deshabilitado en desarrollo — se configurará con reverse proxy en la Fase 4.

---

## Entidades del dominio

- **User** — cuenta del desarrollador, incluye credenciales y refresh token
- **JobApplication** — postulación a un puesto, con estado actual desnormalizado para el dashboard y nombre de contacto desnormalizado para preservar historial
- **ApplicationStatus** — historial de cambios de estado de una postulación
- **Contact** — persona de contacto asociada a postulaciones, con soft delete
- **Interview** — entrevista vinculada a una postulación, con fecha y tipo

---

## Endpoints

### Auth
| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/register` | Registrar usuario |
| POST | `/api/auth/login` | Iniciar sesión |
| POST | `/api/auth/refresh` | Renovar access token |

### Job Applications
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/jobapplications` | Listar postulaciones del usuario |
| GET | `/api/jobapplications/{id}` | Detalle con historial y entrevistas |
| POST | `/api/jobapplications` | Crear postulación |
| PUT | `/api/jobapplications/{id}` | Editar postulación |
| DELETE | `/api/jobapplications/{id}` | Eliminar postulación |
| POST | `/api/jobapplications/{id}/status` | Cambiar estado |

### Contacts
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/contacts` | Listar contactos |
| GET | `/api/contacts/{id}` | Detalle de contacto |
| POST | `/api/contacts` | Crear contacto |
| PUT | `/api/contacts/{id}` | Editar contacto |
| DELETE | `/api/contacts/{id}` | Soft delete |

### Interviews
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/interviews/job-application/{id}` | Entrevistas de una postulación |
| POST | `/api/interviews` | Crear entrevista |
| PUT | `/api/interviews/{id}` | Editar entrevista |
| DELETE | `/api/interviews/{id}` | Eliminar entrevista |

### Dashboard
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/dashboard` | Estadísticas, postulaciones stale, próximas entrevistas |

---

## Fases del proyecto

**Fase 1 — Backend + Auth** ✅
- Clean Architecture completa
- CRUD de postulaciones, contactos y entrevistas
- Autenticación con JWT y Refresh Tokens
- Dashboard endpoint

**Fase 2 — Frontend Angular** ✅
- Dashboard con estadísticas visuales
- Gestión de postulaciones con historial de estados
- Gestión de contactos con soft delete
- Gestión de entrevistas

**Fase 3 — Mensajería y caché** 🔄 Próxima
- RabbitMQ para eventos de dominio
- Redis para caché
- Docker Compose completo con todos los servicios

**Fase 4 — Deploy y documentación**
- Deploy con URL pública
- HTTPS con reverse proxy
- README y Scalar completos

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/) *(requerido a partir de Fase 3)*

## Configuración local

```bash
# Clonar el repositorio
git clone https://github.com/tu-usuario/devboard-api.git
cd devboard-api

# Configurar appsettings.Development.json
# (ver sección de configuración abajo)

# Aplicar migraciones
dotnet ef database update \
  --project src/DevBoard.Infrastructure \
  --startup-project src/DevBoard.API

# Correr la API
dotnet run --project src/DevBoard.API
```

La documentación de la API estará disponible en `http://localhost:{puerto}/scalar/v1`.

### Configuración

Crear `src/DevBoard.API/appsettings.Development.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "tu-clave-secreta-de-al-menos-32-caracteres"
  }
}
```

`appsettings.json` ya incluye el resto de la configuración con valores por defecto para desarrollo local.

---

## Estructura del repositorio

```
DevBoard/
├── src/
│   ├── DevBoard.Domain/
│   │   ├── Common/          # BaseEntity, SoftDeletableEntity
│   │   └── Entities/        # User, JobApplication, ApplicationStatus, Contact, Interview
│   ├── DevBoard.Application/
│   │   ├── Common/          # Result pattern, JwtSettings
│   │   ├── DTOs/            # Request/Response records
│   │   ├── Features/        # Commands y Queries por feature
│   │   └── Interfaces/      # IUnitOfWork, ITokenService, IPasswordHasher, repositorios
│   ├── DevBoard.Infrastructure/
│   │   ├── Persistence/     # DbContext, UnitOfWork, configuraciones Fluent API, migraciones
│   │   ├── Repositories/    # Implementaciones concretas
│   │   └── Services/        # TokenService, PasswordHasher
│   └── DevBoard.API/
│       ├── Controllers/     # Auth, JobApplications, Contacts, Interviews, Dashboard
│       ├── Middleware/      # ExceptionHandlingMiddleware
│       └── Program.cs
└── README.md
```
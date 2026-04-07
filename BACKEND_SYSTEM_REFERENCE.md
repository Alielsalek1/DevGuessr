# Techdle Backend System Reference

Last updated: 2026-04-07

This document explains how the backend is implemented end-to-end, including:
- ASP.NET Core API architecture (Clean Architecture layering)
- Feature implementation chains across API/Application/Infrastructure/Domain
- Security, validation, idempotency, rate limiting
- Docker and Nginx runtime/deployment model
- Data model and persistence behavior
- Testing strategy and current gaps

It is intended as a reference for both humans and LLMs.

---

## 1. System Overview

### 1.1 Tech Stack
- Runtime: .NET 9 (`net9.0`)
- API Framework: ASP.NET Core Web API
- Architecture: Clean Architecture (`Src/API`, `Src/Application`, `Src/Infrastructure`, `Src/Domain`)
- DB: PostgreSQL (EF Core + Npgsql)
- Cache: Redis (`IDistributedCache`)
- Jobs: Hangfire (PostgreSQL storage)
- Messaging: MassTransit + RabbitMQ (configured)
- Reverse Proxy / TLS / edge rate limiting: Nginx
- Logging: Serilog + optional Seq sink
- Tests: xUnit + ASP.NET Core integration testing + Testcontainers

### 1.2 Layering and Dependency Direction
- `API` depends on `Application` and orchestrates HTTP concerns.
- `Application` contains use-cases, DTOs, service interfaces, business orchestration.
- `Infrastructure` implements interfaces declared in `Application` (repositories, file storage, cache wiring, message broker wiring).
- `Domain` contains entities, enums, guards, constraints, exceptions.

Dependency direction is inward:
- `API -> Application <- Infrastructure`
- `Domain` is the core model layer and is referenced by `Application` and `Infrastructure`.

---

## 2. Runtime Request Lifecycle

## 2.1 Deployment Edge Flow
1. Client hits Nginx (`80`/`443`).
2. HTTP requests are redirected to HTTPS.
3. Nginx applies:
- TLS termination
- security headers
- edge rate limiting (`api_limit` and stricter `login_limit`)
- proxying to upstream `api:8080`
- selective caching for GET responses
4. Request reaches ASP.NET Core API container.

Files:
- `docker-compose.yml`
- `Nginx/nginx.conf`
- `Src/API/Dockerfile`

## 2.2 ASP.NET Core Pipeline
In `Src/API/Program.cs`:
1. Environment variables loaded with DotNetEnv.
2. Options loaded via `AddAppOptions()`.
3. Kestrel limits configured (body/header/request sizes).
4. Serilog configured (console, file, optional Seq in development).
5. Layer DI registrations:
- `AddDomain()`
- `AddInfrastructure()`
- `AddApplication()`
- `AddApiLayer(...)`
6. Middleware pipeline (relevant sequence):
- `UseSerilogRequestLogging()`
- `UseForwardedHeaders()`
- `UseCors("AllowAll")` (currently always enabled)
- `UseRateLimiter()` (disabled only in `Testing` env)
- `UseMiddleware<GlobalExceptionHandlerMiddleware>()`
- `UseHangfireDashboard("/hangfire")`
- `UseStaticFiles(...)` for uploaded assets
- `UseAuthentication()` / `UseAuthorization()`
- `MapControllers()`
- `MapHealthChecks(...)`

Note: Controllers are globally protected by default auth policy in API DI, with explicit `[AllowAnonymous]` on public endpoints.

---

## 3. Composition Root and DI

## 3.1 API Layer Registration
`Src/API/DependencyInjection.cs` wires:
- JWT authentication and authorization
- controller behavior + validation filter
- rate limiter (fixed-window per remote IP)
- API versioning (URL segment `v{version}`)
- idempotency filter registration
- forwarded headers config
- Hangfire server + PostgreSQL storage
- OpenAPI docs (development only)

## 3.2 Application Layer Registration
`Src/Application/DependencyInjection.cs` wires:
- FluentValidation validators
- email sender stack (`FluentEmail` + SMTP via options)
- OTP service strategy registrations
- auth services (internal/external/session/password reset/etc.)
- domain feature services (users, programming languages, logodle targets, technection categories, techdle player)

## 3.3 Infrastructure Layer Registration
`Src/Infrastructure/DependencyInjection.cs` wires:
- Npgsql data source + EF Core `AppDbContext`
- repository implementations
- file storage service
- Redis distributed cache options
- MassTransit RabbitMQ transport

---

## 4. API Surface (Controllers and Endpoints)

All routes are versioned under `api/v{version:apiVersion}/...` and currently use `v1`.

### 4.1 InternalAuthController
File: `Src/API/Controllers/InternalAuthController.cs`

Endpoints:
- `POST /internal-auth/register` `[AllowAnonymous]` `[Idempotent]`
- `POST /internal-auth/promote/guest` `[Idempotent]`
- `POST /internal-auth/login/guest` `[AllowAnonymous]` `[Idempotent]`
- `POST /internal-auth/login` `[AllowAnonymous]`
- `POST /internal-auth/confirm-login` `[AllowAnonymous]`
- `POST /internal-auth/confirm-email` `[AllowAnonymous]`
- `POST /internal-auth/resend-confirmation-email` `[AllowAnonymous]`
- `POST /internal-auth/forget-password` `[AllowAnonymous]`
- `POST /internal-auth/reset-password` `[AllowAnonymous]`
- `POST /internal-auth/refresh-token` `[AllowAnonymous]`

Notes:
- refresh token is managed via secure HttpOnly cookie (`refreshToken`).
- device trust uses secure cookie (`deviceId`).
- confirms new device login via OTP flow.

### 4.2 ExternalAuthController
File: `Src/API/Controllers/ExternalAuthController.cs`

Endpoints:
- `POST /external-auth/login/google` `[AllowAnonymous]`
- `POST /external-auth/link/google` (requires authenticated user)

Notes:
- validates Google `id_token`, creates/links users, then issues JWT + refresh token.

### 4.3 UsersController
File: `Src/API/Controllers/UsersController.cs`

Endpoints:
- `GET /users/profile`
- `PATCH /users/profile` `[Idempotent]`

### 4.4 ProgrammingLanguagesController (Admin)
File: `Src/API/Controllers/ProgrammingLanguagesController.cs`

Role requirement:
- `[Authorize(Roles = "Admin")]`

Endpoints:
- `GET /programming-languages`
- `GET /programming-languages/{name}`
- `POST /programming-languages`
- `PUT /programming-languages/{name}`
- `POST /programming-languages/{name}/tags`
- `DELETE /programming-languages/{name}/tags`
- `DELETE /programming-languages/{name}`

### 4.5 LogodleTargetsController (Admin)
File: `Src/API/Controllers/LogodleTargetsController.cs`

Role requirement:
- `[Authorize(Roles = "Admin")]`

Endpoints:
- `GET /logodle-targets`
- `GET /logodle-targets/{name}`
- `POST /logodle-targets` (`multipart/form-data`)
- `DELETE /logodle-targets/{name}`

### 4.6 TechnectionCategoriesController (Admin)
File: `Src/API/Controllers/TechnectionCategoriesController.cs`

Role requirement:
- `[Authorize(Roles = "Admin")]`

Endpoints:
- `GET /technection-categories`
- `GET /technection-categories/{groupName}`
- `POST /technection-categories`
- `PUT /technection-categories/{groupName}`
- `POST /technection-categories/{groupName}/words`
- `DELETE /technection-categories/{groupName}/words`
- `DELETE /technection-categories/{groupName}`

### 4.7 TechdleController
File: `Src/API/Controllers/TechdleController.cs`

Endpoints:
- `POST /techdle/guess`

---

## 5. Feature Implementation Chains (Across Layers)

## 5.1 Internal Auth (Register/Login/Refresh/Reset/Guest)
API:
- `InternalAuthController`

Application:
- Facade: `InternalAuthFacadeService`
- Register/guest promotion: `InternalRegisterationService`
- Session/login/refresh/device confirmation: `InternalSessionService`
- Email verification: `InternalUserVerificationService`
- Password reset: `InternalPasswordResetService`
- OTP engine: `OtpService<T>` + `IOtpStrategy<T>`
- Token utilities: `JwtTokenProvider`, `RefreshTokenProvider`
- Login throttling: `LoginThrottlingService` (Redis-backed)

Infrastructure:
- Repositories: `UserRepository`, `UserDevicesRepository`, `UserRefreshTokensRepository`
- Cache backend for OTP and throttling: Redis (`IDistributedCache`)
- Background emails: Hangfire (`IBackgroundJobClient`) + SMTP sender classes

Domain:
- Entities: `User`, `UserDevice`, `UserRefreshToken`
- Guards: user constraints and validation helpers

Important behavior details:
- password hashing uses bcrypt.
- refresh token stored hashed (SHA-256 hex string).
- token reuse grace check exists (`40` seconds) before invalidating replay.
- unknown device login triggers OTP email and confirmation step.
- guest login creates user with `Guest` role, then can be promoted.

## 5.2 External Auth (Google OAuth)
API:
- `ExternalAuthController`

Application:
- `ExternalAuthService`
- `GoogleAuthValidator`

Infrastructure:
- `UserRepository`, `UserRefreshTokensRepository`

Domain:
- `User` with external creation/update path

Flow summary:
1. Validate Google `id_token` against configured audience.
2. Find user by `GoogleId`; fallback to email; create/link as needed.
3. Generate access token and refresh token.
4. Persist refresh token hash.

## 5.3 User Profile
API:
- `UsersController`

Application:
- `UserFacadeService`
- `UserService`

Infrastructure:
- `UserRepository`

Domain:
- `User` entity updates (`UpdateAddress`, `UpdatePhoneNumber`)

## 5.4 Programming Languages (Admin CRUD)
API:
- `ProgrammingLanguagesController`

Application:
- `ProgrammingLanguageService`

Infrastructure:
- `ProgrammingLanguageRepository`

Domain:
- `ProgrammingLanguage` entity with guard-validated updates/tag operations

## 5.5 Logodle Targets (Admin CRUD + images)
API:
- `LogodleTargetsController`

Application:
- `LogodleTargetService`

Infrastructure:
- `LogodleTargetRepository`
- `FileStorageService`

Domain:
- `LogodleTarget`

Image behavior:
- save original file under configured upload path
- generate 5 blurred variants using ImageSharp Gaussian blur sigmas `[1, 4, 8, 14, 22]`
- store relative URLs in DB

## 5.6 Technection Categories (Admin CRUD + words)
API:
- `TechnectionCategoriesController`

Application:
- `TechnectionCategoryService`

Infrastructure:
- `TechnectionCategoryRepository`

Domain:
- `TechnectionCategory`

## 5.7 Techdle Guess Evaluation
API:
- `TechdleController`

Application:
- `TechdlePlayerService`

Infrastructure:
- `DailyTechdleRepository` (loads puzzle with target language)
- `ProgrammingLanguageRepository` (guessed language lookup)
- Redis cache for same-day puzzle target data

Domain:
- `DailyTechdle`
- `ProgrammingLanguage`
- enums like `TypingDiscipline`, `TypeStrength`, `MatchStatus`

Evaluation logic compares:
- release year (higher/lower/match)
- typing discipline + type strength (miss/partial/match)
- tags overlap (miss/partial/match)

---

## 6. Cross-Cutting Concerns

## 6.1 Authentication and Authorization
Files:
- `Src/API/DependencyInjection.cs`
- `Src/Application/Services/Implementations/Misc/JwtTokenProvider.cs`
- `Src/API/Extensions/ControllerBaseExtensions.cs`

Details:
- JWT Bearer auth configured using options from environment.
- Token validation checks issuer, audience, lifetime, signing key, algorithm (`HS256`).
- Global auth policy on controllers (`RequireAuthenticatedUser`), overridden by `[AllowAnonymous]`.
- Role-based authorization on admin controllers.

## 6.2 Validation
Files:
- `Src/API/ActionFilters/ValidationFilter.cs`
- `Src/Application/DependencyInjection.cs` (FluentValidation registration)
- domain guard classes under `Src/Domain/Constraints/**`

Details:
- model validation failures return structured `FailApiResponse`.
- domain constructors/mutators enforce invariants via guard methods.

## 6.3 Idempotency
Files:
- `Src/API/ActionFilters/IdempotencyFilter.cs`
- `Src/API/ActionFilters/IdempotentAttribute.cs`

Behavior:
- requires `Idempotency-Key` header.
- computes request body hash (SHA-256) to detect key reuse with different payloads.
- caches successful response for 24h in Redis.

## 6.4 Rate Limiting
- App-level limiter (`AddRateLimiter`) uses fixed window per remote IP: `60 req / 1 min`.
- Nginx also enforces edge limits:
- general API zone: `10r/s`
- strict auth endpoints: `1r/s`

## 6.5 Global Exception Handling
File:
- `Src/API/Middlewares/GlobalExceptionHandlerMiddleware.cs`

Behavior:
- catches unhandled exceptions and returns `FailApiResponse`.
- maps `DomainException` and `DbException` separately.
- default fallback message hides internals.

## 6.6 Response Contract Mapping
File:
- `Src/API/Extensions/ControllerBaseExtensions.cs`

Pattern:
- application services return `Result<T>`.
- controller extension converts success/failure to HTTP status + standard response envelope.

---

## 7. Persistence and Data Model

## 7.1 DbContext
File:
- `Src/Infrastructure/Persistance/AppDbContext.cs`

DbSets:
- `Users`
- `UserDevices`
- `UserRefreshTokens`
- `ProgrammingLanguages`
- `LogodleTargets`
- `TechnectionCategories`
- `DailyTechdles`

## 7.2 Entity Configuration Highlights
Files under:
- `Src/Infrastructure/Persistance/Configurations/`

Important mappings:
- `User`: unique indexes for `GoogleId`, `Username`, `Email`.
- `UserDevice`: composite key `(UserId, DeviceId)`, FK to `User`.
- `UserRefreshToken`: composite key `(UserId, RefreshTokenHash)`, FK to `User`.
- `ProgrammingLanguage.Tags`: `jsonb`.
- `LogodleTarget.BlurredImageUrls`: `jsonb`.
- `TechnectionCategory.Words`: `jsonb`.
- `DailyTechdle.PuzzleDate`: unique index; FK to `ProgrammingLanguage` with restrict delete.

## 7.3 Domain Entities
Core entities in `Src/Domain/Models/**`:
- `User`
- `UserDevice`
- `UserRefreshToken`
- `ProgrammingLanguage`
- `LogodleTarget`
- `TechnectionCategory`
- `DailyTechdle`

Design style:
- private setters + guarded mutation methods.
- constructor validation to enforce invariants at entity boundary.

## 7.4 Migrations
Folder:
- `Src/Infrastructure/Migrations/`

Observed migration history includes:
- initial schema
- logodle additions
- technection categories
- daily techdle

---

## 8. Infrastructure and Deployment

## 8.1 Docker Compose Services
File:
- `docker-compose.yml`

Services:
- `postgres`
- `redis`
- `rabbitmq`
- `api`
- `nginx`
- `seq`

Volumes:
- persistent postgres data
- nginx cache
- uploaded images

## 8.2 API Container
File:
- `Src/API/Dockerfile`

Build model:
- multi-stage build (SDK -> publish -> ASP.NET runtime)
- exposes `8080`
- includes container healthcheck on `/health`

## 8.3 Nginx Responsibilities
File:
- `Nginx/nginx.conf`

Configured for:
- HTTPS termination and HTTP->HTTPS redirect
- TLS protocols/ciphers
- security headers (including HSTS)
- proxying to API upstream
- edge rate limiting
- response caching for GET requests
- static serving for `Tests/test_oauth`

## 8.4 Health and Observability
API health endpoints:
- `/health`
- `/health/ready` (DB/Redis/Hangfire readiness tags)
- `/health/auth` (requires authorization)

Logging:
- Serilog console + rolling files + optional Seq in dev.

---

## 9. Configuration and Options

## 9.1 Environment Variable Loader
File:
- `Src/API/Utils/EnvironmentVariableLoader.cs`

Purpose:
- enforce required env vars
- build grouped email config

## 9.2 Options Binding
File:
- `Src/API/Extensions/OptionsExtensions.cs`

Options classes configured from env:
- `JwtOptions`
- `EmailOptions`
- `DatabaseOptions`
- `RedisOptions`
- `RabbitMqOptions`
- `FileStorageOptions`

---

## 10. Testing Architecture

## 10.1 Test Setup
- Project: `Tests/Techdle.Tests.csproj`
- Test framework: xUnit
- Integration host factory: `Tests/Common/CustomWebApplicationFactory.cs`
- Base class: `Tests/Common/BaseIntegrationTest.cs`

## 10.2 Test Infrastructure
Uses Testcontainers and providers for:
- PostgreSQL
- Redis
- RabbitMQ
- MailHog

Also includes:
- DB reset support via Respawn provider
- fake remote IP middleware to control IP-dependent behavior
- test double for Google token validator

## 10.3 Integration Test Coverage Areas
Folders under `Tests/IntergrationTests/`:
- `Auth/` (register, login, refresh, password reset, OAuth link/login, guest flows, security)
- `User/`
- `ProgrammingLanguages/`
- `LogodleTargets/`
- `TechnectionCategories/`
- `TechdlePlayer/`

## 10.4 Stress Tests
- `Tests/StressTests/ApiStressTests.cs`
- load scenarios include guest login, registration, health checks, mixed endpoint tests.

---

## 11. Security Controls Inventory

Implemented controls:
- JWT auth with issuer/audience/key/lifetime validation
- role-based authorization for admin APIs
- global auth requirement + explicit anonymous overrides
- request DTO validation + domain guards
- idempotency key filter on selected write endpoints
- API-level and Nginx-level rate limiting
- secure refresh token/device cookies (`HttpOnly`, `Secure`, `SameSite=Strict`)
- forwarded header handling for reverse proxy setups
- centralized exception handling and structured failure envelopes
- TLS and security headers at Nginx edge

---

## 12. Known Gaps and Observations

1. `README.md` is currently placeholder text and does not document system usage.
2. CORS policy `AllowAll` is enabled in runtime pipeline (comment says testing-only, but it is active unless changed).
3. MassTransit/RabbitMQ is configured, but no consumers/producers were found in this codebase path scan.
4. Hangfire is configured and used for fire-and-forget email jobs, but no recurring jobs were identified.
5. Hangfire dashboard is mapped at `/hangfire`; access control is via global auth policy (no explicit admin-only policy in `Program.cs`).
6. `TechdlePlayerService` reads from cache but does not write puzzle data back to cache in current implementation.
7. API package list includes `AspNetCoreRateLimit`, while active implementation uses built-in `AddRateLimiter`.

---

## 13. Developer Operations Quick Reference

Setup docs:
- `DOCKER_SETUP.md`
- `commands.md`

Common lifecycle:
1. Copy env template and configure `.env`.
2. Start stack: `docker compose up -d --build`.
3. Verify health via `http://localhost/health`.
4. API docs in development: `http://localhost/api-docs`.
5. Hangfire dashboard: `http://localhost/hangfire`.

EF migrations (as documented in `commands.md`):
- add migration in `Infrastructure` with startup project `API`
- apply update against active connection string

---

## 14. File Map for Fast Navigation

Bootstrap and host:
- `Src/API/Program.cs`
- `Src/API/DependencyInjection.cs`

API endpoints:
- `Src/API/Controllers/InternalAuthController.cs`
- `Src/API/Controllers/ExternalAuthController.cs`
- `Src/API/Controllers/UsersController.cs`
- `Src/API/Controllers/ProgrammingLanguagesController.cs`
- `Src/API/Controllers/LogodleTargetsController.cs`
- `Src/API/Controllers/TechnectionCategoriesController.cs`
- `Src/API/Controllers/TechdleController.cs`

Cross-cutting API:
- `Src/API/ActionFilters/IdempotencyFilter.cs`
- `Src/API/ActionFilters/ValidationFilter.cs`
- `Src/API/Middlewares/GlobalExceptionHandlerMiddleware.cs`
- `Src/API/Extensions/ControllerBaseExtensions.cs`
- `Src/API/Extensions/OpenApiExtensions.cs`
- `Src/API/Extensions/OptionsExtensions.cs`

Application core services:
- `Src/Application/DependencyInjection.cs`
- `Src/Application/Services/Implementations/Auth/**`
- `Src/Application/Services/Implementations/Misc/**`
- `Src/Application/Services/Implementations/User/**`
- `Src/Application/Services/Implementations/ProgrammingLanguage/**`
- `Src/Application/Services/Implementations/LogodleTarget/**`
- `Src/Application/Services/Implementations/TechnectionCategory/**`
- `Src/Application/Services/Implementations/TechdlePlayerService.cs`

Infrastructure:
- `Src/Infrastructure/DependencyInjection.cs`
- `Src/Infrastructure/Persistance/AppDbContext.cs`
- `Src/Infrastructure/Persistance/Configurations/**`
- `Src/Infrastructure/Repositories/Implementations/**`
- `Src/Infrastructure/Common/Services/FileStorageService.cs`
- `Src/Infrastructure/Migrations/**`

Domain:
- `Src/Domain/Models/**`
- `Src/Domain/Constraints/**`
- `Src/Domain/Enums/**`
- `Src/Domain/Exceptions/**`

Deployment and edge:
- `docker-compose.yml`
- `Src/API/Dockerfile`
- `Nginx/nginx.conf`

Testing:
- `Tests/Common/**`
- `Tests/IntergrationTests/**`
- `Tests/StressTests/ApiStressTests.cs`

---

## 15. Suggested Next Documentation Additions

To make this reference even more complete, consider adding:
1. Sequence diagrams for auth flows (register/login/refresh/new-device confirmation).
2. An ER diagram generated from current EF mappings.
3. `.env.example` annotated reference with defaults and security notes.
4. OpenAPI-generated endpoint table snapshot for contracts.
5. Explicit production hardening checklist (CORS, dashboard auth policy, secrets management, cert rotation).

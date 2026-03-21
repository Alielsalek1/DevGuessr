# Technology Reference

This document summarizes the technologies and implementation patterns used in `Techdle`.

For architectural decision rationale, see:

- `docs/adr/001-backend-architecture-and-technology-stack.md`

## Runtime Stack

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core (Npgsql provider)
- PostgreSQL (primary relational database)
- Redis (idempotency and cache scenarios)
- RabbitMQ + MassTransit (message bus)
- Hangfire + Hangfire.PostgreSql (background jobs)
- Serilog + Seq (structured logging)

## Auth and Security

- JWT access/refresh token authentication
- Google OAuth external login
- BCrypt password hashing
- API versioning via `Asp.Versioning`
- Rate limiting (disabled only in testing environment)
- Request-size and header limits configured at Kestrel level

## Validation and Mapping

- FluentValidation for DTO validation
- Filter-based request validation handling
- Mapster for DTO/entity mappings

## Email Delivery

- FluentEmail SMTP sender
- Scoped SMTP sender setup for safer concurrent usage
- Email workflows dispatched asynchronously using Hangfire

## Key Architectural Patterns

### Clean Architecture

- `API`: HTTP endpoints, middleware, filters
- `Application`: use cases, contracts, business orchestration
- `Domain`: entities and core rules
- `Infrastructure`: persistence and external system implementations

### Result Pattern

Application services return explicit success/failure results for expected outcomes, keeping failure handling predictable and testable.

### Idempotency Filter

Selected endpoints use an idempotency key and Redis-backed storage to avoid duplicate processing when clients retry requests.

### OTP Strategy Pattern

Device confirmation, registration verification, and password reset use OTP strategy abstractions for consistent generation and validation logic.

## Observability

- Console logging for local diagnostics
- Rolling file logs in `Src/API/Logs`
- Seq sink in Development environment
- Request logging via Serilog middleware

## Testing Stack

- xUnit test framework
- Testcontainers for PostgreSQL, Redis, RabbitMQ, and MailHog
- Respawn for deterministic database reset between tests
- Integration-heavy test strategy with real infrastructure dependencies

## Operational Notes

- Hangfire data is persisted in PostgreSQL (schema/table set managed by Hangfire).
- Test infrastructure is configured to avoid clearing Hangfire internal tables during DB resets.
- Email assertions in tests use polling because background jobs are asynchronous.

## Related Files

- `README.md`
- `DOCKER_SETUP.md`
- `docs/adr/README.md`


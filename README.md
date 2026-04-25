# DevGuessr

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![CI](https://github.com/Alielsalek1/DevGuessr/actions/workflows/main.yml/badge.svg)](https://github.com/Alielsalek1/DevGuessr/actions)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://makeapullrequest.com)

A suite of daily developer guessing games. Live at [devguessr.site](https://devguessr.site).

![Social Preview](Frontend/public/social-preview.jpg)

## Games

### Langdle

Guess the programming language. Each guess returns attribute-level feedback:

| Attribute | Feedback |
|-----------|----------|
| **Language** | The guessed language name |
| **Year** | Higher / Lower / Match |
| **Type Checking** | Match / Miss |
| **Memory** | Match / Miss |
| **Scope Syntax** | Match / Miss |
| **Semicolons** | Match / Miss |
| **Tags** | Match / Partial / Miss |

### Logodle

Identify a tech logo through 5 tiers of blur and pixelation. Each wrong guess reveals a clearer image. 6 attempts max.

### Mythdle

Six technical concepts are presented. 2 Easy, 2 Medium, 1 Hard, and 1 Fake. Find the myth in 2 tries.

## Features

- **Daily Puzzles** — New Langdle, Logodle, and Mythdle challenges every day at UTC midnight
- **Daily Quest Tracker** — Home page tracks completion across all 3 games with a progress bar and countdown timer to next reset
- **Past Drops Archive** — Replay previous daily challenges
- **How to Play Docs** — In-app documentation page explaining each game mode
- **SEO Optimized** — Dynamic canonical URLs, OpenGraph/Twitter meta tags, JSON-LD structured data, and sitemap
- **Admin API** — Full CRUD for Langdle targets, Logodle targets, Mythdle targets, and Clusterdle categories (role-protected)
- **Auth System** — Registration, login, email OTP verification, new-device confirmation, password reset, guest accounts, and Google OAuth
- **Idempotency** — Redis-backed idempotency keys on write endpoints (24h TTL, SHA-256 body hashing)
- **Rate Limiting** — Dual layer: ASP.NET fixed-window (60 req/min per IP) + Nginx edge limits
- **Security Headers** — HSTS, CSP, X-Frame-Options, X-Content-Type-Options via Nginx

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **API** | .NET 9 · ASP.NET Core · Clean Architecture |
| **Database** | PostgreSQL 16 · EF Core · Npgsql |
| **Cache** | Redis 7 |
| **Queue** | RabbitMQ · MassTransit |
| **Jobs** | Hangfire (PostgreSQL storage) |
| **Validation** | FluentValidation |
| **Email** | FluentEmail · SMTP |
| **Auth** | JWT (HS256) · Google OAuth · bcrypt · OTP |
| **Frontend** | Angular 20 · Tailwind CSS 4 · TypeScript |
| **Fonts** | Space Grotesk · JetBrains Mono · Material Symbols |
| **Proxy** | Nginx (TLS, gzip, response caching, security headers) |
| **Logging** | Serilog · Seq |
| **Testing** | xUnit · Testcontainers (PostgreSQL, Redis, RabbitMQ, MailHog) · Respawn |
| **CI/CD** | GitHub Actions (build → test → publish to GHCR → deploy via self-hosted runner) |
| **Container Registry** | GitHub Container Registry (GHCR) |
| **API Versioning** | URL segment (`/api/v1/...`) |

## Quick Start

### Prerequisites

- Docker & Docker Compose

### Setup

```bash
# Clone
git clone https://github.com/Alielsalek1/DevGuessr.git
cd DevGuessr

# Configure
cp .env.example .env
# Edit .env and fill in the values marked below, then:

# Run
docker compose up -d --build
```

### Environment Variables

Most defaults in `.env.example` work out of the box. You **must** fill in these before running:

| Variable | Purpose | Notes |
|----------|---------|-------|
| `EMAIL_HOST` | SMTP server host | e.g. `smtp.gmail.com` or `mailhog` for local |
| `EMAIL_PORT` | SMTP port | e.g. `587` (TLS) or `1025` (MailHog) |
| `EMAIL_USERNAME` | SMTP username | Your email or leave blank for MailHog |
| `EMAIL_PASSWORD` | SMTP password | App password or leave blank for MailHog |
| `EMAIL_ENABLE_SSL` | Enable TLS | Set `false` for MailHog |
| `JWT_KEY` | Token signing key | **Change from default** — use a long random string |
| `Google__ClientId` | Google OAuth client ID | Optional — only needed for Google login |
| `Google__ClientSecret` | Google OAuth client secret | Optional — only needed for Google login |

### Endpoints

| Endpoint | URL |
|----------|-----|
| App | `http://localhost` |
| API Docs | `http://localhost/api-docs` |
| Hangfire | `http://localhost/hangfire` |
| Seq Logs | `http://localhost:5341` |

See [DOCKER_SETUP.md](DOCKER_SETUP.md) for full configuration reference.

## Architecture

```
Backend/
├── API/                Controllers, Middlewares, Action Filters, DI
├── Application/        Services, DTOs, Validators, Interfaces
├── Infrastructure/     EF Core, Repositories, Migrations, File Storage
└── Domain/             Entities, Enums, Guards, Exceptions

Frontend/
├── features/           langdle/ · logodle/ · mythdle/
├── pages/              home · archive · docs · about
├── core/               SEO service, API client, guards, interceptors
└── layout/             App shell

Nginx/                  Reverse proxy, TLS termination, caching, security headers
Tests/                  Integration tests (Auth, Langdle, Logodle, Mythdle, Clusterdle, User)
Data/                   Game datasets (CSV)
.github/workflows/      CI (build + test + publish) · CD (self-hosted deploy)
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

MIT — see [LICENSE](LICENSE).

## Security

Report vulnerabilities privately — see [SECURITY.md](SECURITY.md).

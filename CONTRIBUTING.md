# Contributing to DevGuessr

First off, thank you for considering contributing to DevGuessr! It's people like you that make DevGuessr such a great tool for the developer community.

## How Can I Contribute?

### Reporting Bugs
- Use the **Bug Report** template when opening an issue.
- Describe the bug clearly and provide steps to reproduce.
- Include environment details (OS, Browser, etc.).

### Suggesting Enhancements
- Use the **Feature Request** template.
- Explain why this enhancement would be useful.

### Pull Requests
1. Fork the repository.
2. Create a new branch (`git checkout -b feature/amazing-feature`).
3. Make your changes.
4. Run tests to ensure everything is working correctly.
5. Commit your changes (`git commit -m 'Add some amazing feature'`).
6. Push to the branch (`git push origin feature/amazing-feature`).
7. Open a Pull Request.

## Development Setup

### Prerequisites
- Docker & Docker Compose
- .NET 9 SDK (for local development without Docker)
- Node.js & npm (for frontend development)

### Running with Docker
Follow the instructions in [DOCKER_SETUP.md](file:///home/alilililili/Projects/techdle/DOCKER_SETUP.md) to get the full stack running.

### Project Structure
- `Backend/`: ASP.NET Core Web API (Clean Architecture).
- `Frontend/`: Angular application.
- `Nginx/`: Reverse proxy configuration.
- `Tests/`: Integration and stress tests.

## Coding Standards
- Follow .NET coding conventions for the backend.
- Use Prettier for frontend formatting.
- Ensure all new features are covered by tests.

## Community
By participating in this project, you agree to abide by our [Code of Conduct](file:///home/alilililili/Projects/techdle/CODE_OF_CONDUCT.md).

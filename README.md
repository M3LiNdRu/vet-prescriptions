# VetPrescription

Veterinary prescription generator compliant with Article 105.5 of Regulation (EU) 2019/6 (Catalonia jurisdiction).

## Stack

| Layer | Technology |
|---|---|
| Frontend | React 19 + Vite + TypeScript + TailwindCSS |
| Backend | .NET 10 ASP.NET Core minimal API |
| Database | MongoDB 8.0 |
| PDF generation | QuestPDF |
| Infrastructure | Docker Compose / Fly.io |

## Local development

### Prerequisites

- Docker and Docker Compose
- Node.js 20+ (for frontend-only work)
- .NET 10 SDK (for backend-only work)

### Quickstart

```bash
cp .env.example .env
docker compose up
```

- Frontend: http://localhost:5173
- API: http://localhost:8080
- API docs (Swagger): http://localhost:8080/swagger

## Running tests

### Backend

```bash
cd backend
dotnet test VetPrescription.slnx
```

Integration tests spin up a real MongoDB container automatically via Testcontainers — no external database needed.

### Frontend

```bash
cd frontend
npm install
npm test -- --run
```

## Deployment

The app is deployed to Fly.io. See [docs/fly-deploy.md](docs/fly-deploy.md) for the full setup guide.

Live URLs:
- Frontend: https://vet-prescription-ui.fly.dev
- API: https://vet-prescription-api.fly.dev

## Conventional Commits cheatsheet

| Prefix | Effect |
|---|---|
| `feat:` | Minor version bump |
| `fix:`, `perf:` | Patch version bump |
| `BREAKING CHANGE:` footer | Major version bump |
| `chore:`, `ci:`, `docs:`, `refactor:`, `test:` | No version bump |

Every push to `master` runs CI and, if green, triggers semantic-release to tag the version and update `CHANGELOG.md`.

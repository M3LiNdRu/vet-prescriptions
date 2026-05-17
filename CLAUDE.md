# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A veterinary prescription generator compliant with Article 105.5 of Regulation (EU) 2019/6 (Catalonia jurisdiction). Stack: React 19 + .NET 10 REST API + MongoDB, containerised with Docker Compose.

All architectural decisions, data models, and task breakdowns live in `specs/`.

## Custom Skills

Four project-specific skills are available. Always prefer them over ad-hoc implementation.

| Command | Role |
|---|---|
| `/api-spec <task>` | API Specification Engineer — writes and maintains `api-specs/openapi.yaml` and `api-specs/arazzo.yaml` |
| `/backend <task>` | Senior Backend Engineer — implements .NET 10 vertical slices (Endpoint → Handler → Repository) |
| `/frontend <task>` | Senior Frontend Engineer — implements React features with Vitest + React Testing Library |
| `/sre <task>` | Site Reliability Engineer — manages Docker, Docker Compose, and GitHub Actions pipelines |

Each skill spawns Claude Haiku 4.5 sub-agents for research and exploration tasks.

## Workflow Rules

### 1. API Spec First
The `/api-spec` skill must produce and the user must **explicitly approve** the OpenAPI/Arazzo spec before any `/backend` or `/frontend` implementation begins for that feature. No exceptions.

### 2. One PR Per Feature
Each implemented feature must be delivered as a **single pull request** that includes all of the following that apply:
- API spec changes (`api-specs/`)
- Backend slice (`backend/`)
- Frontend feature (`frontend/`)
- SRE changes (`docker-compose.yml`, Dockerfiles, CI workflows) if the feature requires them

Never split a feature across multiple PRs unless explicitly instructed.

### 3. Conventional Commits
All commits must follow the [Conventional Commits](https://www.conventionalcommits.org/) standard. Semantic release runs automatically on every push to `master` and calculates the version tag from commit messages.

| Prefix | Effect |
|---|---|
| `feat:` | Minor version bump |
| `fix:`, `perf:` | Patch version bump |
| `BREAKING CHANGE:` footer | Major version bump |
| `chore:`, `ci:`, `docs:`, `refactor:`, `test:` | No version bump |

### 4. Bug-Fixing: Test-Driven Development
When fixing a bug, always follow TDD:
1. Write a failing test that reproduces the bug.
2. Confirm the test fails.
3. Apply the fix.
4. Confirm the test passes.
5. Check coverage is still ≥80%.

Never apply a fix before the reproducing test exists.

### 5. When in Doubt, Ask
If requirements are ambiguous, the spec is incomplete, or a design decision has multiple valid options — **stop and ask** before implementing. Do not assume.

## Architecture at a Glance

### Backend (`backend/`)
- Solution: `VetPrescription.slnx` (.NET 10 format)
- Projects: `VetPrescription.Domain`, `VetPrescription.Api`, `VetPrescription.UnitTests`, `VetPrescription.IntegrationTests`
- Pattern: CQRS + vertical slices, **no MediatR**
- Each slice: `Endpoint → Handler (validates + logs + calls repo) → Repository`
- The Handler calls FluentValidation inline, executes the use case, then writes a natural-language Serilog audit log
- Domain project has zero framework dependencies
- PDF generation: QuestPDF inside `GeneratePdfHandler`

### Frontend (`frontend/`)
- Vite + React 19 + TypeScript + TailwindCSS (mobile-first)
- Vertical slices: `src/features/<feature>/<use-case>/` with `Page.tsx`, `Form.tsx`, `api.ts`
- Shared: `src/shared/api-client.ts` (Axios base instance)

### API Specs (`api-specs/`)
- `openapi.yaml` — OpenAPI 3.1, single source of truth for all endpoint contracts
- `arazzo.yaml` — Arazzo 1.0, multi-step workflow scenarios

### Infrastructure
- `docker-compose.yml`: three services — `mongo`, `api`, `frontend`
- `frontend` proxies `/api` requests to the `api` container via nginx
- CI: `ci.yml` (test + coverage) → `release.yml` (semantic-release on `master`)

## Quality Gates

| Gate | Threshold | Enforced by |
|---|---|---|
| Backend coverage | ≥80% | `coverlet` + `dotnet test` |
| Frontend coverage | ≥80% | `@vitest/coverage-v8` |
| API spec exists | 100% | Workflow rule — spec approved before implementation |
| Conventional Commits | 100% | `release.yml` semantic-release |

## Key Reference Files

- `specs/constitution.md` — governing principles, do not violate
- `specs/001-vet-prescription/spec.md` — user stories, acceptance scenarios, functional requirements
- `specs/001-vet-prescription/plan.md` — full architecture, data model, design decisions
- `specs/001-vet-prescription/tasks.md` — ordered task list

# Vet Prescription Tool — Constitution

**Version**: 1.1 | **Date**: 2026-05-17

## Core Principles

1. **Document-first**: Every prescription must produce a valid, printable PDF document. The document is the product — UI is secondary.
2. **Data integrity**: Prescription data must be complete and validated before a document is generated. Partial or ambiguous prescriptions must not be printable.
3. **Simplicity**: Prefer the simplest implementation that produces a correct document. No over-engineering. No unnecessary abstractions or frameworks.
4. **Offline-capable**: The tool must work without internet connectivity. No cloud dependencies for core functionality.
5. **Regulatory compliance**: Prescription fields and layout must follow veterinary prescription regulations (Catalonia, Spain — Art. 105.5, Regulation EU 2019/6).

## Constraints

- No proprietary cloud services for document generation.
- Output must be a standards-compliant PDF.
- No framework-level mediator (MediatR or similar) — validation and logging live inside the Handler directly.

## Observability

- Every significant user action MUST produce a structured audit log entry written in natural language describing who did what and to which resource (e.g. "Veterinary Dr. Joan Puig issued prescription RX-2026-0001" or "Veterinary Dr. Joan Puig printed a copy of prescription RX-2026-0001"). No raw technical log noise in audit entries.

## Quality

- Backend and frontend code MUST each maintain a minimum code coverage ratio of 80%.
- Backend unit tests use **xUnit** and **Moq**. Integration tests use **Microsoft.AspNetCore.Mvc.Testing**.
- Frontend unit tests use **Vitest** and **React Testing Library** (`@testing-library/react`, `@testing-library/user-event`).
- Every REST API endpoint MUST be described by an **OpenAPI** specification. Workflow scenarios (e.g. create → generate PDF) MUST be described using **Arazzo** specification.

## Release Management

- All commits MUST follow the **Conventional Commits** standard (`feat:`, `fix:`, `chore:`, `docs:`, `refactor:`, `test:`, `perf:`, `BREAKING CHANGE:` etc.).
- Every push to `master` triggers **semantic-release**: version tag is calculated from commit messages, `CHANGELOG.md` is updated automatically.
- Version numbers follow **SemVer**: `feat` → minor bump, `fix`/`perf` → patch bump, `BREAKING CHANGE` → major bump.

## Development Workflow

- Spec before code: every feature starts from a written spec.
- Constitution compliance checked before implementation begins.
- OpenAPI/Arazzo specs are written alongside (or before) the endpoint implementation, not after.

## Governance

Amendments to this constitution require updating this file, bumping the version, and noting the date.

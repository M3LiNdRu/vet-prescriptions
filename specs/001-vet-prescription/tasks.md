# Tasks: Veterinary Prescription Generator

**Branch**: `001-vet-prescription` | **Spec**: spec.md | **Plan**: plan.md

---

## Phase 1 — Project Setup

- [ ] T-001 Create solution: `VetPrescription.slnx` (.NET 10 format) with projects `VetPrescription.Domain`, `VetPrescription.Api`, `VetPrescription.UnitTests`, `VetPrescription.IntegrationTests`
- [ ] T-002 Add NuGet packages: FluentValidation.AspNetCore, MongoDB.Driver, QuestPDF, Serilog.AspNetCore, Swashbuckle.AspNetCore, xUnit, Moq, coverlet.collector, Microsoft.AspNetCore.Mvc.Testing
- [ ] T-003 Scaffold Vite + React + TypeScript frontend with TailwindCSS — configure mobile-first responsive breakpoints from the start; add Vitest + React Testing Library + `@testing-library/user-event` + `@vitest/coverage-v8`; configure `vitest.config.ts` with coverage threshold ≥80%
- [ ] T-004 Write `docker-compose.yml` with three services: `mongo`, `api`, `frontend`
- [ ] T-005 Write Dockerfiles for `api` and `frontend`
- [ ] T-006 Configure CORS in API to allow frontend container origin
- [ ] T-007 Configure Serilog in `Program.cs`; register `MongoDbContext` and all slice Handlers in DI
- [ ] T-008 Create `MongoDbContext.cs` with collection accessors for `prescriptions` and `vet_profiles`
- [ ] T-009 Scaffold `TestWebApplicationFactory.cs` in integration tests project
- [ ] T-010 Create `api-specs/openapi.yaml` skeleton with info, servers, and empty paths section
- [ ] T-011 Create `api-specs/arazzo.yaml` skeleton defining the "create prescription → download PDF" workflow
- [ ] T-012 Add `.releaserc.json` with semantic-release plugins: commit-analyzer, release-notes-generator, changelog, git
- [ ] T-013 Add GitHub Actions workflow `.github/workflows/release.yml` — runs semantic-release on push to `master` after tests pass

---

## Phase 2 — Domain Layer

- [ ] T-014 `[P]` Create `Vet.cs` entity
- [ ] T-015 `[P]` Create `Owner.cs` entity
- [ ] T-016 `[P]` Create `Patient.cs` entity
- [ ] T-017 `[P]` Create `PrescriptionItem.cs` entity
- [ ] T-018 Create `Prescription.cs` aggregate root with auto-generated `PrescriptionNumber`

---

## Phase 3 — US1: Create & Generate Prescription (P1)

- [ ] T-019 `[P]` Implement `CreatePrescription` slice (Endpoint → Handler [validates + logs] → Repository) + unit tests + integration test + OpenAPI paths entry
- [ ] T-020 `[P]` Implement `GeneratePdf` slice (Endpoint → Handler [validates + logs] → Repository → QuestPDF) + unit tests + integration test + OpenAPI paths entry
- [ ] T-021 Design PDF layout: header (vet info + clinic), owner section, patient section, prescription items table, date, blank line for physical signature + typed vet name beneath — Catalan/Spanish format (Art. 105.5, EU 2019/6)
- [ ] T-022 `[P]` Implement `CreatePrescriptionPage` and `CreatePrescriptionForm` in frontend (mobile-first) + unit tests (render, user input, form submission with mocked api.ts)
- [ ] T-023 Wire frontend form submission to `POST /api/prescriptions`
- [ ] T-024 Wire "Download PDF" button to `GET /api/prescriptions/{id}/pdf` — triggers browser download on desktop and mobile
- [ ] T-025 Add Arazzo workflow step: create prescription → download PDF

---

## Phase 4 — US2: Vet Profile (P2)

- [ ] T-026 `[P]` Implement `SaveVetProfile` slice (Endpoint → Handler [validates + logs] → Repository, upsert by licenceNumber) + unit tests + integration test + OpenAPI paths entry
- [ ] T-027 `[P]` Implement `GetVetProfile` slice (Endpoint → Handler → Repository) + unit tests + integration test + OpenAPI paths entry
- [ ] T-028 `[P]` Implement `VetProfilePage` and `VetProfileForm` in frontend (mobile-first) + unit tests (render, pre-fill, save interaction with mocked api.ts)
- [ ] T-029 On app load, fetch vet profile and pre-fill prescription form vet fields

---

## Phase 5 — US3: Prescription History (P3)

- [ ] T-030 `[P]` Implement `ListPrescriptions` slice (Endpoint → Handler → Repository, sorted by date desc) + unit tests + integration test + OpenAPI paths entry
- [ ] T-031 `[P]` Implement `GetPrescriptionById` slice (Endpoint → Handler → Repository) + unit tests + integration test + OpenAPI paths entry
- [ ] T-032 `[P]` Implement `PrescriptionListPage` and `PrescriptionListItem` in frontend (mobile-first) + unit tests (render list, empty state, item click)
- [ ] T-033 `[P]` Implement `PrescriptionDetailPage` with reprint button + unit tests (render detail, reprint action with mocked api.ts)

---

## Phase 6 — Polish

- [ ] T-034 Add global error handling middleware in API (return RFC 7807 problem details)
- [ ] T-035 Add form validation feedback in frontend (field-level errors from API)
- [ ] T-036 Add loading states to all async actions in frontend
- [ ] T-037 Verify ≥80% coverage on backend (`dotnet test --collect:"XPlat Code Coverage"`) and frontend (`vitest --coverage`); fail CI build if either is below threshold
- [ ] T-038 Write `.env.example` with all required environment variables documented
- [ ] T-039 Write README with `docker compose up` quickstart and Conventional Commits cheatsheet

---

## Phase 7 — Fly.io Deployment

- [x] T-040 Add `fly/mongo/fly.toml` — MongoDB app config with persistent volume (`mongo_data`)
- [x] T-041 Add `fly/api/fly.toml` — .NET API app config; MongoDB URI injected as Fly secret
- [x] T-042 Add `fly/frontend/fly.toml` — nginx frontend app config with `API_UPSTREAM` build arg pointing to Fly internal DNS
- [x] T-043 Update `frontend/nginx.conf` and `frontend/Dockerfile` to support `API_UPSTREAM` build arg (default: `http://api:8080` for local; override for Fly)
- [x] T-044 Add `.github/workflows/deploy.yml` — deploy all three apps to Fly.io after CI passes on `master`
- [ ] T-045 Create Fly.io apps and volume: `fly apps create vet-prescription-mongo`, `fly apps create vet-prescription-api`, `fly apps create vet-prescription-frontend`, `fly volume create mongo_data --app vet-prescription-mongo --region mad`
- [ ] T-046 Set Fly secrets: `fly secrets set ConnectionStrings__MongoDB="mongodb://vet-prescription-mongo.internal:27017" --app vet-prescription-api`
- [ ] T-047 Add `FLY_API_TOKEN` secret to GitHub repository settings
- [ ] T-048 Write `docs/fly-deploy.md` — beginner guide (concepts, CLI install, first deploy, secrets, logs)

---

`[P]` = can run in parallel with other `[P]` tasks in the same phase (different files, no dependencies)

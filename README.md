# Verus — Company API (Full-Stack)

A company-submission application built for the Verus coding exercise: an **ASP.NET Core Web API**
(Clean Architecture, CQRS, TDD-oriented) with an in-memory data store and a name↔website **relevance**
check, plus a responsive **Angular 20** client. Clients submit a company (name + website URL), the API
validates and persists it, and the data can be retrieved, filtered, and searched by relevance.

- **Backend:** ASP.NET Core Web API · .NET 9 · Clean Architecture · MediatR · FluentValidation · in-memory persistence
- **Frontend:** Angular 20 (standalone + signals) · TailwindCSS · feature-based architecture
- **No database / no auth** — persistence is in-memory behind a repository abstraction, so a database can be dropped in later with no Application-layer changes.

> **.NET version note:** the exercise targets the current .NET SDK; this solution is built and verified
> against the **.NET 9 SDK**. Retargeting to `net10.0` is a one-line change to `<TargetFramework>` in
> each `.csproj` once the .NET 10 SDK is installed — no code changes required.

## Repository layout

```
VerusApplications/
├── README.md            ← you are here — overview, quick-start, API reference
├── backend/
│   └── Verus/           ← ASP.NET Core Web API solution (Verus.sln)
│       ├── Verus.Domain
│       ├── Verus.Application
│       ├── Verus.Infrastructure
│       ├── Verus.WebAPI
│       └── Verus.Tests
└── frontend/            ← Angular 20 client
```

## Architecture at a glance

```
┌──────────────────────────┐            HTTP + CORS              ┌────────────────────────────┐
│   Angular 20 (frontend)   │  ─────────────────────────────────▶ │  ASP.NET Core Web API        │
│  features/companies        │  ◀───────────────────────────────── │  Controllers → MediatR       │
│  signals · reactive forms  │        JSON / ProblemDetails         │  Application → Domain        │
└──────────────────────────┘                                       │  In-memory repository        │
                                                                   └────────────────────────────┘
```

**Backend dependency direction** (Clean Architecture, never violated):
`WebAPI → Infrastructure → Application → Domain`. `Domain` references nothing; `Application` depends only
on abstractions (`ICompanyRepository`, `IRelevanceEvaluator`) that `Infrastructure` implements. Business
logic lives in MediatR use-case handlers — controllers only translate HTTP ↔ MediatR, and no storage or
framework types leak into the Application layer. **Frontend** is feature-based: the `companies` feature is
self-contained (model, service, components, page), and a single HTTP interceptor normalizes every backend
error into one `AppError` shape.

### Bounded-context layout (backend)

```
Verus.Domain/CompanyBoundedContext/
  Entities/CompanyDomain.cs            # entity with behavior + invariants (factory, no public setters)
  Repositories/ICompanyRepository.cs   # persistence abstraction (implemented in Infrastructure)
  Services/WebsiteDomainResolver.cs    # pure registrable-domain extraction
Verus.Application/
  Contracts/Relevance/                 # IRelevanceEvaluator + RelevanceAssessment
  Behaviors/ValidationBehavior.cs      # FluentValidation runs in the MediatR pipeline
  UseCases/CompanyBoundedContext/      # CreateCompany, GetAllCompanies, GetCompanyById, SearchCompanies
Verus.Infrastructure/
  Persistence/InMemory/                # ConcurrentDictionary-backed ICompanyRepository (singleton)
  Relevance/                           # RelevanceEvaluator + TextSimilarity + RelevanceOptions
Verus.WebAPI/
  Controllers/CompaniesController.cs   # thin REST surface
  Extensions/GlobalExceptionHandler.cs # exceptions → RFC 9457 ProblemDetails
```

## API reference

Base URL: `http://localhost:5000` (HTTP) or `https://localhost:7000` (HTTPS). All contracts use DTOs —
the internal `CompanyDomain` entity and the in-memory store are never exposed.

| Method & route | Description | Success | Errors |
| --- | --- | --- | --- |
| `POST /api/companies` | Create a company after validating name, URL, and name↔website relevance | `201 Created` + company + `Location` | `400` ProblemDetails |
| `GET /api/companies` | Retrieve all stored companies | `200 OK` + array (`[]` if none) | — |
| `GET /api/companies/{id}` | Retrieve one company by id | `200 OK` + company | `404 Not Found` |
| `GET /api/companies/search?name=&domain=&term=` | Filter by partial name and/or domain; `term` ranks results by relevance | `200 OK` + array (`[]` if none) | — |

**Company shape (response DTO):**

```json
{
  "id": "01a1378b-e61e-71c8-8f2a-b78152fff833",
  "name": "Acme Widgets",
  "website": "https://www.acmewidgets.com/",
  "websiteDomain": "acmewidgets.com",
  "createdAtUtc": "2026-10-13T23:14:37.7270634Z"
}
```

**Example — a rejected submission (`400`):**

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Website": [
      "Company name 'Acme Widgets' does not appear relevant to 'https://unrelateddomain.com'. Relevance score 0.13 for domain 'unrelateddomain' is below the required 0.35 threshold."
    ]
  }
}
```

## Validation & relevance strategy

Validation runs in the MediatR pipeline via `ValidationBehavior`, so handlers stay focused on the happy
path. The rules from the exercise are enforced in `CreateCompanyValidator`:

- **Company name** — required, 2–200 characters.
- **Website URL** — required, a well-formed absolute `http(s)` URL.
- **Relevance** — the name must be relevant to the website.

The **relevance strategy is domain-matching based**, deterministic, offline, and explainable (no external
calls). `RelevanceEvaluator` normalizes the company name (lower-cased, legal suffixes like *Inc/LLC/Ltd*
stripped) and compares it to the website's registrable label (e.g. `www.acmewidgets.co.uk` → `acmewidgets`)
using four complementary signals, taking the strongest as the score (0–1):

1. **Containment** — the condensed name is inside the domain label, or vice versa.
2. **Token match** — a meaningful word from the name appears in the label.
3. **Acronym** — the initials of the name words form the label (*International Business Machines* ↔ `ibm`).
4. **Fuzzy similarity** — Levenshtein ratio, to tolerate minor spelling/abbreviation differences.

A submission is accepted when the score clears a configurable threshold (`Relevance:MinimumScore`,
default `0.35`). The same evaluator powers relevance-ranked search. Because it sits behind
`IRelevanceEvaluator`, the strategy can be swapped (e.g. for content inspection) without touching callers.

## Quick start

Run the **backend first**, then the frontend.

### 1. Backend — `http://localhost:5000`

Requires the **.NET 9 SDK**.

```bash
cd backend/Verus
dotnet run --project Verus.WebAPI
```

The API starts on `http://localhost:5000` (and `https://localhost:7000` with the `https` profile).
OpenAPI is served at `/openapi/v1.json` in Development. Data is held in memory, so it resets on restart.

Try it with curl:

```bash
# Create (201)
curl -X POST http://localhost:5000/api/companies \
  -H "Content-Type: application/json" \
  -d '{"name":"Acme Widgets","website":"https://www.acmewidgets.com"}'

# List (200)
curl http://localhost:5000/api/companies

# Relevance search (200)
curl "http://localhost:5000/api/companies/search?term=acme"
```

### 2. Frontend — `http://localhost:4200`

Requires **Node 20+**. The dev build points at `http://localhost:5000` (see
`src/environments/environment.development.ts`); CORS on the API already allows `http://localhost:4200`.

```bash
cd frontend
npm install
npm start
```

Open http://localhost:4200. A top nav bar links the pages, each backed by an API capability:

- **All companies** (`/companies`) — lists every stored company (`GET /api/companies`); rows link to detail; empty state when none exist.
- **Add company** (`/companies/new`) — submission form (name + website URL) with inline validation success/error messages (`POST /api/companies`); navigates to the new company on success.
- **Search** (`/companies/search`) — filter by partial name and/or website domain, and enter a relevance term to rank matches (`GET /api/companies/search`); graceful "no matches" state.
- **Company detail** (`/companies/:id`) — single-company view (`GET /api/companies/{id}`) with a graceful not-found state for unknown ids.

## Testing

- **Backend — `Verus.Tests` (NUnit + Moq): 36 tests.** Domain invariants, the relevance evaluator and
  in-memory repository, Application handlers and the `CreateCompanyValidator`, and full HTTP-pipeline
  integration tests via `WebApplicationFactory<Program>` (201/400/404/empty-list/search paths).

  ```bash
  cd backend/Verus && dotnet test
  ```

- **Frontend (Jasmine/Karma): 26 tests.** The `CompanyService` (HTTP contract), the error interceptor's
  ProblemDetails → `AppError` normalization, and the company components/pages (form submit
  success/validation error, list rendering/empty state, all-companies load/error, search criteria +
  empty results, and detail load/404/not-found).

  ```bash
  cd frontend && npm test -- --watch=false --browsers=ChromeHeadless
  ```

## Development conventions

The codebase follows a small set of ground rules that keep the layering honest and the tests meaningful:

1. **TDD first.** For any new endpoint, use case, or component behavior, the failing test comes before the implementation.
2. **Business logic lives in Application use cases**, not in controllers or Angular components. Controllers translate HTTP ↔ MediatR; Angular components handle presentation, logic goes in services.
3. **No storage/framework types leak outside Infrastructure.** The Application layer works against `ICompanyRepository`/`IRelevanceEvaluator` and DTOs — never the `ConcurrentDictionary` or `HttpContext`.
4. **Every endpoint has** an appropriate HTTP verb, request/response DTOs (never the raw `CompanyDomain`), FluentValidation on inputs where applicable, and tests for both the use case and the HTTP surface.
5. **Relevance is a swappable strategy** behind `IRelevanceEvaluator` — the current implementation is deterministic/offline (domain matching) and is never inlined into handlers or controllers.
6. **In-memory only.** No database or persistence files; data resets on restart. A DB-backed `ICompanyRepository` can replace the store without Application-layer changes.

## Design decisions & extensibility

- **In-memory persistence behind `ICompanyRepository`.** The `ConcurrentDictionary` store is private to
  `InMemoryCompanyRepository`; swapping in EF Core / a database means providing one new implementation —
  the Application layer and controllers are untouched.
- **DTOs at every boundary.** `CompanyResponse` / `CreateCompanyRequest` are distinct from `CompanyDomain`,
  keeping the API contract independent of internal entity changes.
- **Consistent errors.** A single `GlobalExceptionHandler` maps validation → `400`, missing resources →
  `404`, and anything unexpected → `500` as RFC 9457 ProblemDetails, which the Angular interceptor parses
  uniformly.
- **Extensible by design.** The RESTful surface and CQRS use-case structure make update, delete, pagination,
  and sorting additive — a new use-case folder plus a controller action, no cross-cutting rewrites.

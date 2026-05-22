# AI Project Handoff

## 1. Purpose of This Document
- This file is the main handoff document for AI/engineers working on this project from different machines.
- Goal: anyone opening this file should quickly understand:
  - what the project is
  - current implementation status
  - important architectural constraints
  - what has already been done
  - what should be done next
  - how to update this document after new work is completed

## 2. Project Overview
- Project name: `NewStart`
- Project type: `Web First + Responsive Core Management Platform`
- Main purpose:
  - build a reusable core platform for system management features such as:
  - user management
  - role / permission / menu management
  - logging / audit
  - error handling
- Future direction:
  - expand to more modules
  - possibly support mobile or other clients later

## 3. Current Architecture Direction
- Architecture style: `Modular Monolith`
- Backend style: `Clean Architecture`
- Frontend direction: `Angular + Tailwind`
- Database direction: `SQL with code-first migrations`
- Current database target: `PostgreSQL`

## 4. Repository Status
- Git repository: initialized
- Remote repository:
  - [pongano/NewStart](https://github.com/pongano/NewStart.git)
- Current known initial commit:
  - `0a93a5e` - `Initialize repository and scaffold backend foundation`
- Working tree status when this file was created:
  - clean

## 5. Current Project Structure
- Root:
  - [PROJECT_PLAN.md](E:\Project\NewStart\PROJECT_PLAN.md)
  - [TODAY_SUMMARY_2026-05-20.md](E:\Project\NewStart\TODAY_SUMMARY_2026-05-20.md)
  - [TODAY_SUMMARY_2026-05-21.md](E:\Project\NewStart\TODAY_SUMMARY_2026-05-21.md)
  - [AI_PROJECT_HANDOFF.md](E:\Project\NewStart\AI_PROJECT_HANDOFF.md)
- Backend:
  - [CoreProject.Backend.slnx](E:\Project\NewStart\Backend\CoreProject.Backend.slnx)
  - `CoreProject.Backend.Domain`
  - `CoreProject.Backend.Application`
  - `CoreProject.Backend.Infrastructure`
  - `CoreProject.Backend.API`
  - `CoreProject.Backend.Application.UnitTests`
  - `CoreProject.Backend.API.IntegrationTests`
- Frontend:
  - folder exists
  - no Angular app scaffolded yet

## 6. Current Backend Implementation Status

### Implemented
- Clean Architecture backend scaffold completed
- Target framework:
  - `.NET 8`
- Dependency injection wiring completed
- PostgreSQL + EF Core wiring completed
- Initial `DbContext` created
- Initial migration created
- Swagger enabled in development
- Health check endpoint added
- Global exception middleware added
- Request logging baseline added
- Placeholder current-user abstraction added
- Sample feature added:
  - `SystemInfo`

### Current API endpoints
- `GET /health`
- `GET /api/system/info`
- `GET /api/system/error`

### Important backend files
- API startup:
  - [Program.cs](E:\Project\NewStart\Backend\CoreProject.Backend.API\Program.cs)
- Sample endpoint:
  - [SystemController.cs](E:\Project\NewStart\Backend\CoreProject.Backend.API\Controllers\SystemController.cs)
- Exception middleware:
  - [ExceptionHandlingMiddleware.cs](E:\Project\NewStart\Backend\CoreProject.Backend.API\Middleware\ExceptionHandlingMiddleware.cs)
- DbContext:
  - [ApplicationDbContext.cs](E:\Project\NewStart\Backend\CoreProject.Backend.Infrastructure\Persistence\ApplicationDbContext.cs)
- DI wiring:
  - [DependencyInjection.cs](E:\Project\NewStart\Backend\CoreProject.Backend.Infrastructure\DependencyInjection.cs)
- Initial migration:
  - [20260520200331_InitialCreate.cs](E:\Project\NewStart\Backend\CoreProject.Backend.Infrastructure\Persistence\Migrations\20260520200331_InitialCreate.cs)

## 7. What Is NOT Implemented Yet
- No authentication flow yet
- No JWT yet
- No real `Identity` module yet
- No real `AccessControl` module yet
- No real `User / Role / Permission / Menu` entities yet
- No frontend app yet
- No live PostgreSQL database update executed in this workspace yet
- No production deployment setup yet

## 8. Important Requirements and Constraints

### Architecture constraints
- Keep backend as `modular monolith`
- Do not split to microservices at this stage
- Keep Clean Architecture boundaries strict:
  - `Domain` must not depend on `Application`, `Infrastructure`, or `API`
  - `Application` must not depend on `Infrastructure` or `API`
  - `Infrastructure` may depend on `Application` and `Domain`
  - `API` may depend on `Application` and `Infrastructure`

### Product constraints
- Current project direction is core platform first, not business-specific features first
- Prioritize reusable system modules over one-off features
- `Role + Permission + Menu` should belong to the same future module area
- Authentication exists in project vision, but implementation is intentionally deferred for now

### Database constraints
- Use `EF Core` with `PostgreSQL`
- Use `code-first` migrations
- Keep schema evolution through migrations only

### Documentation constraints
- Any meaningful work by another AI/machine must update this handoff file
- The goal is that another machine can stop and this machine can continue immediately by reading this file

## 9. Known Good Validation State
- The following were confirmed at scaffold stage:
  - `dotnet restore` passed
  - `dotnet build` passed
  - application unit tests passed
  - API integration tests passed
  - initial EF migration generation passed
- `dotnet ef database update` has not been confirmed against a real PostgreSQL instance in this environment

## 10. Recommended Next Work
- Create `Identity` module skeleton
- Create `AccessControl` module skeleton
- Add first real domain entities for:
  - user
  - role
  - permission
  - menu
- Define first application use cases and contracts
- Configure a real local/dev PostgreSQL connection
- Run database update against PostgreSQL
- Start frontend scaffold after backend module direction is clearer

## 11. Handoff Rules for Another AI / Machine
- Before changing code:
  - read this file first
  - read `PROJECT_PLAN.md`
  - inspect latest git status and latest commit
- Before implementing new modules:
  - confirm they align with modular monolith direction
  - confirm they do not violate Clean Architecture boundaries
- After any meaningful work:
  - update this file
  - update or add a daily summary file
  - commit the documentation together with the code when possible

## 12. Required Update Process

### When this file must be updated
- new module created
- architecture decision changed
- new endpoint added
- migration added or changed
- tests added or removed
- auth/security direction changed
- frontend scaffold started
- deployment/process assumptions changed
- another machine completes a task and hands work back

### Sections that must be updated after each meaningful change
- `Repository Status`
- `Current Project Structure`
- `Current Backend Implementation Status`
- `What Is NOT Implemented Yet`
- `Known Good Validation State`
- `Recommended Next Work`
- `Work Log`

### Minimum update checklist
- what was changed
- which files or areas were affected
- what commands were run to verify
- what passed
- what was not verified
- what should happen next

## 13. Work Log

### 2026-05-20
- Project direction defined
- Chose `modular monolith`
- Decided not to split into many small services
- Defined major module direction:
  - `Identity`
  - `AccessControl`
  - `Logging/Audit`
  - `Configuration`

### 2026-05-21
- Initialized git repository
- Added shared root `.gitignore`
- Scaffolded backend solution using Clean Architecture
- Added EF Core + PostgreSQL setup
- Added sample API endpoints
- Added unit and integration tests
- Added initial migration
- Pushed repository to GitHub

## 14. Update Template For Future Editors
- Copy and append this block under `Work Log` when another machine completes work:

```md
### YYYY-MM-DD
- Machine/agent:
  - <name or short identifier>
- Main outcome:
  - <short summary>
- Code areas changed:
  - <module / folder / file groups>
- Validation performed:
  - <commands or checks>
- Result:
  - <passed / failed / partial>
- Open issues:
  - <important remaining problems>
- Next recommended step:
  - <next action>
```

## 15. Fast Resume Checklist
- Open:
  - [AI_PROJECT_HANDOFF.md](E:\Project\NewStart\AI_PROJECT_HANDOFF.md)
  - [PROJECT_PLAN.md](E:\Project\NewStart\PROJECT_PLAN.md)
- Check:
  - `git status`
  - latest commit
  - current branch
- If continuing backend:
  - inspect `Program.cs`
  - inspect current modules in `Application`, `Domain`, `Infrastructure`, `API`
  - inspect latest migrations
- If starting new feature:
  - update this file before finishing


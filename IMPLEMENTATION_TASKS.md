# Implementation Tasks

## 1. Purpose
- This file is the execution task list for the current project state.
- It is intended for both human developers and AI agents.
- Tasks are ordered by dependency so work can continue with minimal ambiguity.

## 2. Current Baseline
- Repository initialized and pushed to GitHub
- Backend scaffold completed with Clean Architecture
- PostgreSQL + EF Core base setup completed
- Initial migration created
- Sample API endpoints working
- Frontend not scaffolded yet
- Authentication and IAM modules not implemented yet

## 3. Priority Order
- `P0` = must do first
- `P1` = high priority after foundation
- `P2` = important but can come after core modules
- `P3` = later improvement / hardening

## 4. Active Implementation Queue

### P0-01: Configure Local Development Database
- Status: `DONE`
- Goal:
  - prepare a real PostgreSQL connection for local/dev use
- Work:
  - confirm local PostgreSQL instance details
  - update development connection string if needed
  - run `dotnet ef database update`
  - verify database is created successfully
- Acceptance:
  - backend can start with a real PostgreSQL database
  - initial migration is applied successfully
- Progress:
  - Docker Compose PostgreSQL setup prepared with host port `54320` and default database `coreproject_backend_dev`
  - EF Core design-time factory now defaults to the same local development database
  - local `dotnet-ef` tool manifest added
  - PostgreSQL container started successfully and reported healthy
  - initial EF Core migration applied successfully to the Docker database
  - backend API started successfully and `/health` returned `Healthy`

### P0-02: Create Identity Module Skeleton
- Status: `DONE`
- Goal:
  - create the first real module structure for identity-related work
- Work:
  - define module folders in `Domain`, `Application`, `Infrastructure`, `API`
  - add placeholder entity/contracts for user/account concepts
  - keep implementation minimal and aligned with Clean Architecture
- Acceptance:
  - solution contains a clear `Identity` module skeleton
  - no architecture boundary violations
- Progress:
  - added Identity module placeholder namespaces in `Domain`, `Application`, `Infrastructure`, and `API`
  - added placeholder `UserAccount` domain type without persistence mapping
  - intentionally did not add DbContext members, EF configuration, migrations, endpoints, authentication, or JWT

### P0-03: Create AccessControl Module Skeleton
- Status: `TODO`
- Goal:
  - create the first real module structure for `Role / Permission / Menu`
- Work:
  - define module folders in `Domain`, `Application`, `Infrastructure`, `API`
  - keep `Role`, `Permission`, and `Menu` in the same module area
  - add placeholder contracts/entities only as needed
- Acceptance:
  - solution contains a clear `AccessControl` module skeleton
  - module boundaries are ready for future CRUD and authorization logic

## 5. Core Backend Tasks

### P1-01: Implement User Entity and Persistence
- Status: `TODO`
- Goal:
  - add the first real `User` domain model
- Work:
  - define user entity
  - add EF configuration
  - add migration
  - decide minimal required fields for v1
- Suggested minimum fields:
  - `Id`
  - `Username`
  - `Email`
  - `DisplayName`
  - `IsActive`
- Acceptance:
  - user table exists through migration
  - user entity is accessible through persistence layer

### P1-02: Implement Role Entity and Persistence
- Status: `TODO`
- Goal:
  - define `Role` as part of `AccessControl`
- Work:
  - create role entity
  - configure EF mapping
  - prepare migration
- Acceptance:
  - role table exists
  - role model is ready for assignment to users later

### P1-03: Implement Permission Entity and Persistence
- Status: `TODO`
- Goal:
  - define permission structure for API / UI access control
- Work:
  - create permission entity
  - define minimal permission code structure
  - configure EF mapping
  - prepare migration
- Acceptance:
  - permission table exists
  - permission model is ready for role mapping

### P1-04: Implement Menu Entity and Persistence
- Status: `TODO`
- Goal:
  - define menu structure for frontend navigation control
- Work:
  - create menu entity
  - define parent-child/menu ordering structure
  - configure EF mapping
  - prepare migration
- Acceptance:
  - menu table exists
  - menu model is ready to be linked to permissions

### P1-05: Implement User-Role / Role-Permission Relationships
- Status: `TODO`
- Goal:
  - establish the first useful access-control relationships
- Work:
  - define join entities or mappings
  - configure EF relationships
  - prepare migration
- Acceptance:
  - user-role and role-permission structures are persisted correctly

## 6. Application Layer Tasks

### P1-06: Create First User Management Use Cases
- Status: `TODO`
- Goal:
  - support first CRUD workflow for user management
- Work:
  - create `CreateUser`
  - create `GetUserById`
  - create `ListUsers`
  - optionally create `UpdateUserStatus`
- Acceptance:
  - at least one create and one read flow work end-to-end

### P1-07: Create First Role Management Use Cases
- Status: `TODO`
- Goal:
  - support basic role CRUD
- Work:
  - create role create/list/get flows
- Acceptance:
  - role endpoints can be tested end-to-end

### P1-08: Create First Permission / Menu Query Flows
- Status: `TODO`
- Goal:
  - expose permission and menu data for future admin UI
- Work:
  - create list queries
  - add DTOs
- Acceptance:
  - permission and menu data can be retrieved through API

## 7. API Layer Tasks

### P1-09: Add User Management Endpoints
- Status: `TODO`
- Goal:
  - expose first `User` APIs
- Suggested endpoints:
  - `POST /api/users`
  - `GET /api/users`
  - `GET /api/users/{id}`
- Acceptance:
  - endpoints compile, run, and return expected shapes

### P1-10: Add Role Management Endpoints
- Status: `TODO`
- Goal:
  - expose first `Role` APIs
- Suggested endpoints:
  - `POST /api/roles`
  - `GET /api/roles`
  - `GET /api/roles/{id}`
- Acceptance:
  - endpoints compile, run, and return expected shapes

### P1-11: Standardize Validation/Error Responses for New Modules
- Status: `TODO`
- Goal:
  - keep all future endpoints aligned with the current error contract
- Work:
  - ensure validation failures use the standardized response shape
  - keep trace id / status / message structure consistent
- Acceptance:
  - new modules do not introduce inconsistent error formats

## 8. Security Tasks

### P2-01: Design Authentication Baseline
- Status: `TODO`
- Goal:
  - define how authentication should work before implementation
- Work:
  - choose JWT strategy
  - define token payload baseline
  - decide login identifier strategy
- Acceptance:
  - auth direction is documented and implementation-ready

### P2-02: Implement JWT Authentication
- Status: `TODO`
- Goal:
  - add real authentication flow
- Work:
  - JWT config
  - login endpoint
  - token generation
  - auth middleware wiring
- Acceptance:
  - protected endpoints can use bearer token auth

### P2-03: Implement Authorization Baseline
- Status: `TODO`
- Goal:
  - connect roles/permissions with API access
- Work:
  - define permission check strategy
  - add authorization policies or custom checks
- Acceptance:
  - API authorization can enforce permission-based access

## 9. Logging / Reliability Tasks

### P2-04: Add Audit Log Baseline
- Status: `TODO`
- Goal:
  - capture important system actions
- Work:
  - define audit entity
  - define key events to record
  - wire logging points in admin flows
- Acceptance:
  - important actions can be traced in database or persistent log storage

### P2-05: Expand Global Error Handling
- Status: `TODO`
- Goal:
  - improve operational quality
- Work:
  - map common exception types
  - add optional problem classification/error codes
- Acceptance:
  - errors are easier to diagnose and more consistent

## 10. Frontend Tasks

### P2-06: Scaffold Angular Application
- Status: `TODO`
- Goal:
  - create the first frontend application under `Frontend`
- Work:
  - initialize Angular app
  - add Tailwind
  - define base layout
  - define route structure
- Acceptance:
  - frontend app starts locally
  - project structure aligns with backend module direction

### P2-07: Build Admin Layout Foundation
- Status: `TODO`
- Goal:
  - create reusable shell for future management screens
- Work:
  - login placeholder layout
  - sidebar/topbar/app shell
  - responsive foundation
- Acceptance:
  - base admin layout is reusable across modules

### P2-08: Build First Management Screens
- Status: `TODO`
- Goal:
  - connect frontend to backend baseline
- Suggested screens:
  - user list
  - role list
  - permission list
  - menu list
- Acceptance:
  - frontend can consume first real backend APIs

## 11. Quality / Hardening Tasks

### P3-01: Add More Unit Tests
- Status: `TODO`
- Goal:
  - improve confidence in application logic
- Work:
  - add tests for user/role/permission handlers
- Acceptance:
  - meaningful coverage exists for core use cases

### P3-02: Add More Integration Tests
- Status: `TODO`
- Goal:
  - validate real API behavior across modules
- Work:
  - test CRUD endpoints
  - test validation failures
  - test auth-protected endpoints later
- Acceptance:
  - main API flows are covered by automated tests

### P3-03: Add Deployment/Environment Documentation
- Status: `TODO`
- Goal:
  - make setup reproducible for other machines
- Work:
  - document local backend run steps
  - document PostgreSQL setup
  - document migration/update steps
- Acceptance:
  - another machine can run the backend without guesswork

## 12. Recommended First Sprint
- Sprint 1 target:
  - `P0-01`
  - `P0-02`
  - `P0-03`
  - `P1-01`
  - `P1-02`
- Expected result:
  - real database connected
  - first two real modules exist
  - first `User` and `Role` entities are persisted

## 13. Update Rules For This Task File
- When a task is started:
  - change status to `IN PROGRESS`
- When a task is completed:
  - change status to `DONE`
  - add short note under the task if needed
- When scope changes:
  - add new task IDs instead of silently rewriting old intent
- Keep this file aligned with:
  - [AI_PROJECT_HANDOFF.md](E:\Project\NewStart\AI_PROJECT_HANDOFF.md)
  - [PROJECT_PLAN.md](E:\Project\NewStart\PROJECT_PLAN.md)

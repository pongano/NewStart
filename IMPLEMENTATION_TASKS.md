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
- Note:
  - completed with local PostgreSQL on `localhost:5432`
  - development database `coreproject_backend_dev` created
  - migration applied successfully
  - API verified through `/health` and `/api/system/info`
  - Docker fallback documented in [DEV_DATABASE_SETUP.md](E:\Project\NewStart\DEV_DATABASE_SETUP.md)

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
- Note:
  - added `Identity` structure across `Domain`, `Application`, `Infrastructure`, and `API`
  - added `UserAccount` skeleton entity, overview handler, controller, EF configuration, tests, and migration support

### P0-03: Create AccessControl Module Skeleton
- Status: `DONE`
- Goal:
  - create the first real module structure for `Role / Permission / Menu`
- Work:
  - define module folders in `Domain`, `Application`, `Infrastructure`, `API`
  - keep `Role`, `Permission`, and `Menu` in the same module area
  - add placeholder contracts/entities only as needed
- Acceptance:
  - solution contains a clear `AccessControl` module skeleton
  - module boundaries are ready for future CRUD and authorization logic
- Note:
  - added `AccessControl` structure across `Domain`, `Application`, `Infrastructure`, and `API`
  - added `Role`, `Permission`, and `Menu` skeleton entities, overview handler, controller, EF configurations, tests, and migration support

## 5. Core Backend Tasks

### P1-01: Implement User Entity and Persistence
- Status: `DONE`
- Goal:
  - enrich the `UserAccount` skeleton into the first real `User` domain model
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
  - user model supports real create/read flows
  - user persistence baseline is ready for admin use cases
- Note:
  - implemented real `UserAccount` create/list/get-by-id vertical slice
  - no new migration was required because the existing `user_accounts` schema already supported the v1 fields
  - validation now enforces required fields, max lengths, email format, and duplicate `UserName` / `Email`

### P1-02: Implement Role Entity and Persistence
- Status: `DONE`
- Goal:
  - enrich `Role` as part of `AccessControl`
- Work:
  - enrich role entity with shared max-length constants
  - extend persistence contract through `IApplicationDbContext`
  - implement role add/find/list/duplicate-code support in `ApplicationDbContext`
- Acceptance:
  - role model supports real admin workflows
  - role is ready for assignment to users later
- Note:
  - completed as persistence-only foundation without adding role CRUD endpoints yet
  - no new migration was required because the existing `roles` schema already matched the v1 shape
  - added application unit tests for role persistence contract behavior

### P1-03: Implement Permission Entity and Persistence
- Status: `DONE`
- Goal:
  - enrich permission structure for API / UI access control
- Work:
  - enrich permission entity with shared max-length constants
  - define persistence-ready permission code contract
  - extend `IApplicationDbContext` and `ApplicationDbContext` for permission operations
- Acceptance:
  - permission model supports real admin workflows
  - permission model is ready for role mapping
- Note:
  - completed as persistence-only foundation without adding permission query or CRUD endpoints yet
  - no new migration was required because the existing `permissions` schema already matched the v1 shape
  - added application unit tests for permission persistence contract behavior

### P1-04: Implement Menu Entity and Persistence
- Status: `DONE`
- Goal:
  - enrich menu structure for frontend navigation control
- Work:
  - enrich menu entity with shared max-length constants
  - define persistence-ready parent-child and ordering structure
  - extend `IApplicationDbContext` and `ApplicationDbContext` for menu operations
- Acceptance:
  - menu model supports real admin workflows
  - menu model is ready to be linked to permissions
- Note:
  - completed as persistence-only foundation without adding menu query or CRUD endpoints yet
  - no new menu-only schema change was required because the existing `menus` schema already matched the v1 shape
  - added application unit tests for menu persistence contract behavior

### P1-05: Implement User-Role / Role-Permission Relationships
- Status: `DONE`
- Goal:
  - establish the first useful access-control relationships
- Work:
  - define explicit join entities for `UserRole` and `RolePermission`
  - configure EF relationships and composite keys
  - create and apply migration for relationship tables
- Acceptance:
  - user-role and role-permission structures are persisted correctly
- Note:
  - added `user_roles` and `role_permissions` tables through migration `20260610100533_AddMenuAndAccessControlRelationships`
  - verified relationship tables were created in PostgreSQL and application tests passed

## 6. Application Layer Tasks

### P1-06: Create First User Management Use Cases
- Status: `DONE`
- Goal:
  - support first CRUD workflow for user management
- Work:
  - create `CreateUser`
  - create `GetUserById`
  - create `ListUsers`
  - optionally create `UpdateUserStatus`
- Acceptance:
  - at least one create and one read flow work end-to-end
- Note:
  - completed with `CreateUser`, `GetUserById`, and `ListUsers`

### P1-07: Create First Role Management Use Cases
- Status: `DONE`
- Goal:
  - support basic role CRUD
- Work:
  - create role create/list/get flows
- Acceptance:
  - role endpoints can be tested end-to-end
- Note:
  - completed with `CreateRole`, `GetRoleById`, and `ListRoles`
  - validation covers required fields, max lengths, and duplicate `Code`

### P1-08: Create First Permission / Menu Query Flows
- Status: `DONE`
- Goal:
  - expose permission and menu data for future admin UI
- Work:
  - create list queries
  - add DTOs
- Acceptance:
  - permission and menu data can be retrieved through API
- Note:
  - completed with `ListPermissions` and `ListMenus`
  - list responses use stable ordering from persistence/query handlers

## 7. API Layer Tasks

### P1-09: Add User Management Endpoints
- Status: `DONE`
- Goal:
  - expose first `User` APIs
- Suggested endpoints:
  - `POST /api/users`
  - `GET /api/users`
  - `GET /api/users/{id}`
- Acceptance:
  - endpoints compile, run, and return expected shapes
- Note:
  - implemented `POST /api/users`, `GET /api/users`, and `GET /api/users/{id}`
  - validated success, duplicate, invalid email, and not-found scenarios through integration tests

### P1-21: Add User Update/Delete Flows
- Status: `DONE`
- Goal:
  - complete the first admin management cycle for `UserAccount`
- Work:
  - add update use case
  - add delete use case
  - expose `PUT` and `DELETE` endpoints
- Acceptance:
  - user update/delete compile, run, and are covered by tests
- Note:
  - implemented `PUT /api/users/{id}` and `DELETE /api/users/{id}`
  - validated update success, delete success, duplicate-email validation, and standardized not-found responses through tests

### P1-10: Add Role Management Endpoints
- Status: `DONE`
- Goal:
  - expose first `Role` APIs
- Suggested endpoints:
  - `POST /api/roles`
  - `GET /api/roles`
  - `GET /api/roles/{id}`
- Acceptance:
  - endpoints compile, run, and return expected shapes
- Note:
  - implemented `POST /api/roles`, `GET /api/roles`, and `GET /api/roles/{id}`
  - validated success, duplicate, list ordering, and not-found scenarios through integration tests

### P1-12: Add Permission Management Endpoints
- Status: `DONE`
- Goal:
  - expose first `Permission` management APIs
- Suggested endpoints:
  - `POST /api/permissions`
  - `GET /api/permissions`
  - `GET /api/permissions/{id}`
- Acceptance:
  - endpoints compile, run, and return expected shapes
- Note:
  - implemented first permission create/list/get flows and endpoints
  - validated success, duplicate-code, list ordering, and not-found scenarios through integration tests

### P1-13: Add Menu Management Endpoints
- Status: `DONE`
- Goal:
  - expose first `Menu` management APIs
- Suggested endpoints:
  - `POST /api/menus`
  - `GET /api/menus`
  - `GET /api/menus/{id}`
- Acceptance:
  - endpoints compile, run, and return expected shapes
- Note:
  - implemented first menu create/list/get flows and endpoints
  - validated success, duplicate-code, list ordering, and not-found scenarios through integration tests

### P1-14: Implement Menu-Permission Relationships
- Status: `DONE`
- Goal:
  - connect navigation visibility with permission rules
- Work:
  - define `MenuPermission` join entity
  - configure EF relationship and migration
  - expose first assign/list API flows
- Acceptance:
  - menu-permission links are persisted correctly and can be queried through API
- Note:
  - added `menu_permissions` table through migration `20260610104156_AddMenuPermissionRelationships`
  - implemented assign/list flows under `/api/menus/{menuId}/permissions`

### P1-15: Add Role Update/Delete Flows
- Status: `DONE`
- Goal:
  - complete the first admin management cycle for `Role`
- Work:
  - add update use case
  - add delete use case
  - expose `PUT` and `DELETE` endpoints
- Acceptance:
  - role update/delete compile, run, and are covered by tests
- Note:
  - implemented `PUT /api/roles/{id}` and `DELETE /api/roles/{id}`
  - validated update success, delete success, and duplicate-code validation through tests

### P1-16: Add Permission Update/Delete Flows
- Status: `DONE`
- Goal:
  - complete the first admin management cycle for `Permission`
- Work:
  - add update use case
  - add delete use case
  - expose `PUT` and `DELETE` endpoints
- Acceptance:
  - permission update/delete compile, run, and are covered by tests
- Note:
  - implemented `PUT /api/permissions/{id}` and `DELETE /api/permissions/{id}`
  - validated update success, delete success, and duplicate-code validation through tests

### P1-17: Add Menu Update/Delete Flows
- Status: `DONE`
- Goal:
  - complete the first admin management cycle for `Menu`
- Work:
  - add update use case
  - add delete use case
  - expose `PUT` and `DELETE` endpoints
- Acceptance:
  - menu update/delete compile, run, and are covered by tests
- Note:
  - implemented `PUT /api/menus/{id}` and `DELETE /api/menus/{id}`
  - validated update success, delete success, and child-menu delete rejection through tests

### P1-18: Add User-Role Management Endpoints
- Status: `DONE`
- Goal:
  - expose first admin APIs for assigning and querying roles by user
- Suggested endpoints:
  - `POST /api/users/{userId}/roles/{roleId}`
  - `GET /api/users/{userId}/roles`
  - `DELETE /api/users/{userId}/roles/{roleId}`
- Acceptance:
  - user-role assignment flows compile, run, and are covered by tests
- Note:
  - implemented assign/list/remove user-role flows end-to-end
  - validated success, duplicate-link rejection, and remove success through unit and integration tests

### P1-19: Add Role-Permission Management Endpoints
- Status: `DONE`
- Goal:
  - expose first admin APIs for assigning and querying permissions by role
- Suggested endpoints:
  - `POST /api/roles/{roleId}/permissions/{permissionId}`
  - `GET /api/roles/{roleId}/permissions`
  - `DELETE /api/roles/{roleId}/permissions/{permissionId}`
- Acceptance:
  - role-permission assignment flows compile, run, and are covered by tests
- Note:
  - implemented assign/list/remove role-permission flows end-to-end
  - validated success, duplicate-link rejection, and remove success through unit and integration tests

### P1-20: Add User Access-Graph Query
- Status: `DONE`
- Goal:
  - expose an admin query that shows a user's effective roles, permissions, and menus
- Suggested endpoint:
  - `GET /api/users/{userId}/access-graph`
- Acceptance:
  - access graph returns effective role, permission, and menu data through API
- Note:
  - implemented `GetUserAccessGraph` query and endpoint
  - access graph derives permissions from assigned roles and menus from linked menu-permissions
  - validated end-to-end through unit and integration tests

### P1-11: Standardize Validation/Error Responses for New Modules
- Status: `DONE`
- Goal:
  - keep all future endpoints aligned with the current error contract
- Work:
  - ensure validation failures use the standardized response shape
  - keep trace id / status / message structure consistent
- Acceptance:
  - new modules do not introduce inconsistent error formats
- Note:
  - kept validation failures on the shared `ApiErrorResponse` contract through middleware
  - standardized controller-generated `404` responses on the same contract for users, roles, permissions, menus, and relationship deletes
  - added regression tests for standardized `400` and `404` payloads

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
  - first `User` slice is working end-to-end
  - `Role` persistence is ready for role use cases and later relationship mapping

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

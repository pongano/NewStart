# Today Summary - 2026-05-20

## 1. Main Outcome
- Defined the initial project direction for `Core Project`
- Agreed to start with a `modular monolith`
- Avoided splitting the system into many small deployable services too early

## 2. Project Direction
- Project type: Web First + Responsive
- Future expansion: Mobile or other platforms later
- Purpose: Core platform for foundational system management

## 3. Core Functional Scope
- User Management
- Role Management
- Permission Management
- Menu Management
- Log / Audit
- Error Handling

## 4. Technology Stack
- Database: SQL with code-first approach
- Backend: C# .NET latest LTS, REST API
- Frontend: Angular + Tailwind CSS

## 5. Architecture Decision
- Current architecture: Modular Monolith
- Current deployment style: Single backend deployable
- Reason:
  - faster to develop
  - simpler to deploy
  - easier to debug
  - lower infrastructure complexity
  - still possible to split into microservices later

## 6. Module Boundary Direction
- `Identity`
  - user
  - authentication
  - account-related functionality
- `AccessControl`
  - role
  - permission
  - menu
- `Logging/Audit`
  - system log
  - audit trail
- `Configuration`
  - system settings
- `Shared`
  - shared abstractions/utilities only
- `ErrorHandling`
  - cross-cutting concern, not a standalone business module

## 7. Key Decision About Services
- Do not split each entity into its own service
- Do not create separate deployables for `Role`, `Permission`, and `Menu`
- If service separation is needed in the future, split by domain capability instead of entity/table

## 8. Future Microservice Direction
- It is possible to start as monolith now and extract some modules later
- A future candidate for extraction could be `AccessControl` if it becomes a high-load or independently scaling area
- Extraction should happen only when there is a clear reason such as:
  - scaling need
  - separate release cycle
  - team ownership split
  - strong isolation requirement

## 9. Files Created Today
- [PROJECT_PLAN.md](E:\Project\NewStart\PROJECT_PLAN.md)
- [TODAY_SUMMARY_2026-05-20.md](E:\Project\NewStart\TODAY_SUMMARY_2026-05-20.md)

## 10. Suggested Next Step
- Define backend solution structure for modular monolith
- Define Angular module / feature structure
- Split detailed plans per module later

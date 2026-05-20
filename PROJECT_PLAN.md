# PROJECT PLAN

## 1. Project Overview
- Project Name: Core Project
- Project Type: Web First + Responsive Core Management Platform
- Purpose: ระบบกลางสำหรับจัดการฟังก์ชันพื้นฐานของระบบ เช่น User, Role, Permission, Menu, Log และ Error Handling
- Target Expansion: รองรับการต่อยอดไปยัง Mobile หรือช่องทางอื่นในอนาคต

## 2. Vision
- สร้าง Core Platform ที่เป็นมาตรฐานกลางสำหรับทุกระบบย่อยในอนาคต
- ลดการพัฒนา feature ซ้ำในส่วนงานพื้นฐาน
- ทำให้การจัดการสิทธิ์, ผู้ใช้งาน, เมนู, การติดตามเหตุการณ์ และการดูแลระบบเป็นโครงสร้างเดียวกัน
- รองรับการขยายระบบได้ง่ายทั้งในเชิง business module และ platform

## 3. Project Goals
- มีโครงสร้างพื้นฐานสำหรับ Authentication / Authorization ที่ชัดเจน
- มีระบบบริหารจัดการผู้ใช้งานและสิทธิ์แบบยืดหยุ่น
- มีระบบ Menu Management ที่ผูกกับสิทธิ์การใช้งานได้
- มีระบบ Log และ Error Handling ที่ช่วยให้ monitor และ troubleshoot ได้สะดวก
- มี API มาตรฐานสำหรับให้ Frontend และระบบอื่นเชื่อมต่อ
- มี UI ที่ responsive และพร้อมต่อยอดเป็น design foundation สำหรับระบบอื่น

## 4. Success Criteria
- ผู้ดูแลระบบสามารถจัดการ User, Role, Permission และ Menu ได้ครบในระบบเดียว
- สิทธิ์การเข้าถึงถูกควบคุมได้ในระดับหน้าจอและระดับ action
- สามารถตรวจสอบการใช้งานและปัญหาผ่าน log ได้
- โครงสร้าง Backend และ Frontend รองรับการเพิ่ม module ใหม่ได้โดยไม่กระทบแกนหลักมาก
- ระบบพร้อม deploy และต่อยอดใน environment จริง

## 5. Product Scope

### In Scope
- User Management
- Role Management
- Permission Management
- Menu Management
- Authentication / Authorization
- Audit Log / Activity Log
- Error Handling
- Basic System Configuration
- REST API for core services
- Responsive Web UI

### Future Scope
- Mobile Application / Hybrid App
- Notification System
- File / Media Management
- Workflow / Approval Engine
- Dashboard / Analytics
- Multi-language
- Multi-tenant
- Integration with external identity providers

## 6. Core Modules
- Authentication Module
- User Management Module
- Role Management Module
- Permission Management Module
- Menu Management Module
- Logging & Audit Module
- Error Handling Module
- Configuration Module
- Common Shared Module

## 7. High-Level Functional Idea
- ผู้ดูแลระบบสามารถสร้างและจัดการ user account
- ระบบรองรับการกำหนด role และ permission ตามหน้าที่การใช้งาน
- เมนูในระบบสามารถแสดงผลตามสิทธิ์ของผู้ใช้
- ระบบบันทึกกิจกรรมสำคัญเพื่อการตรวจสอบย้อนหลัง
- ระบบจัดการ error อย่างเป็นมาตรฐานทั้งฝั่ง API และ UI
- โครงสร้างถูกออกแบบให้เพิ่ม business module ใหม่ได้ง่ายในอนาคต

## 8. Technical Direction

### Database
- SQL Database
- Code-first style
- รองรับการ version schema ผ่าน migration

### Backend
- C# .NET using latest LTS
- Architecture: REST API
- แนวทางแยกชั้นอย่างชัดเจน เช่น API, Application, Domain, Infrastructure

### Frontend
- Angular
- Tailwind CSS
- Responsive-first UI
- รองรับการจัดการ route, guard, state และ reusable component structure

## 9. High-Level Architecture
- Frontend SPA ติดต่อผ่าน REST API
- Backend เป็นศูนย์กลางของ business rule และ security
- Database เป็นแหล่งเก็บข้อมูลหลัก
- Logging และ Error Handling เป็น cross-cutting concern ที่ใช้ร่วมกันทั้งระบบ
- Authorization ถูกควบคุมจาก role-permission mapping และเชื่อมกับเมนู/การเข้าถึง API

## 10. Security Direction
- Authentication ที่ปลอดภัยและขยายได้
- Role-based access control (RBAC)
- API authorization ตาม permission
- Secure password handling
- Audit trail สำหรับเหตุการณ์สำคัญ
- มาตรฐานการจัดการ validation และ exception

## 11. Non-Functional Requirements
- Responsive UI
- Scalability สำหรับเพิ่ม module ในอนาคต
- Maintainability ของ codebase
- Reusability ของ component และ service
- Observability ผ่าน log และ error monitoring
- Performance ที่เหมาะสมกับระบบจัดการภายในองค์กร
- Security baseline สำหรับ production

## 12. Development Principles
- Modular design
- Separation of concerns
- Shared standard สำหรับ coding, naming และ structure
- API contract ชัดเจนระหว่าง frontend และ backend
- รองรับการ test และ refactor ได้ง่าย
- วางโครงสร้างให้พร้อมสำหรับ CI/CD ในอนาคต

## 13. High-Level Delivery Plan

### Phase 1: Foundation Setup
- Project structure
- Base architecture
- Environment setup
- Shared configuration
- Base authentication flow

### Phase 2: Core Security & Access Control
- User management
- Role management
- Permission management
- Menu management
- Route / API protection

### Phase 3: System Reliability
- Logging
- Audit trail
- Error handling
- Validation standard

### Phase 4: UI Completion & Experience
- Admin pages
- Responsive improvements
- Shared UI components
- UX consistency

### Phase 5: Hardening & Release Readiness
- Testing
- Performance review
- Security review
- Deployment preparation
- Documentation

## 14. Key Deliverables
- Core solution structure
- Database schema and migrations
- REST API for core modules
- Frontend admin panel
- Role-permission-menu management flow
- Logging and error handling baseline
- Project documentation

## 15. Risks / Considerations
- สิทธิ์การใช้งานอาจซับซ้อนขึ้นเมื่อระบบขยาย
- โครงสร้าง menu และ permission ต้องออกแบบเผื่ออนาคต
- การออกแบบ log มากเกินไปอาจกระทบ performance
- ถ้าไม่มี standard กลางตั้งแต่ต้น อาจทำให้ module ใหม่ไม่สอดคล้องกัน
- การเตรียมขยายไป mobile ควรถูกคิดในระดับ API contract ตั้งแต่แรก

## 16. Assumptions
- ระบบนี้เป็น Core Platform สำหรับหลาย module ในอนาคต
- เริ่มต้นจาก Web Application เป็นหลัก
- Frontend และ Backend พัฒนาแยกกันแต่มี contract ร่วมกัน
- ใช้ SQL database และ code-first เป็นแนวทางหลัก
- ระบบต้องพร้อมรองรับการเติบโตเชิงโครงสร้างมากกว่าการทำเฉพาะ use case เดียว

## 17. Suggested Module Detail Template
- Module Objective
- Business Scope
- Key Features
- User Roles Involved
- Main Screens
- API Endpoints
- Database Entities
- Validation Rules
- Permissions
- Error Cases
- Logs / Audit Events
- Dependencies
- Open Questions

## 18. Next Documents To Split Out
- Vision Detail
- System Architecture Plan
- Backend Module Plan
- Frontend Module Plan
- Database Design Plan
- Security Plan
- Logging & Monitoring Plan
- Error Handling Standard
- Module Detail Documents per feature

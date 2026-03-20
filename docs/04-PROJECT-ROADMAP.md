# Pet Clinic - Project Roadmap

> Detailed roadmap with phases, milestones, timeline, resource allocation,
> risk assessment, and go-live criteria.

---

## EXECUTIVE TIMELINE

```
Week 1           Week 2           Week 3           Week 4           Week 5           Week 6
├─── Phase 0 ───┤─── Phase 1 ────┤── Phase 2&3 ───┤── Phase 4 ─────┤── Phase 5 ─────┤── Ph6&7 ──┤
│  Critical      │  Backend API   │  Frontend Fix  │  Component &   │  Page           │ QA + Ship │
│  Bug Fixes     │  Completion    │  Design Found. │  Layout Redes. │  Redesigns      │           │
│                │                │                │                │                 │           │
├────────────────┤────────────────┤────────────────┤────────────────┤─────────────────┤───────────┤
Day 1    3   5   Day 6   8   10  Day 11  13  15   Day 16  18  20  Day 21  23   25   Day 26  30
     ▲                ▲                ▲                 ▲                 ▲              ▲
     M1               M2               M3                M4                M5             M6
```

---

## PHASE 0: CRITICAL BUG FIXES
**Duration:** Days 1-3 (3 days)
**Effort:** 10 SP (~20 hours)
**Team:** 1 Full-Stack Developer

### Day 1: Core Infrastructure Fixes
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| BUG-001 | Add Tailwind directives to style.css | 1 | |
| BUG-002 | Persist auth token across page refreshes | 3 | ✓ Done |
| BUG-003 | Add /register route to router | 1 | ✓ Done |

### Day 2: Component Fixes
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| BUG-004 | Add v-bind="$attrs" to Input component | 1 | |
| BUG-005 | Fix Toast v-model:show binding | 2 | |
| BUG-006 | Add cancel emit to AppointmentList | 2 | |

### Day 3: Verification & Integration Testing
- [ ] Test all 6 fixes together
- [ ] Verify no regressions on existing pages
- [ ] Run existing test suite (`npm run test`)
- [ ] Smoke test all user flows

### Milestone M1: Application Functional
**Date:** End of Day 3
**Criteria:**
- [ ] All 6 critical bugs verified fixed
- [ ] Application works without errors in browser console
- [ ] Users can register, login, and stay logged in after refresh
- [ ] All form inputs accept native HTML attributes
- [ ] Toast notifications display correctly
- [ ] Cancel button works on appointments

**Deliverables:**
- Git tag: `v0.1.0-bugfix`
- Bug fix verification report

---

## PHASE 1: BACKEND API COMPLETION
**Duration:** Days 4-10 (7 days)
**Effort:** 26 SP (~52 hours)
**Team:** 1 Backend Developer

### Days 4-5: Core Entity Endpoints
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| API-023 | Pet CRUD (GET/POST/PUT/DELETE /owners/me/pets) | 5 | |
| API-024 | Appointment management (GET/:id, PUT, PATCH cancel) | 5 | |

### Days 6-7: Visit & Invoice Endpoints
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| API-025 | Visit retrieval + Invoice endpoints | 5 | |
| API-026 | Veterinarian listing + availability | 5 | |

### Days 8-9: Extended Features
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| API-027 | Health records endpoint | 3 | |
| API-028 | Inventory full CRUD | 3 | ◐ Partial (PUT + incoming/delivered implemented) |

### Day 10: API Integration Testing
- [ ] All endpoints tested via Swagger UI
- [ ] Authorization verified on all protected endpoints
- [ ] Error responses follow consistent format
- [ ] Database seeding includes test data for new entities

### Milestone M2: Complete API Coverage
**Date:** End of Day 10
**Criteria:**
- [ ] All 11 missing frontend service methods have working backend endpoints
- [ ] Swagger documentation covers all endpoints
- [ ] All endpoints return proper error codes (400, 401, 403, 404)
- [ ] Database seeding includes veterinarians, health records, invoices
- [ ] API integration tests passing

**Deliverables:**
- Git tag: `v0.2.0-api`
- Updated Swagger documentation
- API test results

---

## PHASE 2: FRONTEND BUG FIXES + API INTEGRATION
**Duration:** Days 11-14 (4 days)
**Effort:** 34 SP (~68 hours)
**Team:** 1 Frontend Developer

### Days 11-12: Fix Functional Bugs
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| BUG-007 | Fix pet form validation error keys | 1 | |
| BUG-008 | Connect BookingPage to real time slot API | 5 | |
| BUG-009 | Fix empty veterinarianId in booking | 3 | |
| BUG-010 | Load real pet data in VisitDetails | 2 | |
| BUG-011 | Include treatments in completeVisit API call | 2 | |

### Days 13-14: Remove Mock Data + Create Dedicated Pages
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| BUG-012 | Connect VisitHistory to real API | 5 | |
| BUG-013 | Connect HealthRecords to real API | 3 | |
| BUG-014 | Create dedicated MyPets + Appointments pages | 5 | |
| BUG-015 | Connect Invoices, Inventory, Patients to API | 8 | |

### Milestone M3: Full Data Integration
**Date:** End of Day 14
**Criteria:**
- [ ] Zero mock/hardcoded data remaining on any page
- [ ] All pages load data from the backend API
- [ ] All forms submit to backend and update UI
- [ ] All CRUD operations work end-to-end
- [ ] No console errors or warnings
- [ ] All 15 functional bugs verified fixed

**Deliverables:**
- Git tag: `v0.3.0-integrated`
- Integration test results

---

## PHASE 3: DESIGN SYSTEM FOUNDATION
**Duration:** Days 15-16 (2 days)
**Effort:** 3 SP (~6 hours) + setup time
**Team:** 1 Frontend Developer

### Day 15: Design Tokens & Tooling
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-029a | Update tokens.css with new color palette | 1 | |
| UI-029b | Update tailwind.config.js (colors, shadows, fix borderRadius bug) | 1 | |
| UI-029c | Install @heroicons/vue and verify imports work | 1 | |

### Day 16: Verification
- [ ] All pages reflect new colors
- [ ] Dark mode uses new slate neutrals
- [ ] Heroicons available and rendering
- [ ] No build errors
- [ ] Visual diff check (before/after screenshots)

### Milestone M3.5: Design Foundation Ready
**Date:** End of Day 16
**Criteria:**
- [ ] New color palette applied (cyan-500 primary, slate neutrals)
- [ ] Heroicons installed and importable
- [ ] Tailwind config updated with complete color scales
- [ ] CSS build produces valid output
- [ ] Dark mode working with new colors

**Deliverables:**
- Design token reference (exported colors)
- Before/after screenshots

---

## PHASE 4: COMPONENT & LAYOUT REDESIGN
**Duration:** Days 17-22 (6 days)
**Effort:** 13 SP (~26 hours)
**Team:** 1 Frontend Developer

### Days 17-18: Core Component Redesign
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-030a | Button component: new colors, sizes, icon support | 1 | |
| UI-030b | Card component: updated shadow, radius, dark mode | 1 | |
| UI-030c | Input component: updated focus ring, error state | 1 | |
| UI-030d | Modal component: new shadow, radius, a11y (UX-016) | 2 | |

### Days 19-20: Extended Components
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-030e | Toast component: updated variant colors, timer fix | 1 | |
| UI-030f | Badge component: updated color variants | 1 | |
| UI-030g | NEW: Avatar component | 1 | |
| UI-030h | NEW: Tabs component with keyboard nav | 2 | |
| UI-030i | NEW: DataTable component with sorting | 3 | |

### Days 21-22: Layout Redesign
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-031a | Replace emoji icons with Heroicons in sidebar | 2 | |
| UI-031b | Update header/topbar styling | 1 | |
| UI-031c | Create Breadcrumb component | 2 | |
| UX-017 | Create 404 NotFound page | 2 | |
| UX-018 | Create 403 Forbidden page | 1 | |
| UX-022 | Fix Button MouseEvent passthrough | 1 | |

### Milestone M4: Component Library Modernized
**Date:** End of Day 22
**Criteria:**
- [ ] All 9 UI components match design specifications
- [ ] Professional icons throughout (no emoji in navigation)
- [ ] Breadcrumb navigation on detail pages
- [ ] 404 and 403 pages created
- [ ] All component states working (hover, focus, active, disabled, error)
- [ ] Dark mode verified for all components
- [ ] Keyboard navigation works on all interactive components

**Deliverables:**
- Git tag: `v0.4.0-components`
- Component showcase screenshots

---

## PHASE 5: PAGE REDESIGNS
**Duration:** Days 23-28 (6 days)
**Effort:** 13 SP (~26 hours)
**Team:** 1 Frontend Developer

### Day 23: Authentication Pages
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-032a | LoginPage: split-screen layout, brand panel | 2 | |
| UI-032b | RegisterPage: matching design, form improvements | 1 | |

### Day 24: Owner Dashboard + Booking
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-032c | OwnerDashboard: compact profile, stat cards, pet grid | 3 | |
| UI-032d | BookingPage: visual progress, improved selections | 2 | |

### Day 25: Vet Pages
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-032e | VetDashboard: alert cards, schedule view | 2 | |
| UI-032f | VisitDetails: two-column layout, timeline | 2 | |
| UI-032g | PatientsPage: search/filter redesign | 1 | |

### Day 26: Owner Feature Pages
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| UI-032h | HealthRecords: pet tabs, vaccination timeline | 1 | |
| UI-032i | InvoicesPage: stat summary, filter bar, table | 1 | |
| UI-032j | VisitHistory: filter bar, visit cards | 1 | |
| UI-032k | InventoryPage: stock bars, categories | 1 | |

### Milestone M5: All Pages Redesigned
**Date:** End of Day 26
**Criteria:**
- [ ] All 14 pages match wireframe specifications
- [ ] All existing functionality preserved
- [ ] Responsive at 375px, 768px, 1024px, 1280px breakpoints
- [ ] Dark mode works on every page
- [ ] Loading states and empty states implemented consistently
- [ ] All user flows work end-to-end

**Deliverables:**
- Git tag: `v0.5.0-redesign`
- Full-page screenshots (light + dark, desktop + mobile)

---

## PHASE 6: QUALITY ASSURANCE
**Duration:** Days 27-29 (3 days)
**Effort:** 14 SP (~28 hours)
**Team:** 1 QA Engineer or Full-Stack Developer

### Day 27: Automated Testing
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| QA-033 | Accessibility audit (axe-core, keyboard, screen reader) | 5 | |
| QA-036 | Performance benchmarking (Lighthouse CI) | 3 | |

### Day 28: Manual Testing
| Task ID | Task | SP | Status |
|---------|------|----|--------|
| QA-034 | Cross-browser testing (Chrome, Firefox, Safari, Edge) | 3 | |
| QA-035 | Responsive testing (4 breakpoints, landscape) | 3 | |

### Day 29: Bug Fix Day
- [ ] Fix all critical issues found during QA
- [ ] Re-run automated tests
- [ ] Verify all fixes
- [ ] Regression testing

### Milestone M6: Quality Gates Passed
**Date:** End of Day 29
**Criteria:**
- [ ] Lighthouse scores: Performance >= 90, Accessibility >= 90, Best Practices >= 90
- [ ] Zero WCAG 2.1 AA violations
- [ ] No JavaScript errors on any page in any browser
- [ ] All breakpoints verified
- [ ] All user flows tested end-to-end
- [ ] Test suite passing: `npm run test` and `npm run e2e`

**Deliverables:**
- Lighthouse reports (PDF)
- Accessibility audit report
- Cross-browser test report
- Responsive test screenshots

---

## PHASE 7: POLISH & DEPLOYMENT
**Duration:** Day 30 (1 day)
**Effort:** ~8 hours
**Team:** Full team

### Day 30: Ship It
| Task | Time | Status |
|------|------|--------|
| Final UI polish and spacing adjustments | 2h | |
| Update all documentation | 2h | |
| Production build and verification | 1h | |
| Deploy to staging | 1h | |
| Staging smoke test | 1h | |
| Deploy to production | 1h | |

### Milestone M7: GO-LIVE
**Date:** Day 30
**Criteria:** See Go-Live Criteria section below

---

## RESOURCE ALLOCATION

### Solo Developer Scenario
```
Duration: 6 weeks (30 working days)
Effort:   ~268 hours total (134 SP @ 2hr/SP)

Week 1 (Days 1-5):    Phase 0 (bugs) + Phase 1 start (API)
Week 2 (Days 6-10):   Phase 1 complete (API)
Week 3 (Days 11-16):  Phase 2 (frontend bugs) + Phase 3 (design foundation)
Week 4 (Days 17-22):  Phase 4 (components + layout)
Week 5 (Days 23-28):  Phase 5 (pages) + Phase 6 start (QA)
Week 6 (Days 29-30):  Phase 6 complete (QA) + Phase 7 (deploy)
```

### Two-Developer Scenario (Recommended)
```
Duration: 4 weeks (20 working days)

Dev A (Frontend):           Dev B (Backend):
───────────────────         ───────────────────
Week 1:                     Week 1:
  Phase 0 (critical bugs)    Phase 1 (API endpoints)
  Phase 2 start               API testing

Week 2:                     Week 2:
  Phase 2 complete            Phase 1 complete
  Phase 3 (design tokens)    Support frontend integration

Week 3:                     Week 3:
  Phase 4 (components)       QA support
  Phase 4 (layout)           Documentation

Week 4:                     Week 4:
  Phase 5 (pages)            Phase 6 (QA testing)
  Phase 7 (deploy)           Phase 7 (deploy)
```

### Three-Developer Scenario (Fastest)
```
Duration: 3 weeks (15 working days)

Dev A (Frontend):     Dev B (Backend):      Dev C (QA/Design):
─────────────────     ─────────────────     ──────────────────
Week 1:               Week 1:               Week 1:
  Phase 0 (bugs)       Phase 1 (API)         Design system prep
  Phase 2 (fix)                              Component prototypes

Week 2:               Week 2:               Week 2:
  Phase 4 (components)  Support integration   Phase 5 (pages)
  Phase 4 (layout)                            Phase 5 (pages cont.)

Week 3:               Week 3:               Week 3:
  Phase 5 (pages)       Phase 6 (perf)        Phase 6 (a11y)
  Phase 7 (deploy)      Phase 7 (deploy)      Phase 6 (responsive)
```

---

## RISK REGISTER

| # | Risk | Probability | Impact | Mitigation |
|---|------|-------------|--------|------------|
| R1 | Backend API implementation takes longer due to complex business rules | Medium | High | Start API work early (Phase 1). Define clear DTOs upfront. Use TDD. |
| R2 | Tailwind class conflicts after color palette change | Low | Medium | Incremental changes with visual regression testing. Use design tokens. |
| R3 | Accessibility fixes reveal deeper component architecture issues | Medium | Medium | Budget extra time in Phase 4. Consider headless UI libraries (@headlessui/vue). |
| R4 | Dark mode regressions across pages | Medium | Low | Maintain light/dark screenshot pairs for each component. Automate with Playwright. |
| R5 | PostgreSQL schema changes break existing data | Low | High | Use EF Core migrations instead of EnsureCreated. Create backup before changes. |
| R6 | Performance degrades with new design (larger bundle) | Low | Medium | Lighthouse CI in pipeline. Tree-shake unused icons. Code-split aggressively. |
| R7 | Vet/Owner domain identity mismatch (Owner entity used for Vet auth) | High | High | Address in Phase 1: either align login to use Veterinarian entity or create unified User entity. |
| R8 | Scope creep from new feature requests during redesign | Medium | High | Strict adherence to checklist. New requests go to backlog, not current sprint. |
| R9 | Missing test coverage allows regressions | Medium | Medium | Write unit tests for all new components. Add e2e tests for critical flows. |
| R10 | Deployment issues (Docker .NET version mismatch) | High | Medium | Fix Dockerfile to use .NET 9.0 images. Test Docker build early. |

### Risk Response Matrix
```
                    Low Impact    Medium Impact    High Impact
High Probability    Monitor       Mitigate         Mitigate Immediately
Medium Probability  Accept        Mitigate         Mitigate
Low Probability     Accept        Monitor          Mitigate
```

### Top 3 Risks Requiring Immediate Action:
1. **R7 (Vet identity):** Owner/Vet domain mismatch could cause auth issues. Fix in Phase 1.
2. **R10 (Docker):** Fix Dockerfile to .NET 9.0 immediately to prevent deployment failures.
3. **R5 (Schema):** Implement EF Core migrations before any schema changes.

---

## GO-LIVE CRITERIA CHECKLIST

### Functional Requirements
- [ ] All user roles can authenticate (register, login, logout, refresh)
- [ ] Pet owners can manage pets (CRUD)
- [ ] Pet owners can book, view, and cancel appointments
- [ ] Pet owners can view visit history and health records
- [ ] Pet owners can view and pay invoices
- [ ] Veterinarians can view today's appointments
- [ ] Veterinarians can start and complete visits
- [ ] Veterinarians can manage prescriptions
- [ ] Veterinarians can manage inventory
- [ ] Veterinarians can view patient records

### Non-Functional Requirements
- [ ] Lighthouse Performance score >= 90
- [ ] Lighthouse Accessibility score >= 90
- [ ] First Contentful Paint < 1.5 seconds
- [ ] Largest Contentful Paint < 2.5 seconds
- [ ] No WCAG 2.1 AA violations
- [ ] Responsive at 375px, 768px, 1024px, 1280px
- [ ] Dark mode works on all pages
- [ ] Cross-browser compatible (Chrome, Firefox, Safari, Edge)

### Quality Requirements
- [ ] Zero known critical bugs (Priority 0)
- [ ] Zero known high-priority bugs (Priority 1)
- [ ] All automated tests passing
- [ ] No security vulnerabilities (OWASP top 10 check)
- [ ] API endpoints return proper error codes and messages
- [ ] No hardcoded/mock data in production build

### Deployment Requirements
- [ ] Production build succeeds (`npm run build`, `dotnet publish`)
- [ ] Docker build succeeds with correct .NET version
- [ ] Environment variables documented
- [ ] Database migration/creation plan documented
- [ ] Rollback procedure documented
- [ ] SSL/TLS configured
- [ ] CORS configured for production domain

### Documentation Requirements
- [ ] API documentation complete and accurate
- [ ] Troubleshooting guide covers common issues
- [ ] Developer quick-reference guide available
- [ ] Deployment runbook documented

---

## POST-LAUNCH SUPPORT PLAN

### Week 1 Post-Launch (Hypercare)
- **Monitoring:** Active monitoring of error logs, API response times, user sessions
- **Support Hours:** Extended (8am-8pm) for first 3 days
- **Bug Fix SLA:** Critical = 4 hours, High = 24 hours, Medium = 72 hours
- **Communication:** Daily standup with stakeholders
- **Rollback readiness:** Previous version tagged and deployable within 30 minutes

### Week 2-4 Post-Launch (Stabilization)
- **Monitoring:** Standard monitoring with alerts
- **Support Hours:** Normal business hours
- **Bug Fix SLA:** Critical = 8 hours, High = 48 hours, Medium = 1 week
- **Feedback Collection:** User surveys, analytics review
- **Performance check:** Weekly Lighthouse audits

### Month 2+ (Steady State)
- **Monitoring:** Automated alerts only
- **Support Hours:** Standard
- **Enhancements:** Process user feedback into backlog
- **Maintenance:** Monthly dependency updates, security patches

### Post-Launch Metrics to Track
| Metric | Target | Tool |
|--------|--------|------|
| Error rate | < 0.1% of requests | Application logs |
| API response time (p95) | < 500ms | APM tool |
| Page load time (FCP) | < 1.5s | Lighthouse CI |
| User session duration | Increase vs baseline | Analytics |
| Task completion rate | > 95% for core flows | Analytics |
| Support ticket volume | Decreasing week-over-week | Ticketing system |

---

## APPENDIX: SPRINT PLANNING (Agile View)

### Sprint 1 (Week 1): Foundation
**Goal:** Fix critical bugs, start backend APIs
**Velocity Target:** 36 SP
```
BUG-001 (1)  BUG-002 (3)  BUG-003 (1)  BUG-004 (1)  BUG-005 (2)  BUG-006 (2)
API-023 (5)  API-024 (5)  API-025 (5)  API-026 (5)  API-027 (3)  API-028 (3)
Total: 36 SP
```

### Sprint 2 (Week 2-3): Integration + Design
**Goal:** Connect frontend to backend, establish design system
**Velocity Target:** 37 SP
```
BUG-007 (1)  BUG-008 (5)  BUG-009 (3)  BUG-010 (2)  BUG-011 (2)
BUG-012 (5)  BUG-013 (3)  BUG-014 (5)  BUG-015 (8)
UI-029 (3)
Total: 37 SP
```

### Sprint 3 (Week 3-4): Component & Layout
**Goal:** Modernize all components and layout
**Velocity Target:** 34 SP
```
UI-030 (8)  UI-031 (5)
UX-016 (3)  UX-017 (2)  UX-018 (2)  UX-022 (1)
UI-032a-d (8)  UI-032e-g (5)
Total: 34 SP
```

### Sprint 4 (Week 5): Pages + QA
**Goal:** Complete page redesigns, pass quality gates
**Velocity Target:** 19 SP
```
UI-032h-k (4)
UX-019 (5)  UX-020 (5)  UX-021 (3)
QA-033 (partial)  QA-034 (partial)
Total: 19 SP (with QA buffer)
```

### Sprint 5 (Week 6): QA + Ship
**Goal:** Quality assurance, polish, deploy
**Velocity Target:** 14 SP
```
QA-033 (5)  QA-034 (3)  QA-035 (3)  QA-036 (3)
Phase 7 deployment
Total: 14 SP
```

---

## DECISION LOG

| Date | Decision | Rationale | Impact |
|------|----------|-----------|--------|
| Mar 2026 | Use Heroicons instead of emoji | Professional appearance, accessibility, consistency | Medium - all nav items updated |
| Mar 2026 | Cyan-500 as primary color | Healthcare aesthetic, distinguishes from generic blue | Low - CSS variable swap |
| Mar 2026 | Fix bugs before redesign | Stable foundation needed before visual changes | High - delays UI work by 2 weeks |
| Mar 2026 | Create dedicated pages for /owner/pets etc. | Better UX, proper route handling | Medium - 2 new pages |
| Mar 2026 | Keep Vue 3 + Tailwind (no framework switch) | Minimize risk, leverage existing knowledge | Low - status quo maintained |
| Mar 2026 | Priority-based implementation order | Critical bugs first, polish last | High - ensures usable product at each milestone |

---

*Document Version: 1.0 | Last Updated: March 2026*
*Total Project Effort: 134 Story Points (~268 developer hours)*
*Timeline: 6 weeks (solo) / 4 weeks (duo) / 3 weeks (trio)*

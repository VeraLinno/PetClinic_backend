# Pet Clinic - Project Roadmap

## Executive Summary

**Project:** Pet Clinic UI Redesign
**Total Effort:** ~86 hours (~141 tasks across 7 phases)
**Timeline:** 2.5 weeks full-time / 5 weeks part-time
**Team:** 1 frontend developer
**Start Date:** TBD (ready to begin immediately)

### Current Progress Snapshot (March 2026)
- Auth persistence across refresh is implemented.
- Register route is active.
- My Pets and Vet Today Appointments pages are implemented.
- Inventory incoming/delivered reorder API flows are integrated in vet UI.
- This roadmap remains the target plan; some phases are now partially completed in code.

---

## Phase Timeline

```
Week 1                          Week 2                          Week 3
┌─────────────────────────────┬─────────────────────────────┬──────────────┐
│ Phase 0  │ Phase 1 │ Phase 2     │ Phase 3 │   Phase 4     │ Ph5  │ Ph6  │
│ Pre-Impl │ Found.  │ Components  │ Layout  │   Pages       │ Test │Polish│
│ (2h)     │ (6h)    │ (15h)       │ (9h)    │   (33h)       │(15h) │ (8h) │
│ Day 0    │ Day 1-2 │ Day 3-5     │ Day 6-7 │   Day 8-12    │D13-15│ D16  │
└──────────┴─────────┴─────────────┴─────────┴───────────────┴──────┴──────┘
```

---

## Detailed Phase Breakdown

### Phase 0: Pre-Implementation Setup
**Duration:** Day 0 (2 hours)
**Owner:** Developer
**Dependencies:** None
**Entry Criteria:** Access to repository, development environment ready

| Milestone | Description | Deliverable |
|-----------|-------------|-------------|
| M0.1 | Feature branch created | `feature/ui-redesign` branch |
| M0.2 | Dependencies installed | @heroicons/vue in node_modules |
| M0.3 | Dev server running | App loads at localhost:5173 |

**Exit Criteria:**
- `npm run dev` starts without errors
- Heroicons import compiles successfully
- Developer has read UI-REDESIGN-SPECIFICATION.md

---

### Phase 1: Foundation
**Duration:** Days 1-2 (6 hours)
**Owner:** Developer
**Dependencies:** Phase 0 complete
**Entry Criteria:** Dependencies installed, dev server running

| Milestone | Description | Deliverable | Hours |
|-----------|-------------|-------------|-------|
| M1.1 | Color palette updated | tailwind.config.js + tokens.css with cyan primary | 2h |
| M1.2 | Tailwind directives fixed | @tailwind directives in style.css | 0.5h |
| M1.3 | Typography system defined | Font loading + CSS variables | 1.5h |
| M1.4 | Global styles updated | Shadows, transitions, utilities | 1h |
| M1.5 | Visual verification | All pages render with new colors | 1h |

**Exit Criteria:**
- Primary color is cyan (#0ea5e9) across all pages
- Tailwind utilities work correctly (`bg-cyan-500` renders)
- Dark mode toggle still functions
- No console errors or warnings
- All existing pages still render (no visual regressions beyond color updates)

**Key Risk:** Breaking existing color references
**Mitigation:** Test each page after color updates, keep both old and new color values temporarily

---

### Phase 2: Component Redesign
**Duration:** Days 3-5 (15 hours)
**Owner:** Developer
**Dependencies:** Phase 1 complete (new color palette available)
**Entry Criteria:** Color system and typography in place

| Milestone | Description | Deliverable | Hours |
|-----------|-------------|-------------|-------|
| M2.1 | Button component redesigned | All variants (primary/secondary/danger/ghost) | 2h |
| M2.2 | Input component fixed + redesigned | Forwards attributes, cyan focus ring, error states | 1.5h |
| M2.3 | Card component redesigned | New borders, shadows, header/footer | 1h |
| M2.4 | Modal component redesigned + accessible | ARIA attributes, focus trap, Escape key | 2h |
| M2.5 | Toast component fixed + redesigned | v-model fix, timer cleanup, color variants | 1.5h |
| M2.6 | Badge component redesigned | All semantic color variants, dot indicator | 1h |
| M2.7 | Avatar component created (NEW) | Image/initials/status support, 4 sizes | 1.5h |
| M2.8 | Tabs component created (NEW) | Tab switching, keyboard nav, active indicator | 2h |
| M2.9 | Table component created (NEW) | Responsive wrapper, sorting, empty state | 2.5h |

**Exit Criteria:**
- All 10 components render correctly in light and dark mode
- Button disabled state works, Toast auto-dismisses, Modal traps focus
- Input forwards HTML attributes (min, max, required etc.)
- New components (Avatar, Tabs, Table) render with sample data
- Tab through all components — focus ring visible on every interactive element

**Key Risk:** Component API changes breaking existing page usage
**Mitigation:** Maintain backward-compatible props, test imports in each page after changes

---

### Phase 3: Layout Refactoring
**Duration:** Days 6-7 (9 hours)
**Owner:** Developer
**Dependencies:** Phase 2 complete (updated components available)
**Entry Criteria:** All 10 components redesigned

| Milestone | Description | Deliverable | Hours |
|-----------|-------------|-------------|-------|
| M3.1 | Sidebar icons replaced | All Heroicons, no emoji | 1h |
| M3.2 | Sidebar styling updated | Dark bg, white text, active indicators | 1.5h |
| M3.3 | Header/topbar updated | Search, notifications, dark toggle, user menu | 2h |
| M3.4 | Mobile sidebar working | Hamburger toggle, overlay backdrop | 1.5h |
| M3.5 | Breadcrumb component created | Path navigation for detail pages | 1h |
| M3.6 | Breadcrumbs added to pages | 7+ pages have breadcrumb navigation | 1h |
| M3.7 | Responsive verification | All breakpoints tested | 1h |

**Exit Criteria:**
- No emoji icons anywhere in the sidebar
- Active menu item clearly highlighted (cyan accent)
- Mobile: hamburger opens sidebar, overlay closes it
- Tablet: sidebar collapses, content uses full width
- Desktop: sidebar fixed at 240px, content offset
- Breadcrumbs render on detail pages with working links
- User profile section at sidebar bottom with Avatar component

**Key Risk:** Mobile sidebar animation janky / layout breaks at breakpoints
**Mitigation:** Test at 375px, 768px, 1024px after each change; use CSS transitions

---

### Phase 4: Page Redesigns
**Duration:** Days 8-12 (33 hours)
**Owner:** Developer
**Dependencies:** Phase 3 complete (layout and components ready)
**Entry Criteria:** Layout modernized, all components available

| Milestone | Description | Deliverable | Hours |
|-----------|-------------|-------------|-------|
| M4.1 | Auth pages redesigned | Login + Register with two-column layout | 3h |
| M4.2 | Owner Dashboard redesigned | Profile, stats, pets, appointments | 4h |
| M4.3 | Booking Wizard redesigned | Progress bar, cards, calendar, confirmation | 4h |
| M4.4 | Visit pages redesigned | VisitDetails + VisitHistory | 5h |
| M4.5 | Health & Invoices redesigned | HealthRecords + InvoicesPage | 6h |
| M4.6 | Vet pages redesigned | VetDashboard + Inventory + Patients | 6h |
| M4.7 | Critical bugs fixed | Auth persistence, missing routes, data fixes | 3h |
| M4.8 | Shared fixes applied | AppointmentList emit, router cleanup | 2h |

**Exit Criteria:**
- All 11 pages render with new design system
- Authentication token persists across page refresh
- Register page is accessible via route and login page link
- Hardcoded/mock data either connected to real API or clearly labeled as demo
- All forms validate correctly with proper error display
- No console errors or warnings on any page
- All pages work in light and dark mode

**Key Risk:** Mock data has no real API endpoints (VisitHistory, Inventory, Patients, Invoices)
**Mitigation:** Either implement missing endpoints or add clear "Demo Data" labels; document in Missing Endpoints section

**Key Risk:** Breaking existing workflows during redesign
**Mitigation:** Test each page's full user flow after redesign (e.g., login → dashboard → book appointment → view visit)

---

### Phase 5: Testing & QA
**Duration:** Days 13-15 (15 hours)
**Owner:** Developer
**Dependencies:** Phase 4 complete (all pages redesigned)
**Entry Criteria:** All pages redesigned and rendering

| Milestone | Description | Deliverable | Hours |
|-----------|-------------|-------------|-------|
| M5.1 | Accessibility audit passed | Lighthouse ≥ 90, WCAG 2.1 AA checklist | 4h |
| M5.2 | Responsive testing passed | All pages at 4 breakpoints | 4h |
| M5.3 | Dark mode audit passed | All pages visually correct in dark mode | 2h |
| M5.4 | Cross-browser testing | Chrome, Firefox, Safari, Edge | 3h |
| M5.5 | Performance audit passed | Lighthouse performance ≥ 90, bundle size checked | 2h |

**Exit Criteria:**
- Lighthouse accessibility ≥ 90 on all pages
- Lighthouse performance ≥ 90
- All pages render at 375px, 768px, 1024px, 1280px without issues
- Dark mode has no bright/white elements that should be dark
- All critical user flows work in Chrome, Firefox, Edge (Safari if available)
- No console errors or warnings in any browser
- All found issues documented with severity

**Key Risk:** Accessibility failures that require component rework
**Mitigation:** Test incrementally during Phase 2-4 (not just at end); use Wave extension during development

---

### Phase 6: Polish & Deploy
**Duration:** Day 16 (8 hours)
**Owner:** Developer
**Dependencies:** Phase 5 complete (all testing passed)
**Entry Criteria:** No P0 bugs remaining

| Milestone | Description | Deliverable | Hours |
|-----------|-------------|-------------|-------|
| M6.1 | P0/P1 bugs fixed | Zero critical or high bugs | 3h |
| M6.2 | Cosmetic polish | Spacing alignment, subtle animations | 1.5h |
| M6.3 | Production build verified | `npm run build` succeeds, preview works | 1h |
| M6.4 | Documentation updated | README, changelog | 1h |
| M6.5 | PR created and merged | Feature branch → main | 1.5h |

**Exit Criteria (Go-Live Checklist):**
- [ ] Zero P0 bugs
- [ ] Zero P1 bugs (or documented with timeline)
- [ ] `npm run build` succeeds with no errors
- [ ] `npm run preview` loads and all features work
- [ ] Lighthouse scores ≥ 90 for accessibility and performance
- [ ] All user flows tested end-to-end (login, booking, visit, etc.)
- [ ] Dark mode works on all pages
- [ ] Mobile layout works at 375px
- [ ] README updated with new dependencies and commands
- [ ] PR reviewed and approved
- [ ] No regressions in backend API integration

---

## Risk Assessment

### High Risk

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Missing backend endpoints (10 identified) | Pages show mock data in production | High | Document all missing endpoints; add "Demo" labels; create backend tickets |
| Token persistence bug causes user frustration | Users lose session on refresh | High | Fix in Phase 4 (task 4.40), priority P0 |
| Component API changes break existing pages | Pages crash or render incorrectly | Medium | Maintain backward-compatible props; test all import sites |
| Tailwind directives missing causes all styles to break | Entire app looks broken | High | Fix immediately in Phase 1 (task 1.7) |

### Medium Risk

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Scope creep from 11 page redesigns | Timeline exceeds 2.5 weeks | Medium | Strict adherence to phase plan; defer P3 items |
| Heroicons not having exact emoji replacements | Some icons look generic | Low | Use closest match; custom SVG for pet paw icon |
| Dark mode inconsistencies | Some elements hard to read | Medium | Test dark mode after each component/page change |
| Form validation library (VeeValidate) integration complexity | Delays Phase 4 | Low | Make VeeValidate optional; can use basic validation first |

### Low Risk

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Inter/Outfit font loading slow | FOUT (flash of unstyled text) | Low | Use `font-display: swap`; system fonts as fallback |
| Large bundle size from Heroicons | Slower initial load | Low | Tree-shaking removes unused icons; verify build size |
| CSS changes conflict with existing style.css | Unexpected visual changes | Low | Incremental updates; test after each change |

---

## Resource Allocation

### Single Developer Track
```
Week 1: Foundation + Components + Layout (30 hours)
Week 2: Pages + Bug Fixes (33 hours)
Week 3: Testing + Polish + Deploy (23 hours)
```

### Two Developer Track (Parallel)
```
Dev A: Components (15h) → Vet Pages (12h) → Accessibility Testing (4h)
Dev B: Foundation (6h) + Layout (9h) → Owner Pages (21h) → Responsive Testing (4h)
Both:  Integration Testing (4h) → Polish (4h) → Deploy (4h)
Timeline: ~1.5 weeks
```

---

## Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Lighthouse Accessibility | ≥ 90 | Lighthouse audit on all pages |
| Lighthouse Performance | ≥ 90 | Lighthouse audit on all pages |
| Color Contrast | ≥ 4.5:1 | Wave / axe DevTools |
| Mobile Touch Targets | ≥ 48px | Manual measurement |
| Time to First Contentful Paint | < 1.5s | Lighthouse |
| No Console Errors | 0 errors | Browser DevTools |
| Bundle Size (JS) | < 300KB gzipped | Build output |
| Bundle Size (CSS) | < 50KB gzipped | Build output |
| Test Coverage | All user flows | Manual testing checklist |
| Zero P0 Bugs | 0 | Bug registry |

---

## Post-Launch Plan

### Week 1 Post-Launch
- Monitor for user-reported issues
- Hot-fix any P0 bugs immediately
- Collect user feedback on new design

### Week 2-4 Post-Launch
- Address P2/P3 bugs from testing phase
- Implement deferred enhancements
- Consider implementing missing backend endpoints

### Ongoing
- Monitor Lighthouse scores monthly
- Update dependencies quarterly
- Review accessibility compliance semi-annually

---

## Decision Log

| Date | Decision | Rationale | Status |
|------|----------|-----------|--------|
| Mar 2026 | Use Cyan (#0ea5e9) as primary color | Healthcare aesthetic, modern, professional | Approved |
| Mar 2026 | Use Heroicons Vue for icons | Tree-shakeable, Vue-native, comprehensive | Approved |
| Mar 2026 | Maintain Tailwind CSS (not upgrade to v4) | Stability during redesign, v3 well-supported | Approved |
| Mar 2026 | VeeValidate + Zod optional | Can be added later without blocking redesign | Approved |
| Mar 2026 | Create Avatar, Tabs, Table components | Essential for redesigned pages | Approved |
| Mar 2026 | Keep all mock data pages (label as demo) | Missing backend endpoints can't be built in scope | Pending |

---

**Last Updated:** March 2026
**Status:** Ready for Phase 0 execution

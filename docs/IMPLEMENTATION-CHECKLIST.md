# Pet Clinic UI Redesign - Implementation Checklist

## Pre-Implementation Setup

- [ ] Read UI-REDESIGN-SPECIFICATION.md completely
- [ ] Read IMPLEMENTATION-ROADMAP.md for understanding
- [ ] Review current application (velinn-petclinic_js)
- [ ] Create git branch: `git checkout -b feature/ui-redesign`
- [ ] Install @heroicons/vue: `npm install @heroicons/vue@latest`
- [ ] Back up current work
- [ ] Set up IDE with Tailwind/Vue extensions
- [ ] Start dev server: `npm run dev`

### Progress Update (March 2026)

- [x] Register route is active at `/register`
- [x] Auth persistence across refresh is implemented (token storage + init bootstrap)
- [x] `MyPetsPage.vue` created and routed at `/owner/pets`
- [x] `VetTodayAppointments.vue` created and routed at `/vet/appointments`
- [x] Inventory incoming/delivered reorder data is API-integrated in vet pages

---

## Phase 1: Foundation Setup (Days 1-2)

### Step 1.1: Update Design System - Design Tokens
- [ ] Open `src/styles/tokens.css`
- [ ] Replace all color values with new palette
  - [ ] Primary: #2563eb → #0ea5e9 (cyan-500)
  - [ ] Success: #16a34a → #22c55e (green-500)
  - [ ] Warning: #f59e0b (update orange tones)
  - [ ] Danger: #ef4444 → #dc2626 (red-600)
- [ ] Add new color scales (50, 100, 200, ... 950)
- [ ] Update neutral grays to Slate
- [ ] Test in browser (colors should update everywhere)
- [ ] Verify dark mode colors

### Step 1.2: Update Tailwind Configuration
- [ ] Open `tailwind.config.js`
- [ ] Extend colors with new palette
- [ ] Remove old blue color customizations
- [ ] Add cyan color scale
- [ ] Add green, orange, red, teal color scales
- [ ] Verify dark mode configuration
- [ ] Update any custom shadows if needed
- [ ] Clear node_modules cache if colors don't update
- [ ] Run `npm run dev` and test

### Step 1.3: Update Global Styles
- [ ] Open `src/style.css`
- [ ] Review skeleton/shimmer animations
- [ ] Update status badge colors section:
  - [ ] status-scheduled: Update to cyan
  - [ ] status-completed: Verify green
  - [ ] status-cancelled: Verify red
  - [ ] status-pending: Verify orange
  - [ ] status-in-progress: Keep purple or update
- [ ] Update any manual color references
- [ ] Test all status classes
- [ ] Verify dark mode variants for status classes

### Step 1.4: Verify Foundation Changes
- [ ] Colors display correctly in browser
- [ ] Dark mode colors correct
- [ ] No console errors
- [ ] All pages reflect new colors
- [ ] Commit: `git commit -m "feat: update design tokens and color palette"`

**Phase 1 Status:** 🚧 In progress (core auth/router fixes completed)

---

## Phase 2: Component Redesign (Days 3-5)

### Step 2.1: Button Component
- [ ] Open `src/components/ui/Button.vue`
- [ ] Update color classes for all variants
  - [ ] Primary: Use new cyan colors
  - [ ] Secondary: Update outline colors
  - [ ] Danger: Use new red colors
  - [ ] Success: Use new green colors
- [ ] Update padding and sizing (40px height for md)
- [ ] Update hover/focus/active states
- [ ] Update disabled state styling
- [ ] Update dark mode variants
- [ ] Test all variants
- [ ] Test with icons
- [ ] Test loading state
- [ ] Commit changes

### Step 2.2: Input Component
- [ ] Open `src/components/ui/Input.vue`
- [ ] Update border color (slate-300)
- [ ] Update focus ring (cyan-500)
- [ ] Update error state:
  - [ ] Border: red-500
  - [ ] Background: red-50
- [ ] Update label styling
- [ ] Update help text styling
- [ ] Update placeholder styling
- [ ] Update dark mode colors
- [ ] Test focus state
- [ ] Test error state
- [ ] Test disabled state
- [ ] Commit changes

### Step 2.3: Card Component
- [ ] Open `src/components/ui/Card.vue`
- [ ] Update border color to slate-200
- [ ] Update shadow level (shadow-md)
- [ ] Update dark mode border (slate-700)
- [ ] Update dark mode background (slate-800)
- [ ] Update header/footer styling
- [ ] Test hover state (if interactive)
- [ ] Verify dark mode
- [ ] Commit changes

### Step 2.4: Modal Component
- [ ] Open `src/components/ui/Modal.vue`
- [ ] Update styling with new shadow (level 4)
- [ ] Update border radius (12px)
- [ ] Update header styling
- [ ] Update close button styling
- [ ] Add role="dialog" if missing
- [ ] Add aria-modal="true" if missing
- [ ] Update dark mode colors
- [ ] Update overlay background
- [ ] Test modal opens/closes
- [ ] Test close button
- [ ] Test backdrop click closes
- [ ] Commit changes

### Step 2.5: Toast Component
- [ ] Open `src/components/ui/Toast.vue`
- [ ] Update success colors (green)
- [ ] Update error colors (red)
- [ ] Update warning colors (orange)
- [ ] Update info colors (blue/cyan)
- [ ] Update left border (4px)
- [ ] Update background colors for each variant
- [ ] Update icon colors
- [ ] Update dark mode variants
- [ ] Test all toast types
- [ ] Test auto-dismiss
- [ ] Test close button
- [ ] Commit changes

### Step 2.6: Badge Component
- [ ] Open `src/components/ui/Badge.vue`
- [ ] Update all color variants
  - [ ] Primary: cyan
  - [ ] Success: green
  - [ ] Warning: orange
  - [ ] Danger: red
  - [ ] Info: blue
- [ ] Update padding for sizes
- [ ] Update dark mode variants
- [ ] Update outline variant styling
- [ ] Test all variants
- [ ] Test all sizes
- [ ] Test dot indicator
- [ ] Test dark mode
- [ ] Commit changes

### Step 2.7: Create Avatar Component (NEW)
- [ ] Create `src/components/ui/Avatar.vue`
- [ ] Implement image support (src prop)
- [ ] Implement initials fallback
- [ ] Implement size variants (sm: 32px, md: 48px, lg: 64px)
- [ ] Implement status indicator (online, offline, away, busy)
- [ ] Style with circular border-radius (9999px)
- [ ] Add proper ARIA labels
- [ ] Test all variants
- [ ] Test with images
- [ ] Test with initials
- [ ] Test status indicators
- [ ] Test dark mode
- [ ] Commit addition

### Step 2.8: Create Tabs Component (NEW)
- [ ] Create `src/components/ui/Tabs.vue`
- [ ] Implement tab navigation bar
- [ ] Implement content panels
- [ ] Style active tab indicator
- [ ] Add hover states
- [ ] Add keyboard navigation (arrow keys)
- [ ] Update colors to match new design
- [ ] Add animation on tab switch
- [ ] Test tab switching
- [ ] Test keyboard navigation
- [ ] Test dark mode
- [ ] Commit addition

### Step 2.9: Create Table Component (NEW)
- [ ] Create `src/components/ui/Table.vue`
- [ ] Implement table wrapper with scroll
- [ ] Implement header row styling
- [ ] Implement body row styling
- [ ] Implement alternating row colors
- [ ] Implement hover state on rows
- [ ] Add sortable column headers
- [ ] Add pagination controls (if needed)
- [ ] Update colors to match design
- [ ] Test column sorting
- [ ] Test row hover
- [ ] Test dark mode
- [ ] Test responsive scroll on mobile
- [ ] Commit addition

### Phase 2 Verification
- [ ] All components render correctly
- [ ] All interactive states work
- [ ] Dark mode verified for all components
- [ ] No console errors
- [ ] Colors match specification
- [ ] Spacing looks correct
- [ ] Commit: `git commit -m "feat: redesign all UI components"`

**Phase 2 Status:** ⏳ Ready to start

---

## Phase 3: Layout Refactoring (Days 6-7)

### Step 3.1: Update MainLayout Sidebar
- [ ] Open `src/layouts/MainLayout.vue`
- [ ] Replace emoji icons with Heroicons
  - [ ] 🏠 → HomeIcon
  - [ ] 📅 → PlusIcon + CalendarIcon
  - [ ] 🐾 → PawIcon
  - [ ] 📋 → ListBulletIcon
  - [ ] 📜 → DocumentTextIcon
  - [ ] 💳 → DocumentDollarIcon
  - [ ] 🏥 → HeartIcon
  - [ ] 💊 → ShoppingBagIcon
  - [ ] 👥 → UsersIcon
  - [ ] 🚪 → ArrowRightStartOnRectangleIcon
- [ ] Update sidebar colors to new palette
- [ ] Update active state indicator (cyan)
- [ ] Update hover state styling
- [ ] Update user profile section styling
- [ ] Update mobile sidebar behavior
- [ ] Update hamburger button styling
- [ ] Test sidebar open/close
- [ ] Test navigation
- [ ] Test active states
- [ ] Test dark mode
- [ ] Commit changes

### Step 3.2: Update Header/Topbar
- [ ] Open MainLayout header section
- [ ] Update header background colors
- [ ] Update search bar styling
- [ ] Update dark mode toggle colors
- [ ] Update notification bell styling
- [ ] Update user menu styling
- [ ] Update button hover states
- [ ] Verify search functionality
- [ ] Test dark mode
- [ ] Test responsive behavior
- [ ] Commit changes

### Step 3.3: Create Breadcrumb Component (NEW)
- [ ] Create `src/components/Breadcrumb.vue`
- [ ] Implement breadcrumb path display
- [ ] Add HomeIcon at start
- [ ] Style separators
- [ ] Add click navigation
- [ ] Responsive: collapse to home + current on mobile
- [ ] Update colors to new palette
- [ ] Add to pages that need it:
  - [ ] VisitDetails
  - [ ] HealthRecords
  - [ ] InvoicesPage
  - [ ] InventoryPage
- [ ] Test navigation
- [ ] Test dark mode
- [ ] Commit addition

### Step 3.4: Verify Layout Changes
- [ ] Sidebar displays correctly
- [ ] Header displays correctly
- [ ] Navigation works on all pages
- [ ] Mobile hamburger menu works
- [ ] Breadcrumbs appear where needed
- [ ] Icons display correctly
- [ ] Colors match specification
- [ ] No console errors
- [ ] Commit: `git commit -m "feat: redesign layout and navigation"`

**Phase 3 Status:** ⏳ Ready to start

---

## Phase 4: Page Redesigns (Days 8-12)

### Step 4.1: Authentication Pages (LoginPage, RegisterPage)
- [ ] LoginPage.vue
  - [ ] Update background colors
  - [ ] Add two-column layout (if feasible)
  - [ ] Use new Button component
  - [ ] Use new Input component
  - [ ] Update error message styling
  - [ ] Add brand/graphics
  - [ ] Test login flow
  - [ ] Test error states
  - [ ] Test dark mode
  - [ ] Test responsive

- [ ] RegisterPage.vue
  - [ ] Update form styling
  - [ ] Use new components
  - [ ] Update button styling
  - [ ] Test registration flow
  - [ ] Test validation
  - [ ] Test dark mode

- [ ] Commit: `git commit -m "feat: redesign authentication pages"`

### Step 4.2: Owner Dashboard
- [ ] Open `src/pages/OwnerDashboard.vue`
- [ ] Update profile section styling (more compact)
- [ ] Update stat cards:
  - [ ] New colors and styling
  - [ ] Update icons (Heroicons)
  - [ ] Improve visual hierarchy
- [ ] Update pet cards section:
  - [ ] Redesign pet cards
  - [ ] Add colored badges for species
  - [ ] Update action buttons
  - [ ] Test add pet modal
- [ ] Update appointment list:
  - [ ] New styling
  - [ ] Update status badges
  - [ ] Update action buttons
- [ ] Test dark mode
- [ ] Test responsive layout
- [ ] Commit: `git commit -m "feat: redesign owner dashboard"`

### Step 4.3: Booking Wizard
- [ ] Open `src/pages/BookingPage.vue`
- [ ] Update progress bar styling
- [ ] Update pet selection:
  - [ ] Make cards more visual
  - [ ] Add selection indicators
  - [ ] Update colors
- [ ] Update date picker styling
- [ ] Update time slot selection
- [ ] Add confirmation review section
- [ ] Update success state
- [ ] Test all steps
- [ ] Test form submission
- [ ] Test error states
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: redesign booking wizard"`

### Step 4.4: Vet Dashboard
- [ ] Open `src/pages/VetDashboard.vue`
- [ ] Redesign alert cards
  - [ ] Color-coded by urgency
  - [ ] Update styling
  - [ ] Make dismissible
- [ ] Update appointment list styling
- [ ] Update stat cards
- [ ] Add quick filters
- [ ] Test dark mode
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: redesign vet dashboard"`

### Step 4.5: Visit Details Page
- [ ] Open `src/pages/VisitDetails.vue`
- [ ] Implement two-column layout:
  - [ ] Left: visit info and notes (60%)
  - [ ] Right: pet info and timeline (40%)
- [ ] Update card styling
- [ ] Update treatment section
- [ ] Update notes display
- [ ] Update action buttons
- [ ] Test dark mode
- [ ] Test responsive (becomes single column on tablet)
- [ ] Commit: `git commit -m "feat: redesign visit details"`

### Step 4.6: Health Records Page
- [ ] Open `src/pages/HealthRecords.vue`
- [ ] Update pet tabs styling
- [ ] Redesign vaccination timeline
- [ ] Update medical conditions list
- [ ] Redesign medications table
- [ ] Update action buttons
- [ ] Test dark mode
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: redesign health records"`

### Step 4.7: Invoices Page
- [ ] Open `src/pages/InvoicesPage.vue`
- [ ] Add summary stat cards
- [ ] Redesign filter bar (segmented control)
- [ ] Update invoice table/list
- [ ] Improve status badge styling
- [ ] Update action buttons
- [ ] Redesign invoice detail modal
- [ ] Test filtering
- [ ] Test dark mode
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: redesign invoices page"`

### Step 4.8: Inventory Page
- [ ] Open `src/pages/InventoryPage.vue`
- [ ] Redesign alert cards
- [ ] Add category organization
- [ ] Improve stock visualization
- [ ] Update inventory table
- [ ] Update add item modal
- [ ] Test filtering
- [ ] Test dark mode
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: redesign inventory page"`

### Step 4.9: Visit History Page
- [ ] Open `src/pages/VisitHistory.vue`
- [ ] Update filter styling
- [ ] Improve visit list display
- [ ] Update date formatting
- [ ] Update action buttons
- [ ] Improve empty state
- [ ] Test filtering
- [ ] Test dark mode
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: redesign visit history"`

### Step 4.10: Patients Page
- [ ] Open `src/pages/PatientsPage.vue`
- [ ] Redesign search/filter
- [ ] Update patient cards/list
- [ ] Update icons (Heroicons)
- [ ] Improve visual organization
- [ ] Test search functionality
- [ ] Test filtering
- [ ] Test dark mode
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: redesign patients page"`

### Step 4.11: Create My Pets Page (NEW)
- [x] Create `src/pages/MyPetsPage.vue`
- [x] Split pet management from dashboard
- [ ] Implement grid/list toggle
- [x] Update pet cards styling
- [x] Add quick actions
- [ ] Create pet detail modal/page
- [x] Add route in router/index.ts
- [ ] Update navigation in MainLayout
- [x] Test functionality
- [ ] Test dark mode
- [ ] Test responsive
- [ ] Commit: `git commit -m "feat: create dedicated my pets page"`

### Phase 4 Verification
- [ ] All pages render correctly
- [ ] All forms work
- [ ] All buttons work
- [ ] Navigation works on all pages
- [ ] Dark mode verified for all pages
- [ ] No console errors
- [ ] Commit: `git commit -m "feat: complete all page redesigns"`

**Phase 4 Status:** ⏳ Ready to start

---

## Phase 5: Testing & Refinement (Days 13-15)

### Step 5.1: Accessibility Testing
- [ ] Keyboard navigation
  - [ ] Tab through all pages
  - [ ] Verify tab order is logical
  - [ ] Test focus visibility
  - [ ] Check for keyboard traps
- [ ] Screen reader testing
  - [ ] Test with NVDA or JAWS
  - [ ] Verify ARIA labels
  - [ ] Check heading hierarchy
  - [ ] Test form labels
- [ ] Color contrast
  - [ ] Use contrast checker tool
  - [ ] Verify 4.5:1 for normal text
  - [ ] Verify 3:1 for large text
  - [ ] Check dark mode contrast
- [ ] Component accessibility
  - [ ] Modal focus management
  - [ ] Form error announcements
  - [ ] Button labels clear
  - [ ] Links descriptive
- [ ] Report any issues found
- [ ] Fix critical issues immediately
- [ ] Log non-critical issues for post-launch

### Step 5.2: Responsive Testing
- [ ] Mobile (375px)
  - [ ] All content visible
  - [ ] Touch targets 48px+
  - [ ] Navigation works
  - [ ] No horizontal scroll
- [ ] Tablet (768px)
  - [ ] Layout optimized
  - [ ] All features accessible
  - [ ] Navigation works
- [ ] Desktop (1024px)
  - [ ] Full layout working
  - [ ] Sidebar visible
  - [ ] All features accessible
- [ ] Large Desktop (1280px+)
  - [ ] Proper spacing and layout
  - [ ] No excessive whitespace
- [ ] Landscape orientations
  - [ ] Mobile landscape works
  - [ ] Tablet landscape works
- [ ] Test on actual devices if possible
- [ ] Log any responsive issues

### Step 5.3: Dark Mode Testing
- [ ] Navigate all pages in dark mode
- [ ] Verify text is readable
  - [ ] Check contrast ratios
  - [ ] Verify no white ghosts
  - [ ] Check gray scales
- [ ] Verify images visible
- [ ] Verify borders/dividers visible
- [ ] Verify icons visible
- [ ] Check form input styling
- [ ] Check modal styling
- [ ] Check card styling
- [ ] Test modal styling in dark mode
- [ ] Verify toggle works
- [ ] Save preference in localStorage
- [ ] Log any dark mode issues

### Step 5.4: Cross-browser Testing
- [ ] Chrome/Chromium
  - [ ] CSS renders correctly
  - [ ] Animations smooth
  - [ ] No console errors
- [ ] Firefox
  - [ ] CSS renders correctly
  - [ ] Flexbox works
  - [ ] Grid works
  - [ ] Animations smooth
- [ ] Safari (desktop)
  - [ ] CSS compatibility
  - [ ] Vendor prefixes work
  - [ ] Animations smooth
- [ ] Safari (mobile/iPad)
  - [ ] Touch interactions work
  - [ ] Safe area respected
  - [ ] Keyboard support works
- [ ] Edge
  - [ ] CSS rendering
  - [ ] Compatibility
- [ ] Log any browser-specific issues

### Step 5.5: Performance Testing
- [ ] Run Lighthouse audit
  - [ ] Performance score
  - [ ] Accessibility score
  - [ ] Best practices score
  - [ ] SEO score
- [ ] Check CSS bundle size
  - [ ] Should not increase significantly
  - [ ] Tailwind purge working
- [ ] Check JavaScript bundle size
  - [ ] No unexpected increases
- [ ] Test image optimization
  - [ ] All images optimized
  - [ ] No huge unoptimized files
- [ ] Verify font loading
  - [ ] No FOUT (Flash of Unstyled Text)
  - [ ] Fast load times
- [ ] Test page load time
  - [ ] First Contentful Paint < 1.5s
  - [ ] Largest Contentful Paint < 2.5s
- [ ] Log any performance issues

### Step 5.6: Feature Testing
- [ ] Test all existing features
  - [ ] Authentication works
  - [ ] Dashboard loads data
  - [ ] Forms submit correctly
  - [ ] APIs respond properly
  - [ ] Error handling works
  - [ ] Loading states work
- [ ] Test new components
  - [ ] Avatar displays correctly
  - [ ] Tabs switching works
  - [ ] Table sorting works (if implemented)
  - [ ] Breadcrumb navigation works
- [ ] Test page flows
  - [ ] Booking wizard complete flow
  - [ ] Pet creation complete flow
  - [ ] Visit details access flow
- [ ] Log any functional issues

### Phase 5 Issues Resolution
- [ ] Categorize issues by severity
  - [ ] Critical (blocks functionality)
  - [ ] High (impacts usability)
  - [ ] Medium (cosmetic improvements)
  - [ ] Low (nice to have)
- [ ] Fix all critical issues
- [ ] Fix all high-priority issues
- [ ] Document medium/low issues
- [ ] Re-test after fixes
- [ ] Commit: `git commit -m "fix: resolve testing issues"`

**Phase 5 Status:** ⏳ Ready to start

---

## Phase 6: Final Polish & Documentation (Day 16)

### Step 6.1: Bug Fixes
- [ ] Fix any remaining issues from testing
- [ ] Fine-tune styling:
  - [ ] Check spacing consistency
  - [ ] Verify color accuracy
  - [ ] Test shadow levels
  - [ ] Verify border radius
- [ ] Adjust responsive breakpoints if needed
- [ ] Update visual feedback for interactions
- [ ] Test all hover states again
- [ ] Test all focus states again
- [ ] Ensure smooth animations
- [ ] Commit: `git commit -m "fix: final styling adjustments"`

### Step 6.2: Documentation Updates
- [ ] Update component README (if exists)
- [ ] Document design decisions
  - [ ] Why new colors chosen
  - [ ] Why layout changed
  - [ ] Icon system rationale
- [ ] Create style guide snapshot
- [ ] Add design system reference
- [ ] Update development guide
- [ ] Document any breaking changes
- [ ] Create migration guide (if needed)
- [ ] Commit: `git commit -m "docs: update documentation"`

### Step 6.3: Deployment Preparation
- [ ] Build for production
  ```bash
  npm run build
  ```
- [ ] Test production build locally
  ```bash
  npm run preview
  ```
- [ ] Verify all features work in production build
- [ ] Check for console errors
- [ ] Test API integration again
- [ ] Verify performance metrics
- [ ] Check bundle sizes
- [ ] Create deployment checklist
- [ ] Notify stakeholders of readiness
- [ ] Final PR review
- [ ] Commit: `git commit -m "chore: prepare for deployment"`

### Step 6.4: Final Verification Checklist
- [ ] All pages load without errors
- [ ] All features work correctly
- [ ] Dark mode works throughout
- [ ] Responsive design verified
- [ ] Accessibility compliant (WCAG 2.1 AA)
- [ ] Performance acceptable
- [ ] Cross-browser tested
- [ ] All commits made with meaningful messages
- [ ] Documentation complete
- [ ] Code reviewed
- [ ] Ready for deployment

**Phase 6 Status:** ⏳ Ready to start

---

## Post-Deployment

### Launch Checklist
- [ ] Deploy to production environment
- [ ] Monitor for errors
- [ ] Gather user feedback
- [ ] Check analytics
- [ ] Monitor performance metrics
- [ ] Be ready for quick hotfixes

### Documentation
- [ ] Keep REDESIGN-SUMMARY.md updated
- [ ] Keep MEMORY.md updated for future sessions
- [ ] Archive any working notes
- [ ] Create post-launch report

---

## Progress Tracking

### Overall Progress Percentage

- [ ] Pre-Implementation: **0%**
- [ ] Phase 1 Complete: **15%**
- [ ] Phase 2 Complete: **35%**
- [ ] Phase 3 Complete: **50%**
- [ ] Phase 4 Complete: **80%**
- [ ] Phase 5 Complete: **95%**
- [ ] Phase 6 Complete: **100%**

### Time Tracking

```
Phase 1: ___ hours / 6 hours
Phase 2: ___ hours / 15 hours
Phase 3: ___ hours / 9 hours
Phase 4: ___ hours / 33 hours
Phase 5: ___ hours / 15 hours
Phase 6: ___ hours / 8 hours
---
Total: ___ hours / 86 hours
```

---

## Notes Section

Use this space to track notes, blockers, or improvements:

```
Date | Task | Notes | Status
-----|------|-------|-------
     |      |       |
     |      |       |
     |      |       |
```

---

## Final Sign-Off

- [ ] All phases completed
- [ ] Testing passed
- [ ] Documentation complete
- [ ] Ready for production
- [ ] Stakeholder approval received
- [ ] Deployment scheduled

**Completion Date:** _______________
**Deployed By:** _______________
**Sign-Off:** _______________

---

**Good luck with your redesign! 🚀 You've got this!**

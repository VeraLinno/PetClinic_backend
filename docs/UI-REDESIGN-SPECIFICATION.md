# Pet Clinic UI Redesign Specification

**Document Version:** 1.0
**Date:** March 2026
**Project:** Pet Clinic Application Modernization

**Implementation Snapshot (March 2026):**
- Auth persistence and startup rehydration are implemented.
- Register route and protected redirect flow are active.
- New pages include `MyPetsPage` and `VetTodayAppointments`.
- Vet inventory includes incoming and delivered reorder data views.

---

## Executive Summary

This document provides a comprehensive redesign specification for the Pet Clinic application, modernizing the user interface while preserving all existing functionality. The redesign focuses on:

- **Modern, clean aesthetic** following 2024-2026 design trends
- **Enhanced user experience** with improved information hierarchy
- **Accessibility improvements** (WCAG 2.1 AA compliance)
- **Performance optimization** through better component architecture
- **Consistency across all pages** and user flows
- **Responsive design** that works flawlessly on all devices

---

## 1. CURRENT STATE ANALYSIS

### 1.1 Current Application Structure

**Backend:** ASP.NET Core (C#) with Clean Architecture
**Frontend:** Vue 3.3 + Tailwind CSS 3.3
**State Management:** Pinia 2.1
**Build Tool:** Vite 4.3

### 1.2 Current Pages & Features

#### Owner-Facing Features:
- ✓ Authentication (Login/Register)
- ✓ Dashboard with pet and appointment overview
- ✓ Pet management (add, view, delete)
- ✓ Appointment booking (multi-step wizard)
- ✓ Visit history with filtering
- ✓ Health records with vaccination tracking
- ✓ Invoice management with status filtering
- ✓ Appointment management and cancellation

#### Vet-Facing Features:
- ✓ Dashboard with today's appointments
- ✓ Appointment management
- ✓ Patient records with search/filter
- ✓ Visit details and notes management
- ✓ Inventory management
- ✓ Low stock alerts
- ✓ Treatment tracking

### 1.3 Design Issues Identified

#### Visual Design Issues:
1. **Color palette lacks sophistication**
   - Primary blue (#2563eb) feels generic
   - Limited semantic meaning in color usage
   - Inconsistent opacity/gradient usage

2. **Typography is functional but uninspiring**
   - System font stack is safe but dated
   - No distinct typographic hierarchy
   - Limited font weights used effectively

3. **Component styling is repetitive**
   - Many inline Tailwind classes
   - No consistent shadow/elevation system
   - Loading states vary between pages

4. **Dark mode implementation is incomplete**
   - Some pages lack proper dark mode styling
   - Contrast issues in some dark mode combinations
   - Inconsistent color transitions

#### UX/Usability Issues:
1. **Information hierarchy could be clearer**
   - Competing visual weights
   - Card designs lack differentiation
   - CTA buttons not sufficiently prominent

2. **Navigation could be more intuitive**
   - Emoji icons are cute but unprofessional
   - Menu organization could be clearer
   - Breadcrumb navigation missing

3. **Forms lack polish**
   - Error states not distinctive enough
   - Validation feedback unclear
   - No inline help text pattern established

4. **Empty/loading states inconsistent**
   - Mix of spinner animations and skeleton screens
   - No unified loading strategy
   - Empty states lack personality

#### Accessibility Issues:
1. **Modal accessibility gaps**
   - No proper aria-modal attribute
   - Focus trap not implemented
   - Close button labeling inconsistent

2. **Skip navigation missing**
   - No skip-to-main-content link
   - Focus management could be improved

3. **Error handling**
   - aria-describedby not used for form errors
   - Error messages not always announced
   - Field associations unclear

---

## 2. MODERN DESIGN SYSTEM

### 2.1 Design Philosophy

The redesign follows these principles:

1. **Minimalism with Purpose**
   - Remove unnecessary visual elements
   - Increase whitespace strategically
   - Focus attention on important interactions

2. **Clarity & Hierarchy**
   - Clear visual distinction between elements
   - Consistent size and weight relationships
   - Logical information grouping

3. **Accessibility First**
   - WCAG 2.1 AA compliant
   - Inclusive color palettes
   - Semantic HTML structure

4. **Performance Conscious**
   - Optimize CSS/JavaScript delivery
   - Efficient component architecture
   - Lazy loading where appropriate

5. **Professional & Trustworthy**
   - Medical/healthcare aesthetic appropriate
   - Clean, polished appearance
   - Consistent experience across devices

### 2.2 Updated Color Palette

#### Primary Colors (Healthcare-Inspired)
```
Primary (Medical Blue):
  50:   #f0f9ff
  100:  #e0f2fe
  200:  #bae6fd
  300:  #7dd3fc
  400:  #38bdf8
  500:  #0ea5e9  ← Main primary (more professional than #2563eb)
  600:  #0284c7
  700:  #0369a1  ← For interactive hover states
  800:  #075985
  900:  #0c3d66  ← For dark backgrounds
  950:  #051e34

Secondary (Professional Teal):
  50:   #f0fdfa
  100:  #ccfbf1
  200:  #99f6e4
  300:  #5eead4
  400:  #2dd4bf
  500:  #14b8a6  ← Accent/Secondary
  600:  #0d9488
  700:  #0f766e
  800:  #115e59
  900:  #134e4a
  950:  #0d3432
```

#### Semantic Colors
```
Success (Healing Green):
  50:   #f0fdf4
  100:  #dcfce7
  200:  #bbf7d0
  300:  #86efac
  400:  #4ade80
  500:  #22c55e  ← Success (more vibrant)
  600:  #16a34a
  700:  #15803d
  800:  #166534
  900:  #145231
  950:  #052e16

Warning (Alert Orange):
  50:   #fffbeb
  100:  #fef3c7
  200:  #fde68a
  300:  #fcd34d
  400:  #fbbf24
  500:  #f59e0b
  600:  #d97706
  700:  #b45309  ← Warning
  800:  #92400e
  900:  #78350f
  950:  #451a03

Danger (Medical Red):
  50:   #fef2f2
  100:  #fee2e2
  200:  #fecaca
  300:  #fca5a5
  400:  #f87171
  500:  #ef4444
  600:  #dc2626
  700:  #b91c1c  ← Danger
  800:  #991b1b
  900:  #7f1d1d
  950:  #450a0a

Info (Informational Blue):
  50:   #f0f9ff
  100:  #e0f2fe
  500:  #0ea5e9
  600:  #0284c7
  700:  #0369a1
```

#### Neutral Colors (Refined)
```
Slate for true neutral:
  50:   #f8fafc
  100:  #f1f5f9
  200:  #e2e8f0
  300:  #cbd5e1
  400:  #94a3b8
  500:  #64748b
  600:  #475569
  700:  #334155
  800:  #1e293b
  900:  #0f172a
  950:  #020617
```

### 2.3 Typography System

#### Font Families
```
Primary Font (Professional & Modern):
- Font: "Inter" or "Outfit" (from Google Fonts)
- Fallback: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto
- Usage: All UI elements, body text, headings

Code Font (System Fonts):
- Font: "Fira Code" or "JetBrains Mono" (for any data display)
- Fallback: 'Monaco', 'Courier New', monospace
- Usage: Medical codes, appointment IDs, identifiers
```

#### Font Sizes & Line Heights
```
Typography Scale:

Display Large:
- Size: 3.5rem (56px)
- Line Height: 1.1
- Weight: 700
- Usage: Page hero titles

Display:
- Size: 2.75rem (44px)
- Line Height: 1.2
- Weight: 600
- Usage: Page main headings

Heading 1:
- Size: 2.25rem (36px)
- Line Height: 1.25
- Weight: 600
- Usage: Section titles

Heading 2:
- Size: 1.875rem (30px)
- Line Height: 1.3
- Weight: 600
- Usage: Card headers, section subtitles

Heading 3:
- Size: 1.5rem (24px)
- Line Height: 1.35
- Weight: 600
- Usage: Component titles

Heading 4:
- Size: 1.25rem (20px)
- Line Height: 1.4
- Weight: 600
- Usage: Subsection titles

Body Large:
- Size: 1.125rem (18px)
- Line Height: 1.6
- Weight: 400/500
- Usage: Large body text, emphasis

Body:
- Size: 1rem (16px)
- Line Height: 1.6
- Weight: 400
- Usage: Default body text

Body Small:
- Size: 0.875rem (14px)
- Line Height: 1.5
- Weight: 400
- Usage: Secondary text, captions

Small:
- Size: 0.75rem (12px)
- Line Height: 1.5
- Weight: 500
- Usage: Labels, badges, metadata

Tiny:
- Size: 0.625rem (10px)
- Line Height: 1.4
- Weight: 600
- Usage: Small labels, tips
```

#### Font Weights
```
- Thin:     100 (not used in UI)
- Light:    300 (body emphasis)
- Regular:  400 (default body)
- Medium:   500 (labels, subtitles)
- Semibold: 600 (headings, emphasis)
- Bold:     700 (display headings)
```

### 2.4 Spacing & Layout

#### Spacing Scale
```
0:     0px
1:     0.25rem (4px)
2:     0.5rem (8px)
3:     0.75rem (12px)
4:     1rem (16px)
6:     1.5rem (24px)
8:     2rem (32px)
10:    2.5rem (40px)
12:    3rem (48px)
16:    4rem (64px)
20:    5rem (80px)
24:    6rem (96px)
```

#### Layout Guidelines
```
Container Max-Widths:
- Mobile:  100% (with 16px padding)
- Tablet:  100% (with 24px padding)
- Desktop: 1200px (with 40px padding)
- Large:   1400px (centered)
- Ultra:   1600px (centered)

Grid System:
- 4-column on mobile
- 8-column on tablet
- 12-column on desktop
- Gutter: 16px (mobile), 24px (tablet), 32px (desktop)

Component Spacing:
- Card padding: 24px (edge) / 32px (section padding)
- Element gap: 16px (default) / 8px (compact) / 24px (loose)
- Top margin for blocks: 32px (default) / 48px (large)
```

### 2.5 Shadow & Elevation System

```
Elevation Levels:
  Level 0 (No shadow):
    - Backgrounds, base elements
    - Box-shadow: none

  Level 1 (Subtle):
    - Form inputs, light cards
    - Box-shadow: 0 1px 2px 0 rgba(0,0,0,0.05)

  Level 2 (Card):
    - Default cards, sections
    - Box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1), 0 2px 4px -2px rgba(0,0,0,0.1)

  Level 3 (Hover):
    - Hovered cards, lifted elements
    - Box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1), 0 4px 6px -4px rgba(0,0,0,0.1)

  Level 4 (Modal):
    - Modals, dropdowns, popovers
    - Box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1), 0 8px 10px -6px rgba(0,0,0,0.1)

  Level 5 (Overlay):
    - Modal backdrop
    - Background: rgba(0,0,0,0.5)
```

### 2.6 Border & Radius System

```
Border Radius:
  sm:     4px    (form inputs, small elements)
  md:     8px    (default, most components)
  lg:     12px   (large cards, sections)
  xl:     16px   (major sections, modals)
  2xl:    20px   (oversized elements)
  full:   9999px (circles, badges)

Border Width:
  1px:    default (dividers, input borders)
  2px:    emphasis (selected, focused)
  4px:    heavy (thick dividers, important lines)

Border Color:
  Light:  #e2e8f0 (slate-200)
  Medium: #cbd5e1 (slate-300)
  Dark:   #94a3b8 (slate-400)
```

### 2.7 Transition & Animation System

```
Timing Functions:
  - ease-in-out: cubic-bezier(0.4, 0, 0.2, 1)
  - ease-out: cubic-bezier(0, 0, 0.2, 1)
  - ease-in: cubic-bezier(0.4, 0, 1, 1)
  - linear: linear

Durations:
  - Fast:     150ms (hover states, small interactions)
  - Base:     200ms (default transitions)
  - Slow:     300ms (larger animations, page transitions)
  - Slower:   500ms (major page changes)

Transitions:
  - Colors:      200ms ease-in-out
  - Opacity:     200ms ease-in-out
  - Transform:   200ms ease-in-out
  - All:         200ms ease-in-out

Animation Uses:
  - Skeleton pulse:     1.5s infinite ease-in-out
  - Spinner:            1s linear infinite
  - Bounce (error):     300ms ease-in-out
  - Fade in (entrance): 200ms ease-out
  - Slide (navigation): 150ms ease-in-out
```

### 2.8 Icon System

#### Icon Approach
- **Primary:** Heroicons (from Tailwind Labs)
- **Backup:** Lucide Icons
- **Style:** Outline (24px) and solid (16-20px) variants

#### Icon Usage Guidelines
```
Navigation Icons:     24px, outline, gray-600
Interactive Icons:    20px, outline, inherit color
Status Icons:         16px, solid, semantic color
Action Icons:         20px, outline, primary-500
Decorative Icons:     24px, outline, gray-400
Alert Icons:          24px, solid, semantic color
```

#### Icon Categories
```
Navigation:
- Home (outline)
- Calendar (outline)
- Heart/Love (outline)
- FileText/History (outline)
- DollarSign/Invoice (outline)
- Hospital/HealthRecords (outline)
- Users/Patients (outline)
- Package/Inventory (outline)
- LogOut (outline)

Actions:
- Plus (add)
- Edit/Pencil
- Trash (delete)
- Search (magnifying glass)
- Filter
- Sort
- Download
- Share

Status:
- CheckCircle (success)
- XCircle (error)
- AlertCircle (warning)
- InfoCircle (info)
- Clock (pending)
- Checkmark (completed)

Object:
- Paw (pet)
- Pet type icons (Dog, Cat, Bird, etc.)
- Calendar events
- Medication bottle
- Stethoscope
```

---

## 3. COMPONENT REDESIGN SPECIFICATIONS

### 3.1 Button Component

#### Variants

**Primary Button**
```
Default State:
  Background: Cyan-500 (#0ea5e9)
  Text Color: White
  Padding: 10px 16px (sm), 12px 20px (md), 14px 24px (lg)
  Border Radius: 8px
  Font Weight: 600
  Font Size: 14px (sm), 16px (md), 18px (lg)
  Box Shadow: 0 1px 2px rgba(0,0,0,0.05)

Hover State:
  Background: Cyan-600 (#0284c7)
  Box Shadow: 0 4px 6px rgba(0,0,0,0.1)
  Transform: translateY(-1px)

Focus State:
  Outline: 2px solid transparent
  Box Shadow: 0 0 0 3px rgba(6,182,212,0.1), 0 0 0 2px #0ea5e9

Active State:
  Background: Cyan-700 (#0369a1)
  Transform: translateY(0)

Disabled State:
  Background: Slate-200 (#e2e8f0)
  Text Color: Slate-500 (#64748b)
  Cursor: not-allowed
  Opacity: 0.5
```

**Secondary Button** (Outline variant)
```
Default State:
  Background: Transparent
  Border: 2px solid Slate-300 (#cbd5e1)
  Text Color: Slate-700 (#334155)
  Padding: 9px 15px (sm), 11px 19px (md), 13px 23px (lg)

Hover State:
  Background: Slate-50 (#f8fafc)
  Border Color: Slate-400 (#94a3b8)

Focus State:
  Box Shadow: 0 0 0 3px rgba(100,116,139,0.1)
```

**Danger Button**
```
Default State:
  Background: Red-600 (#dc2626)
  Text Color: White

Hover State:
  Background: Red-700 (#b91c1c)
```

**Ghost Button**
```
Default State:
  Background: Transparent
  Text Color: Slate-600 (#475569)

Hover State:
  Background: Slate-100 (#f1f5f9)
```

#### Size Variants
```
sm:  Padding 8px 12px,  Font 12px, Height 32px
md:  Padding 10px 16px, Font 14px, Height 40px
lg:  Padding 12px 24px, Font 16px, Height 48px
```

#### Special Features
- Loading state with spinner icon and disabled cursor
- Icon support (left/right positioning)
- Full width option
- Grouped button support

### 3.2 Card Component

```
Default State:
  Background: White (#ffffff)
  Border: 1px solid Slate-200 (#e2e8f0)
  Border Radius: 12px
  Box Shadow: 0 1px 3px rgba(0,0,0,0.1)
  Padding: 24px

Hover State (interactive):
  Box Shadow: 0 4px 6px rgba(0,0,0,0.1)
  Border Color: Slate-300 (#cbd5e1)

Focus State (keyboard navigation):
  Outline: 2px solid Cyan-500

Dark Mode:
  Background: Slate-800 (#1e293b)
  Border Color: Slate-700 (#334155)

Header:
  Padding: 24px
  Border Bottom: 1px solid Slate-200
  Font Weight: 600
  Font Size: 18px

Footer:
  Padding: 16px 24px
  Border Top: 1px solid Slate-200
  Background: Slate-50 (#f8fafc)
```

### 3.3 Input Component

```
Text Input:
  Height: 40px
  Padding: 10px 12px
  Border: 1px solid Slate-300
  Border Radius: 8px
  Font: 14px, 400 weight
  Background: White

Focus State:
  Border Color: Cyan-500
  Box Shadow: 0 0 0 3px rgba(6,182,212,0.1)

Error State:
  Border Color: Red-500
  Background: Red-50 (#fef2f2)

Placeholder:
  Color: Slate-400 (#94a3b8)

Label:
  Font: 14px, 500 weight
  Color: Slate-700
  Margin Bottom: 8px

Help Text:
  Font: 12px, 400 weight
  Color: Slate-500
  Margin Top: 4px

Error Message:
  Font: 12px, 500 weight
  Color: Red-600
  Margin Top: 6px
```

### 3.4 Modal Component

```
Overlay:
  Background: rgba(0,0,0,0.5)
  Animation: Fade in 200ms ease-out

Modal Container:
  Background: White
  Border Radius: 12px
  Box Shadow: 0 20px 25px rgba(0,0,0,0.15)
  Width: 90% (mobile), 600px (tablet+)
  Max Height: 90vh
  Animation: Scale in 200ms ease-out

Header:
  Padding: 24px
  Border Bottom: 1px solid Slate-200
  Title: 20px, 600 weight, Slate-900
  Close Button: Top right, 32x32px, icon 16px

Content:
  Padding: 24px
  Max Height: calc(90vh - 120px)
  Overflow Y: auto

Footer:
  Padding: 16px 24px
  Border Top: 1px solid Slate-200
  Background: Slate-50
  Display: flex, justify-end, gap 12px
```

### 3.5 Toast Component

```
Container:
  Position: Fixed
  Top: 24px
  Right: 24px
  Width: 360px (mobile 90%, tablet 400px)
  Padding: 16px
  Border Radius: 8px
  Box Shadow: 0 10px 15px rgba(0,0,0,0.1)
  Animation: Slide in from right 200ms ease-out

Success:
  Background: Green-50 (#f0fdf4)
  Border Left: 4px solid Green-500
  Text Color: Green-900
  Icon: Check circle, Green-500

Error:
  Background: Red-50 (#fef2f2)
  Border Left: 4px solid Red-500
  Text Color: Red-900
  Icon: X circle, Red-500

Warning:
  Background: Yellow-50 (#fffbeb)
  Border Left: 4px solid Yellow-500
  Text Color: Yellow-900
  Icon: Alert triangle, Yellow-500

Info:
  Background: Blue-50 (#f0f9ff)
  Border Left: 4px solid Blue-500
  Text Color: Blue-900
  Icon: Info circle, Blue-500

Close Button:
  Right: 12px
  Top: 12px
  24px square icon

Auto Dismiss: 5000ms (configurable)
```

### 3.6 Badge Component

```
Default Badge:
  Padding: 4px 12px
  Border Radius: 9999px (full)
  Font: 12px, 500 weight

Small:  Padding 2px 8px,   Font 11px
Medium: Padding 4px 12px,  Font 12px (default)
Large:  Padding 6px 16px,  Font 13px

Variants:

Cyan (Primary):
  Background: Cyan-100 (#cffafe)
  Text: Cyan-800 (#155e75)

Success (Green):
  Background: Green-100 (#dcfce7)
  Text: Green-800 (#166534)

Warning (Orange):
  Background: Orange-100 (#ffedd5)
  Text: Orange-800 (#92400e)

Danger (Red):
  Background: Red-100 (#fee2e2)
  Text: Red-800 (#991b1b)

Info (Blue):
  Background: Blue-100 (#dbeafe)
  Text: Blue-800 (#1e40af)

Neutral:
  Background: Slate-100 (#f1f5f9)
  Text: Slate-700 (#334155)

Outline Variant:
  Background: Transparent
  Border: 1px solid [color 300]
  Text: [color 700]
```

### 3.7 Avatar Component (NEW)

```
Avatar:
  Display: Circular
  Sizes: 32px (sm), 48px (md), 64px (lg)
  Border Radius: 9999px
  Background: Cyan-500 (default)
  Text Color: White
  Font: Bold, centered

Image Avatar:
  Object Fit: cover

Fallback:
  User initials (1-2 characters)

Status Indicator:
  4px dot in bottom-right corner
  Position: Absolute
  Options: online (Green-500), offline (Slate-400), away (Yellow-500), busy (Red-500)
```

### 3.8 Tabs Component (NEW)

```
Tab Bar:
  Display: flex
  Border Bottom: 1px solid Slate-200
  Padding: 0

Tab Item:
  Padding: 12px 16px
  Font: 14px, 500 weight
  Color: Slate-600 (inactive)
  Border Bottom: 2px solid transparent (inactive)
  Cursor: pointer

Active Tab:
  Color: Cyan-700
  Border Bottom: 2px solid Cyan-500

Hover (Inactive):
  Color: Slate-700
  Background: Slate-50

Content Panel:
  Padding: 24px 0
  Animation: Fade in 200ms
```

### 3.9 Data Table Component (NEW)

```
Table Wrapper:
  Overflow X: auto
  Border Radius: 8px
  Border: 1px solid Slate-200

Header Row:
  Background: Slate-50 (#f8fafc)
  Border Bottom: 2px solid Slate-200
  Font Weight: 600
  Font Size: 14px
  Padding: 12px 16px

Data Row:
  Border Bottom: 1px solid Slate-200
  Padding: 12px 16px
  Font Size: 14px

Hover (interactive rows):
  Background: Slate-50

Alternating Rows:
  Even: White
  Odd: Slate-50

Sortable Column Header:
  Cursor: pointer
  Icon: Sort icon (gray) / Up/Down arrow (active)

Pagination:
  Below table
  Show X results per page options
  Previous/Next buttons
  Jump to page input
```

---

## 4. PAGE REDESIGN SPECIFICATIONS

### 4.1 Authentication Pages (Login/Register)

#### Current Issues:
- Generic form layout
- No visual branding
- Minimal error messaging
- No terms/privacy links

#### Redesigned Approach:

**Layout:**
```
Full viewport with:
- Left side (50%): Gradient background with brand/healthcare messaging
- Right side (50%): Form on white background
- Mobile: Single column stacked layout

Left Brand Side (Hidden on mobile):
  - Gradient: Cyan-500 to Teal-600
  - Large icon: Stethoscope + Paw combination
  - Headline: "Pet Care Management"
  - Subheading: "Professional veterinary practice software"
  - Feature list (3 items with icons)
  - Light accent graphics

Right Form Side:
  - Max width 420px
  - Centered vertically
  - Logo/brand name at top
  - Form fields with better spacing
  - Submit button (full width, large)
  - Toggle between login/register
  - "Forgot password?" link (future feature)
  - Terms agreement checkbox (register)
  - Social login buttons (future)
```

**Form Elements:**
- Email input with inline validation
- Password input with show/hide toggle
- Form errors styled with red background + border
- Loading state on submit button
- Success/error toast notifications

**Visual Enhancements:**
- Subtle background pattern on left side
- Smooth transitions between form states
- Better visual feedback for errors
- Clear distinction between form sections

### 4.2 Owner Dashboard

#### Current Issues:
- Profile section too large
- Stats cards lack context
- Pet cards could be more visual
- Appointment list is plain

#### Redesigned Approach:

**Layout:**
```
Hero Section:
  - Compact profile banner
  - Welcome message with time-based greeting
  - Quick action buttons (Book, View Invoices)
  - Responsive: full width on mobile, full height sidebar on desktop

Stats Section:
  - Grid: 3 columns on desktop, 1-2 mobile
  - Cards with larger numbers
  - Subtle icons with specific colors
  - Hover reveals breakdown details
  - Clickable to navigate to detailed view

Pet Management:
  - "My Pets" section with add button
  - Card grid: 4 columns desktop, 2 tablet, 1 mobile
  - Pet card redesign:
    * Large visual: emoji → custom colored badge with pet icon
    * Pet name prominent
    * Species and age below
    * Hover shows action buttons: Edit, Book, More options

  - Empty state:
    * Large icon (paw outline)
    * Clear message
    * Prominent "Add Pet" button

Appointments Section:
  - Tab view: Upcoming | Completed | All
  - Timeline-style list showing:
    * Pet icon + name
    * Service type
    * Date/time with calendar icon
    * Vet name
    * Status badge
    * Action buttons on hover: View, Reschedule, Cancel
  - Empty state with suggested actions

Health Alerts (NEW):
  - Vaccination due, medication refills, check-ups
  - Color-coded alert cards
  - Quick action buttons
  - Dismiss/snooze options
```

### 4.3 Booking Wizard

#### Current Issues:
- Step progress could be clearer
- Pet selection is not visual enough
- Date picker is basic
- Time selection interface could be better

#### Redesigned Approach:

**Visual Progress:**
```
Step indicator redesign:
  - Horizontal progress bar at top
  - Step circles: completed (checkmark), active (number), pending (faded)
  - Step labels: hidden on mobile, shown on desktop
  - Current step highlighted prominently
```

**Step 1: Pet Selection**
```
Large pet cards with:
  - Colored badge (species-specific color)
  - Pet emoji or illustration
  - Name (18px, bold)
  - "Species • Breed • Age"
  - Selection indicator (checkmark + border)
  - Hover shadow increase

Selected state:
  - Cyanish border + background
  - Checkmark in corner
```

**Step 2: Date Selection**
```
Calendar picker:
  - Mini calendar with month navigation
  - Date grid with clickable dates
  - Today marked with dot
  - Selected date highlighted
  - Disabled past dates
  - Next available dates highlighted

Below calendar:
  - Selected date summary
  - "Change date" button
```

**Step 3: Time Selection**
```
Time slots:
  - Grid of available times (AM/PM sections)
  - Each slot: time + availability
  - Interactive: hover shows vet name, service duration
  - Selected time highlighted
  - Slots update based on selection

Appointment summary:
  - Pet icon + name
  - Date shorthand
  - Time confirmation
  - Estimated duration
  - Service type (if applicable)
```

**Final Step: Confirmation**
```
Review section:
  - Pet details in card
  - Date/time in card
  - Service details
  - Notes (if any)
  - Vet assignment (if showing)

Actions:
  - Back to adjust
  - Confirm booking (prominent CTA)

Confirmation state:
  - Success icon + checkmark
  - Confirmation number
  - Next steps
  - Add to calendar button
  - Share with vet toggle
```

### 4.4 Vet Dashboard

#### Current Issues:
- Low stock alerts feel disconnected
- Stat cards lack visual hierarchy
- No quick action bar
- Appointment list is basic

#### Redesigned Approach:

**Hero Section:**
```
Compact header with:
  - Time-based greeting ("Good morning, Dr. Smith")
  - Specialization subtitle
  - Time and current date
  - Quick filters: Today | This week | This month
```

**Alert System (Redesigned):**
```
Alert Cards at top:
  - Low stock items prominently displayed
  - Red border + background for critical
  - Yellow for warning
  - Each alert has:
    * Icon (medication bottle, syringe, etc.)
    * Item name + quantity
    * "Order more" button
    * Dismiss option
```

**Today's Appointments:**
```
Schedule timeline view:
  - Hour markers on left (8am - 6pm)
  - Appointment blocks show:
    * Time slot (colored)
    * Pet name + type icon
    * Owner name
    * Service/reason for visit
    * Vet assignment (if multiple vets)
    * Status (upcoming/in-progress/completed)
  - Colors:
    * Pending: yellow
    * In Progress: blue
    * Completed: green
    * Cancelled: gray
  - Click to expand details
  - Drag to reschedule (future)

List view alternative for mobile:
  - Vertical time blocks
  - Pet details below each time
  - Swipe actions: start visit, note, reschedule, cancel
```

**Quick Stats (Redesigned):**
```
Three stat cards below appointments:
  - Today's Appointments: number + breakdown
  - Completed Visits: counter with checkmark icon
  - Pending Items: counter with dot icon

Card interactions:
  - Click stat to filter appointments
  - Hover shows mini chart/trend
```

**Patient Search/Filter:**
```
Top bar with:
  - Search pet by name
  - Filter by species
  - Filter by owner
  - Today's view toggle
```

### 4.5 Pet/Patient Management

#### Owner Pet View:

**My Pets Page:**
```
Header with "Add New Pet" button

Pet Grid/List toggle:
  - Grid view: card layout with pet photo/badge
  - List view: table with columns (Name, Species, Age, Next Checkup, Actions)

Pet Card (grid):
  - Large colored badge area (species color)
  - Pet name (prominent)
  - "Age: X years" with small icon
  - Breed subtitle
  - Hover actions: View Details | Edit | Book Appointment | Delete
  - Last visit date (if exists)
  - Next appointment (if exists)

Pet Details Modal:
  - Full pet info in tabs:
    * Overview (photo, basic info)
    * Medical History (visits, treatments)
    * Vaccinations (calendar view)
    * Documents (if applicable)
  - Edit button in header
  - Actions dropdown
```

#### Vet Patient Search:

**Patients Page:**
```
Search/Filter bar:
  - Search by pet name or owner name
  - Filter by species (multi-select)
  - Filter by status (recent, flagged, etc.)
  - Show/hide inactive patients

Patient Cards:
  - Pet icon + name (large)
  - Owner name and contact
  - Species, breed, age
  - Last visit date
  - Next appointment
  - Status indicator (healthy/flagged/needs attention)
  - Click to expand details or navigate to visit details

Patient Grid Columns:
  - Pet Name (with icon)
  - Owner
  - Species
  - Age
  - Last Visit
  - Next Apt
  - Status
  - Actions (menu)
```

### 4.6 Visit Details Page

#### Current Issues:
- Layout could show more information at once
- Notes editing is not intuitive
- Treatment section lacks visual hierarchy
- No timeline of events

#### Redesigned Approach:

**Layout:**
```
Two-column design (responsive):
  Left (60%): Visit info and notes
  Right (40%): Pet info, vet info, timeline

Header:
  - Pet emoji/icon + name
  - Visit date and time
  - Service type badge
  - Status badge
  - Action menu (reschedule, cancel, close visit)
```

**Main Content (Left):**
```
Visit Summary Card:
  - Pet and owner info
  - Service type
  - Symptoms/reason for visit
  - Visit notes (large text area, read/edit modes)

Treatments Section:
  - Medications prescribed (table)
    * Medication name
    * Dosage
    * Frequency
    * Duration
    * Notes
  - Edit button (for vet only)
  - Add medication button (for vet only)

Recommendations Section:
  - Follow-up appointments
  - Diet changes
  - Activity restrictions
  - Other advice

Documents:
  - Lab results (if any)
  - Prescriptions
  - Export/download options
```

**Sidebar (Right):**
```
Pet Information Card:
  - Pet photo/badge
  - Name, age, breed
  - Weight
  - Allergies (highlighted)
  - Medical conditions
  - Emergency contact

Veterinarian Card:
  - Vet photo/initials avatar
  - Name
  - Specialization
  - Contact info

Visit Timeline:
  - Checked in: time
  - Exam started: time
  - Treatments applied: time
  - Checked out: time
  - Notes added: time
```

### 4.7 Health Records / Vaccination Tracker

#### Current Issues:
- Tab navigation could be clearer
- Vaccination list is too plain
- No timeline view

#### Redesigned Approach:

**Pet Selector:**
```
Horizontal scrollable pet tabs:
  - Mini pet badge with name
  - Active tab: cyan border + background
  - Shows "No records" state if no data
```

**Vaccination Section:**
```
Timeline View Option:
  - Vertical timeline
  - Past vaccinations with dates
  - Upcoming vaccinations highlighted
  - Next due date marked prominently

Table View:
  - Columns: Vaccination Name, Date Given, Next Due, Notes, Actions
  - Sortable by date
  - Color-coded: due (yellow), upcoming (blue), overdue (red), completed (green)

Add Vaccination:
  - Button with modal form
  - Autocomplete vaccine name
  - Date picker
  - Notes field
  - Save button
```

**Medical Conditions:**
```
List of known conditions:
  - Condition name
  - First diagnosed date
  - Notes
  - Current status (active/resolved)
  - Related medications
```

**Medications:**
```
Current medications table:
  - Medication name
  - Dosage
  - Frequency
  - Start date
  - End date (if applicable)
  - Refill status
  - Notes
```

### 4.8 Invoices Page

#### Current Issues:
- Filter buttons no clear state
- Invoice list is basic
- No summary information

#### Redesigned Approach:

**Header Summary:**
```
Three stat cards:
  - Total Due: Red highlighted if over due
  - Paid This Month: Green
  - Total Invoices: Neutral
```

**Filter Bar:**
```
Status filter (segmented control):
  - All | Pending | Paid | Overdue
  - Active: cyan background, white text
  - Inactive: gray background

Date range picker:
  - Quick options: Last 30 days, This month, Last 3 months, All
  - Custom range option
```

**Invoice List:**
```
Table view:
  - Columns: Invoice #, Date, Amount, Due Date, Status, Actions
  - Mobile: Card view with invoice # + amount + status prominent

Status Badge:
  - Paid: Green
  - Pending: Yellow
  - Overdue: Red

Amount highlighting:
  - Overdue: Red text
  - Due soon (< 7 days): Orange text
  - Paid: Gray text

Actions:
  - View (PDF icon)
  - Pay (CC icon)
  - Download (DL icon)
  - Email (envelope icon)
  - Print (printer icon)

Invoice Detail View:
  - Slideout panel or modal
  - Full invoice with:
    * Invoice header with logo
    * Client details
    * Itemized services
    * Total + balance
    * Payment terms
    * Notes
  - Print button
  - Download PDF button
  - Payment button (if unpaid)
```

### 4.9 Inventory Management

#### Current Issues:
- Alert system looks like errors
- No organization by category
- Stock levels unclear

#### Redesigned Approach:

**Alert Section:**
```
Prominent alert cards for low stock:
  - Icon specific to item type (bottle, syringe, etc.)
  - Item name
  - "Only X left" with progress bar
  - "Reorder" button prominent
  - Dismiss option
  - Color coding:
    * Critical (< 5%): Red
    * Warning (< 25%): Yellow
    * Low (< 50%): Orange
    * Normal: Hidden/collapsed
```

**Inventory View:**
```
View Toggle:
  - Grid view (cards)
  - Table view (detailed)
  - List view (compact)

Categories:
  - Medications
  - Vaccines
  - Supplies
  - Equipment
  - Other

Inventory Card (grid):
  - Item icon/image
  - Item name
  - Quantity + unit
  - Reorder level
  - Stock bar (visual percentage)
  - Last reordered date
  - Actions: Edit, Reorder, History

Inventory Table (detailed):
  - Columns:
    * Item Name
    * Category
    * Quantity
    * Unit
    * Reorder Level
    * Supplier
    * Last Reorder
    * Cost per Unit
    * Total Value
    * Status
  - Sortable
  - Filterable by category/status
  - Editable inline (quantity, reorder level)

Add Item:
  - Modal form with fields:
    * Item name
    * Category (dropdown)
    * Initial quantity
    * Unit
    * Reorder level
    * Supplier
    * Cost per unit
  - Submit button
  - Barcode scanner integration option

Reorder History:
  - Log of reorders
  - Dates
  - Quantities
  - Cost
  - Supplier notes
```

---

## 5. LAYOUT & NAVIGATION REDESIGN

### 5.1 Main Layout (Header + Sidebar)

#### Sidebar Redesign

**Desktop & Tablet:**
```
Width: 240px (slightly narrower for more content space)
Position: Fixed left, full viewport height
Padding: 20px

Logo Section (60px height):
  - Smaller logo (40x40)
  - App name beside (hidden on narrow)
  - Subtle color: Cyan-600

Divider: Border bottom with slate-200

User Profile Section (80px):
  - Avatar (48px) in slate-600 background
  - User name (14px, bold)
  - User role (12px, slate-500)
  - Click to open profile menu (future)

Navigation Section (flex-grow):
  - Menu items with proper spacing
  - Icon (20px) + label
  - Icons updated from emoji to professional icons
  - Item padding: 12px 16px
  - Border-left: 3px solid transparent on inactive
  - Hover: background slate-100, icon highlights
  - Active:
    * background slate-100
    * border-left cyan-500
    * text cyan-700
    * icon cyan-500
  - Smooth 150ms transition

Menu Organization:
  Owner:
    - Dashboard (home icon)
    - Book Appointment (calendar plus icon)
    - My Pets (paw icon)
    - Appointments (list icon)
    - Visit History (history/folder icon)
    - Health Records (health cross icon)
    - Invoices (document dollar icon)

  Vet:
    - Dashboard (home icon)
    - Today's Schedule (calendar check icon)
    - Patients (users icon)
    - Inventory (package icon)
    - Reports (chart icon)

Footer Section (60px):
  - Logout button
    * Icon (logout arrow)
    * "Logout" label
    * Padding: 12px 16px
    * Hover: background red-50, text red-700
    * Icon: red-500

Dark Mode:
  - Background: Slate-900
  - Text: White
  - Hover: Slate-800
  - Active border: maintained in cyan-400
```

**Mobile:**
```
Display: Hidden off-screen (-100%)
Transform animation: 300ms ease-in-out
Overlay backdrop: 0.5 opacity
Z-index: 50 (overlay), 40 (sidebar)
Width: 240px
Fixed left
Full height + safe-area-inset-bottom

Behavior:
  - Opens on hamburger click
  - Closes on item click
  - Closes on backdrop click
  - Closes on screen size change
```

#### Top Header/Navbar Redesign

**Desktop:**
```
Height: 64px
Position: Fixed top-left (lg screens offset by sidebar)
Background: White
Border-bottom: 1px slate-200
Z-index: 40 (below modals)

Left Section (flex items center):
  - Hamburger button (lg:hidden)
    * 32px square
    * Icon 20px
    * Padding 6px (for touch)
    * Border-radius 8px
    * Hover: background slate-100
    * Click toggles sidebar
  - Search bar (hidden on mobile)
    * Max-width: 300px
    * Border: 1px slate-300
    * Border-radius: 8px
    * Padding: 8px 12px
    * Icon: search (16px, gray-400)
    * Placeholder: "Search pets, appointments..."
    * Focus: border cyan-500, ring cyan-100

Right Section (flex items center, gap-3):
  - Notification button
    * Icon: bell (20px)
    * Hover: background slate-100
    * Badge: red dot (6px) if unread
    * Click opens notification drawer (future)

  - Dark mode toggle
    * Icon: sun/moon (20px)
    * Hover: background slate-100
    * Label: "Light" / "Dark" (sr-only)

  - User menu
    * Avatar (36px) with initials
    * Click opens dropdown menu:
      - Profile (gear icon)
      - Settings (settings icon)
      - Help (question icon)
      - Logout (logout icon)
    * Padding: 4px (for visual ring)
    * Hover shadow

Dark Mode:
  - Background slate-800
  - Border slate-700
  - Text white
  - Input background slate-700
```

**Mobile:**
```
Height: 56px
Left spacing for icon
Right spacing for user
Search hidden (moved to dedicated search page or modal)
Notification button present (right-aligned)
User menu simplified (avatar click)
```

#### Main Content Area

```
Padding: 24px (sm), 32px (lg)
Max-width: calc(100vw - 240px) on desktop
Flex column, flex-grow
Background: Slate-50 light mode, Slate-900 dark mode
Min-height: calc(100vh - 64px)

Page Structure:
  - Breadcrumb (if applicable)
  - Page title + actions
  - Filters (if applicable)
  - Main content
  - Footer (optional)

Page Title Style:
  - Font: 28px, 600 weight, slate-900
  - Margin-bottom: 24px
  - With optional description subtitle

Page Actions:
  - Right-aligned buttons
  - High contrast primary action
  - Secondary actions in menu
```

### 5.2 Breadcrumb Navigation (NEW)

```
Not shown on home pages
Shown on detail/settings pages

Items:
  - Home icon (link to dashboard)
  - "/"  separator
  - Current breadcrumb items with icons
  - Last item: current page (not a link)
  - Each previous: clickable link

Style:
  - Font: 12px, gray-500
  - Icons: 14px, gray-400
  - Hover: text cyan-600, underline
  - Responsive: collapse to just home + current on mobile
```

### 5.3 Responsive Behavior

**Breakpoints:**
```
Mobile: 0px - 639px (portrait phone)
  - Single column layouts
  - Sidebar hidden
  - Hamburger menu
  - Full-width cards
  - Stacked components

Tablet: 640px - 1023px
  - 2 column where possible
  - Optional sidebar (hidden)
  - Wider inputs
  - Grid layouts: 2-3 columns

Desktop: 1024px - 1279px
  - Sidebar visible
  - 3+ column layouts
  - Full width content area

Large Desktop: 1280px+
  - Maintains same width
  - More breathing room
  - Enhanced spacing
```

**Common Patterns:**
```
Mobile (1 column):
  grid-cols-1

Tablet (2 columns):
  sm:grid-cols-2

Desktop (3 columns):
  lg:grid-cols-3

4-column grid:
  md:grid-cols-2 lg:grid-cols-4

Flex direction:
  Mobile: flex-col
  Desktop: md:flex-row

Hidden elements:
  Mobile: hidden md:block (show on tablet+)
  Desktop: md:hidden (hide on tablet+)
```

---

## 6. IMPROVED USER FLOWS

### 6.1 Pet Onboarding

**Optimized Flow:**
```
1. Dashboard → "Add Pet" button
   ↓
2. Modal opens with form:
   - Pet name (required)
   - Species dropdown (required)
   - Breed (required)
   - Date of birth (required)
   - Upload photo (optional)
   - Notes (optional)
   ↓
3. Form validation:
   - Real-time validation
   - Clear error messages
   - Field hints on focus
   ↓
4. Confirmation:
   - Success toast
   - Pet appears in grid
   - Option to book appointment immediately
```

### 6.2 Appointment Booking

**Optimized Flow:**
```
1. Click "Book Appointment"
   ↓
2. Pet selection (with visual cards)
   ↓
3. Date selection (calendar picker)
   ↓
4. Time selection (visual time grid)
   ↓
5. Service selection (if multiple services)
   ↓
6. Additional notes (optional)
   ↓
7. Confirmation review:
   - All details visible
   - Edit buttons to go back
   ↓
8. Submit → Success state
   - Confirmation number
   - Add to calendar
   - Instructions email
```

### 6.3 Visit Management

**Owner perspective:**
```
Upcoming appointment
  ↓
Appointment detail view
  ↓
(After visit) Visit details page:
  - Vet notes
  - Treatments
  - Recommendations
  ↓
Save to health records
  ↓
Download invoice (if applicable)
```

**Vet perspective:**
```
Today's schedule (list/calendar)
  ↓
Click appointment → Expand details
  ↓
Start visit → Visit detail page:
  - Pet info
  - Add notes
  - Record treatments
  - Add recommendations
  ↓
Complete visit → Mark as done
  ↓
Invoice auto-generated
```

---

## 7. TECHNOLOGY STACK RECOMMENDATIONS

### 7.1 Continue/Maintain

**Frontend:**
- Vue 3.3+ (latest version)
- Vue Router 4.2+ (routing)
- Pinia 2.1+ (state management)
- Vite 4.3+ (build tool, consider upgrading to 5.x)

**Styling:**
- Tailwind CSS 3.3+ (upgrade to 4.0 when stable)
- PostCSS 8.4+
- Autoprefixer 10.4+

**Tooling:**
- TypeScript 5.0+ (strict mode)
- ESLint (modern config)
- Prettier (code formatting)

### 7.2 Recommended Additions

**Icon Library:**
```
Primary: Heroicons Vue (from Tailwind Labs)
- Official, well-maintained
- Outline and solid variants
- Perfect sizing
- TypeScript support

Install: npm install @heroicons/vue
```

**Additional UI Libraries (Consider):**
```
For advanced date picking:
- HeadlessUI (Vue) - already familiar if using Tailwind
- VueDatePicker - lightweight alternative

For charts/graphs (if needed):
- Chart.js with vue-chartjs
- ECharts Vue

For tables (if not building custom):
- TanStack Table (Vue) - formerly React Table
- Lightweight, headless
```

**Form Validation:**
```
VeeValidate (v4+) - Vue 3 compatible
- Form validation library
- Schema validation support
- Integrates well with Vue 3
- TypeScript support

OR Zod:
- TypeScript-first schema validation
- Lightweight
- Works with Vue 3

npm install vee-validate zod
```

**Accessibility:**
```
- HeadlessUI Vue - built-in a11y
- Aria & a11y best practices documents
- Testing library (vue test utils + JSDOM)
```

**Testing:**
```
Unit Tests:
- Vitest (already in dependencies)
- Vue Test Utils (already in dependencies)
- jsdom (already in dependencies)

E2E Tests:
- Playwright (already in dependencies)
- cy.intercept() for API mocking
```

### 7.3 Recommended Updates

**Upgrade Path:**
```
Immediate:
- Vue 3.4+ (latest stable)
- TypeScript 5.3+ (latest)
- Vite 5.0+ (if stable)
- Tailwind CSS 4.0+ (if stable)

Near-term (within 6 months):
- Update ESLint to v9+
- Update Prettier to v3.0+
- Update Playwright to latest

Version Management:
- Use npm outdated to check
- Update minor versions monthly
- Major versions after testing
```

### 7.4 Build & Performance

**Vite Optimizations:**
```
vite.config.js:
- Enable source maps in development only
- Optimize CSS code-splitting
- Enable Vue-specific optimizations
- Add preload hints for critical assets
```

**CSS Optimization:**
```
- Purge unused Tailwind CSS (automatic)
- Tree-shake unused CSS variables
- Minimize dual color values
- Use CSS variables for runtime theming
```

**JavaScript Optimization:**
```
- Dynamic imports for pages (already using)
- Lazy-load images
- Defer non-critical scripts
- Minify production builds
```

### 7.5 Development Setup

**Install Flow:**
```bash
# Install dependencies
npm install

# Add design system icons
npm install @heroicons/vue

# Optional form validation
npm install vee-validate zod

# Update to latest stable
npm update

# Build for production
npm run build

# Preview production build
npm run preview
```

---

## 8. IMPLEMENTATION PLAN

### Phase 1: Design System Foundation (Week 1)
- [ ] Update tailwind.config.js with new color palette
- [ ] Create/update design tokens CSS
- [ ] Update global styles (style.css)
- [ ] Create color reference documentation

### Phase 2: Component Redesign (Week 2)
- [ ] Redesign Button component (with new variants)
- [ ] Redesign Card component
- [ ] Redesign Input component
- [ ] Redesign Modal component
- [ ] Redesign Toast component
- [ ] Redesign Badge component
- [ ] Create Avatar component (new)
- [ ] Create Tabs component (new)
- [ ] Create Table component (new)

### Phase 3: Layout Refactoring (Week 3)
- [ ] Redesign MainLayout sidebar
- [ ] Redesign top navigation/header
- [ ] Add breadcrumb navigation
- [ ] Update mobile navigation
- [ ] Implement responsive behaviors

### Phase 4: Page Redesigns (Week 4-5)
- [ ] Redesign LoginPage / RegisterPage
- [ ] Redesign OwnerDashboard
- [ ] Redesign BookingPage
- [ ] Redesign VetDashboard
- [ ] Redesign VisitDetails
- [ ] Redesign HealthRecords
- [ ] Redesign InvoicesPage
- [ ] Redesign InventoryPage

### Phase 5: Feature Pages (Week 6)
- [ ] Build My Pets page (new dedicated page)
- [ ] Build Patients page (redesigned)
- [ ] Build Visit History (redesigned)
- [ ] Implement Pet detail page (expandable modals → dedicated page)

### Phase 6: Testing & Refinement (Week 7)
- [ ] Accessibility testing (keyboard nav, screen reader)
- [ ] Cross-browser testing
- [ ] Mobile responsiveness testing
- [ ] Dark mode verification
- [ ] Performance testing
- [ ] User acceptance testing

### Phase 7: Refinement & Deployment (Week 8)
- [ ] Bug fixes
- [ ] Final styling adjustments
- [ ] Documentation updates
- [ ] Deployment

---

## 9. DESIGN DOCUMENTATION

### 9.1 Color Usage Guidelines

```
Primary (Cyan-500 #0ea5e9):
  - Primary buttons
  - Active navigation items
  - Links
  - Focus rings
  - Primary CTAs
  - Active badges

Secondary (Teal-500 #14b8a6):
  - Secondary buttons
  - Accent elements
  - Highlights
  - Supporting CTAs

Success (Green-500 #22c55e):
  - Positive status badges
  - Confirmation messages
  - Completed states
  - Check icons
  - Success toasts

Warning (Orange-500 #f59e0b):
  - Alert messages
  - Warning badges
  - Attention items
  - Low stock warnings
  - Warning toasts

Danger (Red-600 #dc2626):
  - Error states
  - Delete actions
  - Danger badges
  - Error messages
  - Error toasts
  - Overdue items

Info (Blue-500 #0ea5e9):
  - Information messages
  - Info badges
  - Informational toasts
  - Help text

Background:
  Light: White/Slate-50
  Dark mode: Slate-900/Slate-800
```

### 9.2 Typography Hierarchy

```
Hero/Display:        3.5rem (56px), 700 weight
Page Title:          2.25rem (36px), 600 weight
Section Title:       1.875rem (30px), 600 weight
Card Header:         1.5rem (24px), 600 weight
Component Title:     1.25rem (20px), 600 weight
Body Large:          1.125rem (18px), 400/500 weight
Body Regular:        1rem (16px), 400 weight
Body Small:          0.875rem (14px), 400 weight
Label/Badge:         0.75rem (12px), 500 weight
Tiny:                0.625rem (10px), 600 weight
```

### 9.3 Component States

**All interactive components support:**
- Default state
- Hover state
- Active/selected state
- Focused state (keyboard)
- Disabled state
- Loading state (where applicable)
- Error state (where applicable)
- Dark mode state

### 9.4 Accessibility Standards

**Target Compliance:** WCAG 2.1 AA

**Requirements:**
- Color contrast ratio: 4.5:1 for normal text, 3:1 for large text
- Focus indicators: Always visible
- Keyboard navigation: All interactive elements accessible
- Semantic HTML: Proper roles, labels, and attributes
- Screen reader support: Proper ARIA labels
- Mobile touch targets: Minimum 48x48px
- No keyboard traps
- Logical tab order

---

## 10. BREAKING CHANGES & MIGRATION NOTES

### 10.1 Component API Changes

**Button Component:**
```
Old: variant="primary|secondary|danger|outline|ghost"
New: variant="primary|secondary|danger|success|outline|ghost"
Action: Update all button variants to match new design

Old: size="sm|md|lg"
New: size="sm|md|lg" (padding values change)
Action: No API change, but layout may adjust slightly
```

**Card Component:**
```
Old: shadow="sm|md|lg|xl"
New: shadow applied automatically, no prop needed
Action: Remove shadow prop if used
```

**Badge Component:**
```
Old: variant="default|primary|success|warning|danger|info"
New: variant="default|primary|success|warning|danger|info" (colors updated)
Action: Check all badge renders, colors may appear different

Old: dot prop for dot indicator
New: dot prop maintained
Action: No change to API
```

### 10.2 Icon Changes

```
From: Emoji (🏥, 🐾, 📅, etc.)
To: Heroicons (professional SVG icons)

Action: Install @heroicons/vue
Replace all inline SVG/emoji with proper icon components

Example:
Old: <span class="mr-2">🏥</span> Dashboard
New: <HeartIcon class="w-5 h-5 mr-2" /> Dashboard
```

### 10.3 Color Token Changes

```
Old Global Color: #2563eb (blue-600)
New Global Color: #0ea5e9 (cyan-500)

Action:
- Update all manual color references
- Update custom CSS that references old blue
- Re-theme any custom colors to match new palette
```

### 10.4 CSS Class Removals

```
Removed/Refactored:
- `.shadow-card` → Use tailwind shadow classes directly
- `.shadow-card-hover` → Use hover:shadow-lg
- Status badge classes → Refactor to badge component
- `.skeleton-*` classes → May be combined or refactored

Action:
- Replace removed classes with Tailwind alternatives
- Update component styling
- Verify no broken styling
```

---

## 11. DESIGN HAND-OFF DOCUMENTATION

### 11.1 Component Stories (Storybook-style examples)

Each component should document:
- Default state
- All variants
- Interactive states
- Dark mode
- Responsive behavior
- Accessibility features
- Code example

### 11.2 Color Palette Reference

Complete color palette with:
- Hex values
- RGB values
- CSS variable names
- Usage guidelines
- Accessibility info

### 11.3 Typography Rules

- Font family stack
- Font sizes and line heights
- Font weights and usage
- Letter spacing (if needed)
- Line length recommendations

### 11.4 Pattern Library

Common patterns documented:
- Form layouts
- Card layouts
- List layouts
- Navigation patterns
- Modal patterns
- Error/success states

---

## 12. SUCCESS METRICS

The redesign is successful when:

1. **Aesthetic Improvements:**
   - ✓ Modern, clean visual design
   - ✓ Professional color palette
   - ✓ Consistent typography
   - ✓ Improved visual hierarchy

2. **UX Improvements:**
   - ✓ Better information architecture
   - ✓ Clearer user flows
   - ✓ Enhanced interaction feedback
   - ✓ Improved empty/loading states

3. **Functionality:**
   - ✓ All existing features working
   - ✓ No functionality removed
   - ✓ Enhanced features (optional)
   - ✓ Backward compatible (where possible)

4. **Accessibility:**
   - ✓ WCAG 2.1 AA compliant
   - ✓ Keyboard navigation
   - ✓ Screen reader support
   - ✓ Dark mode working

5. **Performance:**
   - ✓ No performance regression
   - ✓ CSS bundle size maintained
   - ✓ Page load times acceptable
   - ✓ Smooth animations (60 FPS)

6. **Responsiveness:**
   - ✓ Works on all devices
   - ✓ Touch-friendly on mobile
   - ✓ Tablet optimized
   - ✓ Desktop full-featured

---

## 13. APPENDICES

### Appendix A: Color Palette Swatches

[See section 2.2 for complete color palette]

### Appendix B: Typography Scale

[See section 2.3 for complete typography system]

### Appendix C: Component Specifications

[See section 3 for detailed component specs]

### Appendix D: Page Designs

[See section 4 for detailed page designs]

---

**End of Specification**

Document prepared for Pet Clinic UI Modernization Project
Version: 1.0 | Date: March 2026

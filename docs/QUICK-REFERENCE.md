# Pet Clinic UI Redesign - Quick Reference Guide

## Color Palette at a Glance

### Primary Colors
```
Old Primary:  #2563eb (Blue-600) - Generic, dated
New Primary:  #0ea5e9 (Cyan-500) - Modern, professional
Associated:   Cyan-600 (#0284c7) for hover
             Cyan-700 (#0369a1) for darker states
```

### Semantic Colors
```
Success:   #22c55e (Green-500)  - Checkmarks, positive feedback
Warning:   #f59e0b (Orange-500) - Alerts, low stock
Danger:    #dc2626 (Red-600)    - Errors, delete actions
Info:      #0ea5e9 (Cyan-500)   - Information, help text
Secondary: #14b8a6 (Teal-500)   - Accents, secondary actions
```

### Neutral Colors (Slate)
```
50:   #f8fafc (lightest background)
100:  #f1f5f9 (light background/hover)
200:  #e2e8f0 (borders, dividers)
300:  #cbd5e1 (input borders, secondary dividers)
400:  #94a3b8 (secondary text, icons)
500:  #64748b (secondary text)
600:  #475569 (primary text, labels)
700:  #334155 (headings, strong text)
800:  #1e293b (dark backgrounds)
900:  #0f172a (darkest backgrounds)
950:  #020617 (pure black alternative)
```

## Component Quick Reference

### Button Component
```vue
<!-- Primary (uses new cyan) -->
<Button variant="primary" size="md">Save</Button>

<!-- Secondary (outline) -->
<Button variant="secondary" size="md">Cancel</Button>

<!-- Danger (for delete actions) -->
<Button variant="danger" size="sm">Delete</Button>

<!-- Disabled -->
<Button variant="primary" disabled>Disabled</Button>

<!-- With Icon -->
<Button variant="primary">
  <PlusIcon class="w-4 h-4 mr-2" />
  Add Pet
</Button>

<!-- Loading -->
<Button variant="primary" :disabled="loading">
  {{ loading ? 'Saving...' : 'Save' }}
</Button>
```

### Card Component
```vue
<Card>
  <template #header>
    <h2>Card Title</h2>
  </template>

  <p>Card content appears here</p>

  <template #footer>
    <Button variant="primary">Action</Button>
  </template>
</Card>
```

### Input Component
```vue
<Input
  v-model="email"
  label="Email Address"
  type="email"
  placeholder="you@example.com"
  :error="errors.email"
/>
```

### Badge Component
```vue
<!-- Status badges -->
<Badge variant="success">Completed</Badge>
<Badge variant="warning">Pending</Badge>
<Badge variant="danger">Overdue</Badge>

<!-- With dot indicator -->
<Badge variant="success" dot>Online</Badge>
```

### Modal Component
```vue
<Modal :is-open="showModal" title="Add Pet" @close="showModal = false">
  <form @submit.prevent="savePet">
    <Input v-model="pet.name" label="Pet Name" required />
  </form>

  <template #footer>
    <Button variant="secondary" @click="showModal = false">Cancel</Button>
    <Button variant="primary" @click="savePet">Add Pet</Button>
  </template>
</Modal>
```

### Avatar Component (NEW)
```vue
<!-- With initials -->
<Avatar initials="JD" size="md" />

<!-- With image -->
<Avatar src="/user-photo.jpg" size="lg" />

<!-- With status indicator -->
<Avatar initials="JD" size="md" status="online" />
```

## Icon System (Heroicons)

### Import Pattern
```vue
<script setup>
import { HomeIcon, PlusIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { CheckIcon } from '@heroicons/vue/24/solid'
</script>

<template>
  <HomeIcon class="w-5 h-5" />
</template>
```

### Common Replacements
```
Old Emoji → New Heroicon (size/variant)

🏠 → HomeIcon (outline, 24px)
📅 → CalendarIcon (outline, 24px)
🐾 → PawIcon (outline, 24px)
📋 → ListBulletIcon (outline, 24px)
📜 → DocumentTextIcon (outline, 24px)
💳 → DocumentDollarIcon (outline, 24px)
🏥 → HeartIcon (outline, 24px)
💊 → ShoppingBagIcon (outline, 24px)
👥 → UsersIcon (outline, 24px)
➕ → PlusIcon (outline, 20px)
✏️ → PencilIcon (outline, 20px)
🗑️ → TrashIcon (outline, 20px)
🔍 → MagnifyingGlassIcon (outline, 20px)
⚙️ → Cog6ToothIcon (outline, 20px)
🚪 → ArrowRightStartOnRectangleIcon (outline, 20px)
✓  → CheckIcon (solid, 20px)
✕  → XMarkIcon (solid, 20px)
⚠️ → ExclamationTriangleIcon (solid, 20px)
ℹ️ → InformationCircleIcon (solid, 20px)
```

## Typography Scale

```
Display Large:   3.5rem (56px) 700 weight - Hero titles
Display:         2.75rem (44px) 600 weight - Page headers
H1:              2.25rem (36px) 600 weight - Section titles
H2:              1.875rem (30px) 600 weight - Card headers
H3:              1.5rem (24px) 600 weight - Component titles
H4:              1.25rem (20px) 600 weight - Subsections
Body Large:      1.125rem (18px) 400/500 - Large text
Body:            1rem (16px) 400 weight - Default (14-16px lines)
Body Small:      0.875rem (14px) 400 - Secondary text
Label:           0.75rem (12px) 500 weight - Labels, badges
Tiny:            0.625rem (10px) 600 weight - Small labels
```

## Spacing Scale

```
0px:    0
4px:    1
8px:    2
12px:   3
16px:   4
24px:   6
32px:   8
40px:   10
48px:   12
64px:   16
80px:   20
96px:   24
```

**Usage:**
```html
<!-- Standard card padding -->
<div class="p-6"><!-- 24px padding --></div>

<!-- Gap between items -->
<div class="gap-4"><!-- 16px gap --></div>

<!-- Margin spacing -->
<div class="mt-8 mb-6"><!-- 32px top, 24px bottom --></div>
```

## Shadow System

```
No Shadow:     No box-shadow (backgrounds, base)
Subtle:        0 1px 2px rgba(0,0,0,0.05) (inputs, subtle cards)
Card:          0 4px 6px -1px rgba(0,0,0,0.1) [DEFAULT] (most cards)
Hover:         0 10px 15px -3px rgba(0,0,0,0.1) (hovered cards)
Modal:         0 20px 25px -5px rgba(0,0,0,0.1) (modals, dropdowns)
```

## Border Radius

```
sm:   4px   (form inputs, small elements)
md:   8px   (DEFAULT, most components)
lg:   12px  (large cards, sections)
xl:   16px  (major sections, modals)
2xl:  20px  (oversized elements)
full: 9999px (circles, badges)
```

## Responsive Grid Patterns

```vue
<!-- 1 column mobile, 2 columns tablet, 3 columns desktop -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
  <!-- items -->
</div>

<!-- Flex column mobile, row on desktop -->
<div class="flex flex-col md:flex-row gap-4">
  <!-- items -->
</div>

<!-- Hidden on mobile, shown on desktop -->
<div class="hidden md:block">
  <!-- Desktop-only content -->
</div>

<!-- Shown on mobile, hidden on desktop -->
<div class="md:hidden">
  <!-- Mobile-only content -->
</div>
```

## Utility Classes

```
<!-- No select (interactive elements) -->
<div class="no-select">

<!-- Thin scrollbar -->
<div class="scrollbar-thin max-h-96 overflow-y-auto">

<!-- Container with responsive padding -->
<div class="container-main">

<!-- Status badge classes (maintain for compatibility) -->
<div class="status-completed"> <!-- green -->
<div class="status-pending">   <!-- yellow -->
<div class="status-in-progress"> <!-- purple -->
<div class="status-cancelled">  <!-- red -->
```

## Form Input Pattern

```vue
<template>
  <div class="space-y-4">
    <Input
      v-model="form.field"
      label="Label"
      type="text"
      placeholder="Placeholder text"
      :error="errors.field"
    />

    <div class="space-y-2">
      <label class="block text-sm font-medium text-slate-700">
        Select Field
      </label>
      <select
        v-model="form.select"
        class="w-full px-3 py-2 border border-slate-300 rounded-md bg-white text-slate-900 focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent"
      >
        <option value="">Select option</option>
        <option value="1">Option 1</option>
      </select>
    </div>
  </div>
</template>
```

## Dark Mode

```vue
<!-- Always use dark: prefix for dark mode support -->
<div class="text-slate-900 dark:text-white">
  <!-- text will be dark on light mode, white on dark mode -->
</div>

<div class="bg-white dark:bg-slate-800">
  <!-- white background on light, dark slate on dark -->
</div>

<div class="border border-slate-200 dark:border-slate-700">
  <!-- light border on light mode, dark border on dark -->
</div>
```

## Common Implementation Tasks

### Add a New Page
1. Create file in `src/pages/NewPage.vue`
2. Add route in `src/router/index.ts`
3. Create corresponding menu item in MainLayout
4. Import and use new UI components
5. Use proper layout structure:
   ```vue
   <template>
     <div class="space-y-6">
       <!-- Breadcrumb -->

       <!-- Page Title -->
       <div class="flex justify-between items-center">
         <h1 class="text-3xl font-bold text-slate-900">Page Title</h1>
         <Button variant="primary">Action</Button>
       </div>

       <!-- Filters/Search -->

       <!-- Main Content -->
       <Card>
         <!-- Content -->
       </Card>
     </div>
   </template>
   ```

### Create a Modal Form
```vue
<Modal :is-open="showModal" title="Add Item" @close="closeModal">
  <form @submit.prevent="submitForm" class="space-y-4">
    <Input v-model="form.name" label="Name" required :error="errors.name" />
    <Input v-model="form.email" label="Email" type="email" :error="errors.email" />
    <!-- More inputs -->
  </form>

  <template #footer>
    <div class="flex justify-end gap-3">
      <Button variant="secondary" @click="closeModal">Cancel</Button>
      <Button variant="primary" @click="submitForm" :disabled="submitting">
        {{ submitting ? 'Saving...' : 'Save' }}
      </Button>
    </div>
  </template>
</Modal>
```

### Create a Status Badge
```vue
<!-- In data -->
const statusColors = {
  'Completed': 'success',
  'Pending': 'warning',
  'Cancelled': 'danger',
  'In Progress': 'info'
}

<!-- In template -->
<Badge :variant="statusColors[item.status]">
  {{ item.status }}
</Badge>
```

## Current Route Snapshot (March 2026)

| Path | Component | Role |
|------|-----------|------|
| `/login` | LoginPage | Any |
| `/register` | RegisterPage | Any |
| `/owner` | OwnerDashboard | Owner |
| `/owner/pets` | MyPetsPage | Owner |
| `/owner/appointments` | OwnerDashboard | Owner |
| `/vet` | VetDashboard | Vet |
| `/vet/appointments` | VetTodayAppointments | Vet |
| `/vet/inventory` | InventoryPage | Vet |
| `/vet/patients` | PatientsPage | Vet |

## Inventory API Snapshot

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/v1/inventory/{id}` | `PUT` | Update medication fields (e.g., name, unitPrice) |
| `/api/v1/inventory/incoming` | `GET` | Pending reorder deliveries |
| `/api/v1/inventory/delivered` | `GET` | Recently delivered reorder history |

Frontend service methods:
- `updateInventoryItem(id, payload)`
- `getIncomingReorders()`
- `getDeliveredReorders()`

## Auth Persistence Snapshot

- Access token is persisted in local storage (`petclinic.accessToken`).
- App bootstrap runs auth initialization before router mount.
- Router preserves protected target via `/login?redirect=...`.
- Authenticated users are redirected away from `/login` and `/register`.

## Quick Debug Checklist

- [ ] Colors match specification (check tokens.css)
- [ ] Icons display correctly (check @heroicons/vue import)
- [ ] Dark mode works (toggle in navbar, check dark: variants)
- [ ] Responsive works (test at 375px, 768px, 1024px)
- [ ] Focus ring visible (tab key navigation)
- [ ] Spacing consistent (check padding/margin values)
- [ ] Text readable (contrast ratio >= 4.5:1)
- [ ] Mobile touch targets 48px+ (buttons, links)
- [ ] No console errors or warnings
- [ ] Page loads in < 3 seconds

---

**Last Updated:** March 2026
**Status:** Living reference for active implementation

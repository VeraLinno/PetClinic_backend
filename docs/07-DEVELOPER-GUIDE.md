# Pet Clinic - Quick Reference Developer Guide

## Project Structure at a Glance

```
velinn-petclinic_js/
├── index.html                    # Entry HTML (add fonts here)
├── vite.config.ts                # Vite config + API proxy
├── tailwind.config.js            # Tailwind theme + colors
├── package.json                  # Dependencies
├── src/
│   ├── main.js                   # App bootstrap
│   ├── App.vue                   # Root component
│   ├── style.css                 # Global styles (needs @tailwind directives!)
│   ├── styles/
│   │   ├── tokens.css            # Design tokens (CSS variables)
│   │   └── reset.css             # CSS normalize
│   ├── router/index.ts           # Vue Router config (13 routes)
│   ├── stores/auth.ts            # Pinia auth store
│   ├── services/
│   │   ├── api.ts                # Axios instance + interceptors
│   │   ├── auth.ts               # Auth API methods
│   │   ├── appointments.ts       # Appointments/visits/invoices API
│   │   └── owners.ts             # Owner/pet API methods
│   ├── utils/jwt.ts              # JWT decoder
│   ├── layouts/
│   │   └── MainLayout.vue        # Sidebar + header layout
│   ├── components/
│   │   ├── ui/                   # Reusable UI primitives
│   │   │   ├── Button.vue
│   │   │   ├── Card.vue
│   │   │   ├── Input.vue
│   │   │   ├── Modal.vue
│   │   │   ├── Badge.vue
│   │   │   ├── Toast.vue
│   │   │   ├── Avatar.vue        # Created
│   │   │   ├── Tabs.vue          # Created
│   │   │   └── Table.vue         # Created
│   │   ├── AppointmentList.vue   # Appointment list feature component
│   │   ├── PetCard.vue           # Pet card feature component
│   │   └── ProtectedRoute.vue    # Dead code — can delete
│   └── pages/                    # Page-level components
│       ├── LoginPage.vue
│       ├── RegisterPage.vue      # Route active: /register
│       ├── OwnerDashboard.vue
│       ├── BookingPage.vue
│       ├── VetDashboard.vue
│       ├── VetTodayAppointments.vue
│       ├── VisitDetails.vue
│       ├── VisitHistory.vue      # API-integrated
│       ├── HealthRecords.vue     # API-integrated
│       ├── InvoicesPage.vue      # API-integrated
│       ├── InventoryPage.vue     # API-integrated (incoming/delivered reorder data)
│       ├── PatientsPage.vue      # API-integrated
│       └── MyPetsPage.vue        # Route active: /owner/pets
```

---

## Quick Commands

| Action | Command |
|--------|---------|
| Start dev server | `npm run dev` |
| Build for production | `npm run build` |
| Preview production build | `npm run preview` |
| Install Heroicons | `npm install @heroicons/vue@latest` |
| Install form validation | `npm install vee-validate@latest zod@latest` |
| Type check | `npx vue-tsc --noEmit` |

---

## Common Patterns

### Creating a New Page

1. Create file: `src/pages/MyNewPage.vue`
```vue
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import Button from '@/components/ui/Button.vue'

const loading = ref(true)
const data = ref([])

onMounted(async () => {
  try {
    // Fetch data
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="space-y-6">
    <!-- Page header -->
    <div class="flex items-center justify-between">
      <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Page Title</h1>
      <Button variant="primary">
        <PlusIcon class="w-5 h-5 mr-2" />
        Add Item
      </Button>
    </div>

    <!-- Content -->
    <Card>
      <div v-if="loading" class="animate-pulse space-y-3">
        <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-3/4"></div>
        <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-1/2"></div>
      </div>
      <div v-else>
        <!-- Your content -->
      </div>
    </Card>
  </div>
</template>
```

2. Add route in `src/router/index.ts`:
```typescript
{
  path: '/my-page',
  name: 'MyPage',
  component: () => import('@/pages/MyNewPage.vue'),
  meta: { requiresAuth: true, roles: ['Owner'] }
}
```

3. Add sidebar link in `src/layouts/MainLayout.vue`

### Making an API Call

```typescript
import { appointmentsService } from '@/services/appointments'
import { ownersService } from '@/services/owners'

// Get data
const appointments = await appointmentsService.getAppointments()
const pets = await ownersService.getPets()
const owner = await ownersService.getMe()

// Create data
const newPet = await ownersService.createPet({
  name: 'Buddy', species: 'Dog', breed: 'Labrador', dateOfBirth: '2022-01-01'
})

// Update data
await appointmentsService.updateAppointment(id, { notes: 'Updated' })

// Delete data
await ownersService.deletePet(petId)
```

### Adding an Icon

```vue
<script setup>
// 1. Import the specific icon you need
import { HomeIcon } from '@heroicons/vue/24/outline'     // Outline 24px
import { CheckIcon } from '@heroicons/vue/24/solid'       // Solid 24px
import { PlusIcon } from '@heroicons/vue/20/solid'        // Mini 20px
</script>

<template>
  <!-- 2. Use with standard sizing classes -->
  <HomeIcon class="w-6 h-6" />           <!-- Navigation (24px) -->
  <PlusIcon class="w-5 h-5" />           <!-- Buttons (20px) -->
  <CheckIcon class="w-4 h-4" />          <!-- Inline (16px) -->

  <!-- 3. With color -->
  <HomeIcon class="w-6 h-6 text-cyan-500" />
  <CheckIcon class="w-5 h-5 text-green-500" />
</template>
```

### Form with Validation

```vue
<script setup lang="ts">
import { ref } from 'vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const form = ref({ name: '', email: '' })
const errors = ref<Record<string, string>>({})
const submitting = ref(false)

const validate = () => {
  errors.value = {}
  if (!form.value.name.trim()) errors.value.name = 'Name is required'
  if (!form.value.email.trim()) errors.value.email = 'Email is required'
  return Object.keys(errors.value).length === 0
}

const submit = async () => {
  if (!validate()) return
  submitting.value = true
  try {
    // API call
  } catch (err) {
    errors.value.general = 'Something went wrong'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <form @submit.prevent="submit" class="space-y-4">
    <Input v-model="form.name" label="Name" :error="errors.name" />
    <Input v-model="form.email" label="Email" type="email" :error="errors.email" />
    <p v-if="errors.general" class="text-sm text-red-600">{{ errors.general }}</p>
    <Button variant="primary" type="submit" :disabled="submitting">
      {{ submitting ? 'Saving...' : 'Save' }}
    </Button>
  </form>
</template>
```

### Responsive Card Grid

```html
<!-- 1 col → 2 col → 3 col -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
  <Card v-for="item in items" :key="item.id">
    {{ item.name }}
  </Card>
</div>
```

### Skeleton Loading State

```html
<!-- Skeleton for a card -->
<div class="animate-pulse space-y-3 p-6">
  <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-3/4"></div>
  <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-1/2"></div>
  <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-2/3"></div>
</div>

<!-- Skeleton for a table row -->
<tr class="animate-pulse">
  <td class="px-4 py-3"><div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-24"></div></td>
  <td class="px-4 py-3"><div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-32"></div></td>
  <td class="px-4 py-3"><div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-16"></div></td>
</tr>
```

### Empty State

```html
<div class="text-center py-12">
  <InboxIcon class="w-12 h-12 text-slate-300 dark:text-slate-600 mx-auto mb-4" />
  <h3 class="text-lg font-medium text-slate-600 dark:text-slate-400">No items found</h3>
  <p class="text-sm text-slate-500 dark:text-slate-500 mt-1">
    Get started by creating a new item.
  </p>
  <Button variant="primary" class="mt-4">
    <PlusIcon class="w-5 h-5 mr-2" />
    Create Item
  </Button>
</div>
```

---

## Routing Cheat Sheet

| Path | Component | Role | Purpose |
|------|-----------|------|---------|
| `/login` | LoginPage | Any | Authentication |
| `/register` | RegisterPage | Any | User account creation |
| `/owner` | OwnerDashboard | Owner | Owner home |
| `/booking` | BookingPage | Owner | Appointment wizard |
| `/visit/:id` | VisitDetails | Both | Visit details |
| `/owner/history` | VisitHistory | Owner | Past visits |
| `/owner/invoices` | InvoicesPage | Owner | Invoice list |
| `/owner/health` | HealthRecords | Owner | Pet health |
| `/owner/pets` | MyPetsPage | Owner | Dedicated pet manager |
| `/owner/appointments` | OwnerDashboard | Owner | **Duplicate — needs own page** |
| `/vet` | VetDashboard | Vet | Vet home |
| `/vet/appointments` | VetTodayAppointments | Vet | Today's appointments |
| `/vet/inventory` | InventoryPage | Vet | Stock management |
| `/vet/patients` | PatientsPage | Vet | Patient list |

---

## Role-Based Access

```
Owner can access:
  /owner, /booking, /visit/:id, /owner/history, /owner/invoices,
  /owner/health, /owner/pets, /owner/appointments

Vet can access:
  /vet, /visit/:id, /vet/appointments, /vet/inventory, /vet/patients

Both roles:
  /visit/:id
```

---

## State Management (Pinia)

```typescript
// Access auth store anywhere
import { useAuthStore } from '@/stores/auth'
const authStore = useAuthStore()

// Check authentication
if (authStore.isAuthenticated) { ... }

// Get user info
const roles = authStore.roles          // ['Owner'] or ['Vet']
const email = authStore.user?.email
const userId = authStore.user?.id

// Login
await authStore.login(email, password)

// Logout
authStore.logout()
```

---

## Keyboard Shortcuts (Browser DevTools)

| Shortcut | Action |
|----------|--------|
| `F12` | Open DevTools |
| `Ctrl+Shift+M` | Toggle responsive mode |
| `Ctrl+Shift+C` | Inspect element |
| `Ctrl+Shift+R` | Hard refresh (clear cache) |
| `Ctrl+Shift+J` | Open Console directly |
| `Ctrl+P` (in DevTools) | Search files in Sources panel |
| `Ctrl+Shift+P` (in DevTools) | Run command (e.g., "screenshot", "coverage") |

### Vue DevTools

| Panel | Use |
|-------|-----|
| Components | Inspect component tree, props, state |
| Pinia | View/edit store state (auth token, user) |
| Routes | See current route, params, meta |
| Timeline | Track events, mutations, renders |

---

## Color Quick Reference

```
Primary:   cyan-500  (#0ea5e9)   — Buttons, links, active states
Secondary: teal-500  (#14b8a6)   — Accents, secondary actions
Success:   green-500 (#22c55e)   — Checkmarks, completed
Warning:   orange-500 (#f59e0b)  — Alerts, pending
Danger:    red-600   (#dc2626)   — Errors, delete, overdue
Text:      slate-900 → slate-400  — Headings to helper text
Borders:   slate-200 (light)      — slate-700 (dark mode)
Page bg:   slate-50 (light)       — slate-900 (dark mode)
Card bg:   white (light)          — slate-800 (dark mode)
```

---

## Dark Mode Classes Cheat Sheet

```html
text:       text-slate-900 dark:text-white
text-body:  text-slate-600 dark:text-slate-400
text-muted: text-slate-400 dark:text-slate-500
bg-page:    bg-slate-50 dark:bg-slate-900
bg-card:    bg-white dark:bg-slate-800
bg-hover:   hover:bg-slate-100 dark:hover:bg-slate-700
border:     border-slate-200 dark:border-slate-700
input:      bg-white dark:bg-slate-800 border-slate-300 dark:border-slate-600
```

---

## Productivity Tips

1. **Use Tailwind IntelliSense** — Type `bg-` and get autocomplete with color preview
2. **Use Vue DevTools** — Edit Pinia state live to test different scenarios
3. **Test mobile first** — Start with `Ctrl+Shift+M` in Chrome, design at 375px, then expand
4. **Commit per milestone** — Don't batch large changes; commit after each component/page
5. **Dark mode test frequently** — Toggle dark mode after each change, not at the end
6. **Use the Quick Debug Checklist** (in Troubleshooting guide) when something looks wrong
7. **Keep the API docs open** — Reference `docs/02-API-DOCUMENTATION.md` for endpoint details
8. **Check the design system** — Reference `docs/03-DESIGN-SYSTEM-REFERENCE.md` for exact colors/spacing

---

## Quick Copy-Paste Snippets

### Standard Page Wrapper
```html
<div class="space-y-6">
  <div class="flex items-center justify-between">
    <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Title</h1>
  </div>
  <!-- content -->
</div>
```

### Card with Header
```html
<div class="bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl shadow">
  <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
    <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Section</h2>
  </div>
  <div class="p-6">Content</div>
</div>
```

### Stats Card
```html
<div class="bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl p-6">
  <div class="flex items-center gap-4">
    <div class="p-3 bg-cyan-50 dark:bg-cyan-900/20 rounded-lg">
      <CalendarIcon class="w-6 h-6 text-cyan-500" />
    </div>
    <div>
      <p class="text-sm text-slate-500 dark:text-slate-400">Appointments</p>
      <p class="text-2xl font-bold text-slate-900 dark:text-white">12</p>
    </div>
  </div>
</div>
```

### Search + Filter Bar
```html
<div class="flex flex-col sm:flex-row gap-3">
  <div class="relative flex-1">
    <MagnifyingGlassIcon class="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
    <input
      v-model="search"
      placeholder="Search..."
      class="w-full pl-10 pr-4 py-2.5 rounded-lg border border-slate-300 dark:border-slate-600
             bg-white dark:bg-slate-800 text-slate-900 dark:text-white
             focus:outline-none focus:ring-2 focus:ring-cyan-500"
    />
  </div>
  <select class="px-3 py-2.5 rounded-lg border border-slate-300 dark:border-slate-600
                 bg-white dark:bg-slate-800 text-slate-900 dark:text-white">
    <option value="">All Status</option>
    <option value="Pending">Pending</option>
    <option value="Confirmed">Confirmed</option>
  </select>
</div>
```

---

**Last Updated:** March 2026
**Status:** Ready for use during implementation

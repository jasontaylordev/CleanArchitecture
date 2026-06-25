## Tech Stack

| Layer | Technology |
|---|---|
| Framework | Angular 21, standalone components only (no NgModules) |
| State | NgRx Signals (`signalStore`, `signalStoreFeature`) |
| Styling | Tailwind CSS + component-scoped SCSS |
| REST API | auto-generated — never edit |
| State mutations | Immer via `produceState()` helper |
| i18n | ngx-translate, files in i18n |

---

## Feature Layout

```
featureName/
  pages/              # Routed top-level components (one per route)
  components/         # Non-routed components
  dialogs/            # Kendo dialog components
  *.store.ts          # Feature-level signal store

_shared/
  components/         # Shared components across features
  dialogs/            # Shared dialog components
  guards/             # Feature-specific route guards
  helpers/            # Shared utility functions
  pipes/              # Feature-scoped pipes
  services/           # Shared services (e.g., SignalR, API)
  stores/             # signal store features (no global store)
```

---

## State Management Rules

- All state via `signalStore()` — never RxJS subjects for state
- Global stores: `providedIn: 'root'` in `shared/stores/`
- Mutate state with `produceState(store, draft => { ... })` (Immer wrapper)
- Async backend data uses `BackendStoreState<T>` shape (`loading`, `data`, `error`)
- Compose with `signalStoreFeature` — co-locate `.store-feature.ts` next to the component

---

## Key Conventions

- **Component selector prefix**: `rst-`
- **Path alias**: `@/*` → `src/app/*` — use for cross-feature imports
- All routes are **lazy-loaded** (`loadComponent`)
- Every store should include `withTestHelper()` and `withDevReloadHelper()` for environment swap support
- HTTP requests automatically get `x-rst-hub-connection-id` header via interceptor
- SignalR DTOs in `shared/services/signalr/` are auto-generated — never modify

---

## Dual-Implementation Pattern

Several files have environment-specific variants (swapped at build time): `*.playwright.ts` for tests, `.development.ts` for dev, and the base file is the production no-op. This applies to `rst-hub.provider`, `config.service`, `test-helper.feature`, and `dev-reload-helper.feature`.

# CursosAPI — Frontend

SPA en **React + Vite** para el TP de Programación 4 (UTN San Nicolás), consumiendo el backend [CursosAPI](../Backend).

## Stack

- **React 18** + **Vite** + **JavaScript (JSX)**
- **wouter** — ruteo (`/`, `/login`, `/cursos`, `/cursos/:id`, `/admin`)
- **axios** — cliente HTTP (`withCredentials: true`, ver nota de Auth)
- **zustand** — estado global de sesión (`src/store/authStore.js`)
- **zod + react-hook-form** — validación de formularios (login, registro, alta/edición de cursos)
- **React.lazy + Suspense** — carga perezosa de `CursoDetailPage` y `AdminPage`
- **recharts** — gráficos en el panel admin
- **lucide-react** — íconos
- **Tailwind CSS v4** — estilos

## Instalación

```bash
npm install
```

Creá un archivo `.env` en la raíz de `Frontend/` con la URL del backend:

```
VITE_API_URL=http://localhost:5000
```

(Si no se define, `src/lib/api.js` usa `https://localhost:7000` como fallback.)

```bash
npm run dev
```

La app levanta en `http://localhost:5173` (puerto fijo en `vite.config.js`, porque el backend tiene el CORS hardcodeado a ese origen).

## Scripts

| Comando | Descripción |
|---|---|
| `npm run dev` | Servidor de desarrollo (puerto 5173) |
| `npm run build` | Build de producción |
| `npm run preview` | Sirve el build localmente |

## ⚠️ Auth: el back usa cookies, no JWT

El backend implementa autenticación por **cookie httpOnly** (`AddCookie` + `SignInAsync`), no JWT. Por eso:

- No hay ningún token que guardar ni mandar en `Authorization`. El navegador maneja la cookie solo.
- Todas las llamadas de axios van con `withCredentials: true` (`src/lib/api.js`), y el back tiene `AllowCredentials()` en su CORS.
- El store de auth (`authStore.js`) persiste únicamente los datos del usuario (para evitar un parpadeo al recargar la página), y en el montaje de `App` se revalida siempre contra `GET /api/auth/me`.

**Importante para el deploy:** la cookie se configura con `SecurePolicy = Always` y `SameSite = None`. Eso significa que **el back tiene que estar servido por HTTPS** (o `localhost`, que los navegadores tratan como contexto seguro) para que la cookie se guarde y se reenvíe. Si el back se despliega en un dominio público sin HTTPS, el login "funciona" (200 OK) pero el navegador descarta la cookie silenciosamente y las siguientes peticiones autenticadas van a fallar con 401.

## Cómo pasar un usuario de Free a Premium

Es una de las funcionalidades clave del proyecto, implementada en el panel de admin (`pages/admin`):

1. El admin entra a `/admin` → pestaña de Usuarios.
2. El front trae `GET /api/roles` (para saber los `id` de `UserGratis` / `UserPremium`) y `GET /api/users`.
3. Al marcar el cambio, se llama a `PUT /api/auth/update-roles/{userId}` con `{ roleIds: [...] }`.

Ese endpoint **reemplaza** la lista completa de roles del usuario (no "agrega"), así que el front arma el array preservando cualquier otro rol que tuviera.

## Estructura

```
src/
├── components/    Navbar, CourseCard, ProtectedRoute, Toast, Loader, QuizBuilder, QuizPlayer, VideoPlayer
├── lib/           api.js (instancia de axios + parseo de errores del back), quiz.js, video.js
├── pages/         HomePage, LoginPage, CursosPage, CursoDetailPage, AdminPage
├── pages/admin/   sub-vistas del panel admin (cursos, usuarios, estadísticas)
├── schemas/       esquemas zod para validación de formularios
├── services/      llamadas a la API (auth, cursos, admin)
├── store/         authStore (zustand)
└── styles/        estilos globales (Tailwind)
```

## Notas de diseño

- Las rutas de listado/detalle se llaman `/cursos` y `/cursos/:id`, en línea con el dominio elegido ("Curso").
- Feedback visual: toasts propios (`components/Toast.jsx`), skeletons/loaders (`components/Loader.jsx`), y estados vacíos/error en cada página.
- El endpoint `GET /api/cursos/{id}` es público, pero el contenido completo (video, material, quiz) solo se sirve si el usuario tiene acceso al curso — el resto ve una vista restringida.

## Pendiente / no implementado

- Tests automatizados.
- Refresh de sesión automático (no aplica: al ser cookie de servidor, la expiración la maneja el backend).

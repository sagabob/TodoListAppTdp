# TodoListApp

A full-stack todo list application with JWT authentication. The **backend** is an ASP.NET Core REST API; the **frontend** is an Angular SPA that talks to it through a dev proxy.

## Tech stack

- .NET 10 SDK
- Node.js (npm 11+)
- Angular 21

## Quick start

Run both apps locally (start the backend first):

### Backend (API)

```bash
cd Backend/TodoListApi_Solution/TodoListApi
dotnet run --launch-profile https
```

- HTTPS: `https://localhost:7028`
- HTTP: `http://localhost:5241`
- Swagger UI (Development): `https://localhost:7028/swagger`

### Frontend (Angular)

```bash
cd Frontend/todo-web
npm install
npm start
```

- App: `http://localhost:4200`
- API calls to `/api/*` are proxied to the backend (`proxy.conf.json`)

## Applications

### Backend — `Backend/TodoListApi_Solution/TodoListApi`

ASP.NET Core Web API (.NET 10) that exposes todo CRUD and auth endpoints. Uses EF Core with an in-memory database, JWT authentication (httpOnly cookie for the SPA, Bearer token supported in Swagger), and CORS for `http://localhost:4200`.

**Explanation on technical decisions**

- Auth is intentionally simple (demo email/password from config), but follows a real SPA pattern: JWT for authenticated calls, stored in an httpOnly cookie with Secure/SameSite settings appropriate for development and production.
- MediatR with CQRS separates reads and writes into focused handlers, keeping controllers thin and making the API easier to extend (e.g. toward vertical slices per feature). The pipeline style also allows us to inject other behaviors such as validation or logging.
- The Result pattern handles expected business failures (such as not found) without exceptions; controllers map these to HTTP status codes. Unexpected errors are handled globally via ASP.NET Core exception handling and ProblemDetails for a consistent error format.

### Frontend — `Frontend/todo-web`

Angular 21 SPA with a login page, an auth guard on the home route, and core services for auth and todo CRUD. API calls use the dev proxy and send credentials via the httpOnly cookie set by the backend on login.

## Testing

Unit tests are included for both apps.

### Backend

```bash
cd Backend/TodoListApi_Solution
dotnet test
```

- **Framework:** xUnit (`TodoListApi.Tests`)
- **Coverage:** `AuthService`, `JwtTokenService`, `TodoListRepository`, `TodoItemMapper`
- **Tools:** Moq for dependencies; EF Core in-memory database for repository tests

### Frontend

```bash
cd Frontend/todo-web
npm test
```

- **Framework:** Vitest (via `ng test`)
- **Coverage:** `AuthService`, `TodoService`, `authGuard`, and components (`LoginComponent`, `HomeComponent`, `App`)
- **Approach:** dependencies are mocked; component and service behavior is tested in isolation

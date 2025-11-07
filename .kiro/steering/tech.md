# Technology Stack

## Backend (TicketSystem.Server)
- **Framework**: ASP.NET Core 8.0 (.NET 8)
- **Language**: C# with nullable reference types enabled
- **API Documentation**: Swagger/Swashbuckle (available in development mode)
- **SPA Integration**: Microsoft.AspNetCore.SpaProxy for development

## Frontend (ticketsystem.client)
- **Framework**: React 19.1.1
- **Language**: TypeScript 5.9.3 (strict mode enabled)
- **Build Tool**: Vite 7.1.7
- **Bundler**: ESNext module system
- **Linting**: ESLint 9 with TypeScript ESLint, React Hooks, and React Refresh plugins

## Development Configuration
- **TypeScript**: Strict mode with unused locals/parameters checking
- **Module Resolution**: Bundler mode with path alias `@` pointing to `src`
- **JSX**: react-jsx transform
- **HTTPS**: Development certificates managed via dotnet dev-certs

## Common Commands

### Backend
```bash
# Build the solution
dotnet build

# Run the server (includes SPA proxy)
dotnet run --project TicketSystem.Server

# Restore packages
dotnet restore
```

### Frontend
```bash
# Install dependencies
npm install

# Run development server (port 51379)
npm run dev

# Build for production
npm run build

# Lint code
npm run lint

# Preview production build
npm run preview
```

### Full Stack
The backend automatically proxies to the frontend during development. Running `dotnet run` from the server project will start both the API and the Vite dev server.

## Port Configuration
- **Frontend Dev Server**: 51379 (HTTPS)
- **Backend API**: 7248 (HTTPS, default)
- API calls from frontend are proxied through Vite (e.g., `/weatherforecast`)

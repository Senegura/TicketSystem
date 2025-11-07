# Project Structure

## Solution Organization
The project uses a Visual Studio solution (`.sln`) with a layered architecture:

```
TicketSystem/
├── TicketSystem.Server/          # ASP.NET Core backend (API layer)
├── TicketSystem.BL/              # Business Logic layer
├── TicketSystem.DAL/             # Data Access Layer
├── TicketSystem.Models/          # Domain models and DTOs
├── ticketsystem.client/          # React frontend
└── TicketSystem.sln              # Solution file
```

## Architectural Layers
- **TicketSystem.Server**: API controllers and HTTP endpoints only
- **TicketSystem.BL**: All business logic implementation
- **TicketSystem.DAL**: All data access logic (repositories, database operations)
- **TicketSystem.Models**: Domain models, entities, and data transfer objects

## Backend Structure (TicketSystem.Server/)
```
TicketSystem.Server/
├── Controllers/                  # API controllers
├── Properties/                   # Project properties and launch settings
├── bin/                         # Build output
├── obj/                         # Intermediate build files
├── Program.cs                   # Application entry point and configuration
├── appsettings.json             # Application configuration
├── appsettings.Development.json # Development-specific settings
├── TicketSystem.Server.csproj   # Project file
└── WeatherForecast.cs           # Example model
```

## Frontend Structure (ticketsystem.client/)
```
ticketsystem.client/
├── src/                         # Source code
├── public/                      # Static assets
├── node_modules/                # NPM dependencies
├── .vscode/                     # VS Code settings
├── obj/                         # Build artifacts
├── index.html                   # Entry HTML file
├── package.json                 # NPM configuration
├── vite.config.ts              # Vite configuration
├── tsconfig.json               # TypeScript configuration (references)
├── tsconfig.app.json           # App-specific TypeScript config
├── tsconfig.node.json          # Node-specific TypeScript config
├── eslint.config.js            # ESLint configuration
└── ticketsystem.client.esproj  # MSBuild project file for SPA
```

## Naming Conventions
- **Backend**: PascalCase for folders, files, and C# code (e.g., `TicketSystem.Server`, `Program.cs`)
- **Frontend**: camelCase for project folder (`ticketsystem.client`), standard React/TypeScript conventions for code
- **Configuration**: lowercase with dots for config files (e.g., `appsettings.json`, `vite.config.ts`)

## Key Files
- `Program.cs`: Backend startup configuration, middleware pipeline, and service registration
- `vite.config.ts`: Frontend build configuration, proxy setup, HTTPS certificates
- `TicketSystem.sln`: Solution file for building both projects together
- Path alias `@` maps to `ticketsystem.client/src` for cleaner imports

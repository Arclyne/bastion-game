# Bastion

Turn-based multiplayer desktop board game inspired by Quoridor. Final project for
*Tecnologías para la Construcción de Software* (Software Engineering, Universidad Veracruzana).

## Stack

| Concern | Technology |
|---|---|
| Language and runtime | C# on .NET 10 |
| User interface | MonoGame DesktopGL 3.8.5 |
| Networking | CoreWCF 1.9.1, duplex over net.tcp (`SecurityMode.None`), shared contracts on netstandard2.0 |
| Persistence | EF Core 10 with the SQL Server provider |
| Database | SQL Server 2022 in Docker |
| Logging | log4net |
| Tests | xUnit v3 |

Everything builds and runs on macOS arm64 and Windows.

## Solution layout

```
Bastion.slnx
├── src/
│   ├── Bastion.Client            Presentation: MonoGame screens, controls, .resx resources, WCF client
│   ├── Bastion.Contracts         Communication: service and callback contracts, data contracts (netstandard2.0)
│   ├── Bastion.Server.Services   Services: contract implementations, validation, security, sessions
│   ├── Bastion.Domain            Game rules: board, pawns, walls, path validation (no dependencies)
│   ├── Bastion.Server.Data       Data access: DbContext, entities, mappings, repositories
│   └── Bastion.Server.Host       Composition root: CoreWCF host, dependency injection, configuration
├── tests/
│   ├── Bastion.Domain.Tests
│   └── Bastion.Server.Services.Tests
└── database/                     SQL Server scripts (create database, tables, test data)
```

### Dependency rules

```
Bastion.Client ────────────► Bastion.Contracts ◄──────────────┐
                                                               │
Bastion.Server.Host ──► Bastion.Server.Services ──► Bastion.Domain
         │                        │
         └──────────────► Bastion.Server.Data ──► SQL Server
```

- References only point downwards. No project references a layer above it.
- `Bastion.Client` references `Bastion.Contracts` only. It cannot reach the database, the server or EF Core.
- EF Core entities never cross the network. Services map them to data contracts.
- The server returns result codes, never user-facing text. The client turns each code into a
  resource key (`Screen.Element`) and shows the text from `Strings.resx` (en-US) or `Strings.es-MX.resx`.
- Game rules and data are validated on the server. The client is never the source of truth.

### Folder conventions

Inside each project, code is grouped by feature, and the namespace follows the folder
(`Bastion.Server.Services.Accounts`, `Bastion.Contracts.Leaderboard`). Every type goes in its own
file named after the type.

## Getting started

1. Install the .NET 10 SDK and `sqlcmd`.
2. Start SQL Server 2022:

   - **Windows:** install SQL Server 2022 Developer, or run the container below with Docker Desktop.
   - **macOS (Apple silicon):** there is no ARM image, so the x64 image runs under Rosetta. With Homebrew:

     ```bash
     brew install colima docker sqlcmd
     colima start --vm-type vz --vz-rosetta --cpu 2 --memory 4
     ```

   ```bash
   docker run -d --name bastion-sql --platform linux/amd64 -e ACCEPT_EULA=Y -e MSSQL_PID=Developer -e MSSQL_SA_PASSWORD="<admin password>" -p 1433:1433 -v bastion-sql-data:/var/opt/mssql mcr.microsoft.com/mssql/server:2022-latest
   ```

3. Run the scripts in `database/` in order, with an administrator account. The first one creates the
   least-privilege login that the server uses. The files are UTF-8; with the classic ODBC `sqlcmd` on Windows add
   `-f 65001`.

   ```bash
   sqlcmd -S localhost -U sa -P "<admin password>" -C -v BastionServerPassword="<server password>" -i database/01_create_database.sql
   sqlcmd -S localhost -U sa -P "<admin password>" -C -i database/02_create_tables.sql
   sqlcmd -S localhost -U sa -P "<admin password>" -C -i database/03_insert_test_data.sql
   sqlcmd -S localhost -U sa -P "<admin password>" -C -i database/04_insert_leaderboard_demo.sql
   ```

   The fourth script is optional: it adds demonstration players so the leaderboard has rows. The passwords of the
   test accounts are listed in `03_insert_test_data.sql`.

4. Give the server its connection string through user-secrets (or the `ConnectionStrings__Bastion`
   environment variable). It is never committed:

   ```bash
   dotnet user-secrets set "ConnectionStrings:Bastion" "Server=localhost,1433;Database=Bastion;User Id=BastionServerConnection;Password=<server password>;TrustServerCertificate=True" --project src/Bastion.Server.Host
   ```

5. Build and run:

   ```bash
   dotnet build Bastion.slnx
   dotnet run --project src/Bastion.Server.Host
   dotnet run --project src/Bastion.Client
   ```

The server listens on the net.tcp port set in `src/Bastion.Server.Host/appsettings.json`. To reach a server on
another machine, start the client with `BASTION_SERVER_HOST` (and `BASTION_SERVER_PORT` if it is not 8000).

## Security notes

- The net.tcp binding uses `SecurityMode.None` on both sides, because the default Windows authentication does not
  work from macOS. This is compensated on the server: every rule is validated there, passwords are
  stored only as PBKDF2-SHA512 hashes with a per-account salt, sign-in answers do not reveal whether an account
  exists, repeated failures lock the account, and every later call is tied to a session token whose SHA-256 hash is
  the only thing stored.
- Only the server reaches the database, through the least-privilege `BastionServerConnection` login.
- Secrets (connection string, database passwords) live in user-secrets or environment variables.

## Coding standard

The team coding standard is enforced through `.editorconfig`, `CodeMetricsConfig.txt` and
`Directory.Build.props` (`EnforceCodeStyleInBuild`). Package versions are managed centrally in
`Directory.Packages.props`. `PublishTrimmed` and `PublishAot` must stay disabled because CoreWCF
relies on reflection.

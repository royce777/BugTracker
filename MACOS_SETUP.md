# BugTracker Development Setup for macOS Apple Silicon

## Prerequisites

- Install **.NET 6 SDK** (arm64 version) from Microsoft's official site
- Use **Visual Studio Code** with C# extension or **JetBrains Rider** (both have excellent Apple Silicon support)
- Install **Docker Desktop** for Mac (Apple Silicon version)

## Database Setup

### Option 1: SQL Server in Docker (Recommended)

1. **Run SQL Server Docker Container:**
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
   ```

2. **Set up Connection String in User Secrets:**
   ```bash
   # Navigate to BugTracker project folder
   cd BugTracker
   
   # Initialize user secrets
   dotnet user-secrets init
   
   # Add the connection string
   dotnet user-secrets set "ConnectionStrings:ApplicationDbContextConnection" "Server=localhost,1433;Database=BugTrackerDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;"
   ```

### Option 2: SQLite for Local Development
- Add `Microsoft.EntityFrameworkCore.Sqlite` package
- Update connection string to use SQLite

## Azure Integration Fix

**Problem:** App expects Azure Key Vault (`VaultUri` environment variable)

**Solution:** Comment out Azure Key Vault code in `Program.cs` lines 11-22 for local development, or set up local secrets.

## Development Commands

```bash
# Restore packages
dotnet restore

# Run migrations (after setting up database)
dotnet ef database update

# Run the application
dotnet run --project BugTracker
```

## Key Configuration Changes Needed

- ✅ Update connection string in user secrets (done above)
- ⚠️ Handle Azure Key Vault dependency for local development
- ✅ Use Docker SQL Server for database (done above)

## Troubleshooting

- Ensure Docker is running before starting the SQL Server container
- Use `sa` as User Id for SQL Server Docker container
- Password must meet SQL Server complexity requirements (uppercase, lowercase, numbers, special chars)
- Add `TrustServerCertificate=true` to connection string for local development

## Next Steps

1. Start Docker SQL Server container
2. Set up user secrets with connection string
3. Run database migrations
4. Handle Azure Key Vault dependency (comment out or configure locally)
5. Run the application
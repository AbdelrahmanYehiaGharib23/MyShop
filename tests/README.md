# Testing

The solution uses xUnit v3 with Microsoft's Testing Platform. Unit tests are kept in `tests/MyShop.UnitTests` and grouped by the production area they cover. HTTP integration tests are kept in `tests/MyShop.IntegrationTests` and run the MVC app through ASP.NET Core's in-memory test server. They do not require SQL Server, Redis, Stripe, or Paymob.

## Run locally

From the repository root:

```powershell
dotnet test MyShop..sln
```

To collect line and branch coverage in Cobertura format:

```powershell
dotnet test MyShop..sln --coverlet --coverlet-output-format cobertura
```

The same restore, build, test, and coverage steps run on pushes and pull requests through `.github/workflows/tests.yml`. GitHub Actions retains the Cobertura report as a downloadable artifact for 14 days.

## Test organization

- `MyShop.UnitTests/Models/Dto` verifies input validation rules.
- `MyShop.UnitTests/Services` verifies business behavior and authorization boundaries with mocked repositories.
- `MyShop.UnitTests/Extensions` verifies formatting behavior independent of the machine's current culture.
- `MyShop.IntegrationTests` verifies application startup, routing, middleware, and rendered HTTP responses through `WebApplicationFactory`.

The `Testing` environment skips production database initialization so the in-memory test server can start without creating or modifying a real database. Keep unit tests independent of live services, and use isolated test infrastructure for additional integration coverage.

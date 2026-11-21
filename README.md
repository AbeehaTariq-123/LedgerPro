# LedgerPro

ASP.NET Core MVC + Razor Pages ledger app (customers, suppliers, products, sales, purchases, payments, invoices, reports).

| Concern | How it's done |
|---|---|
| Login / Signup / Logout | **ASP.NET Core Identity pages** in `Areas/Identity/Pages/Account` (Login, Register, Logout, AccessDenied, Lockout), roles `Admin` and `Staff` |
| Data access | **Dapper** repositories in `Repositories/` (SQL Server) |
| Dependency injection | Interfaces in `Repositories/Interfaces`, registered in `Program.cs`, injected into controllers via constructors |
| AJAX | `wwwroot/js/*.js` load lists (`/Customer/GetAll`, ...), live-search them and delete rows with jQuery |

## Run it

1. Install the .NET 10 SDK and SQL Server LocalDB (comes with Visual Studio).
2. Set the code that allows creating **Admin** accounts (keep it out of git):

   ```
   cd LedgerPro
   dotnet user-secrets set AdminRegistrationCode "choose-a-long-random-code"
   ```
3. `dotnet run`, then open the URL it prints. `/` sends you to the login page.
4. Click **Create one now**. Choose *Staff*, or *Admin* and enter the code from step 2.

On first start the app applies the Identity migrations, creates the `LedgerPro` database and runs
`Database/ledgerpro-schema.sql` (only if the tables don't exist yet), and creates the two roles.

## Layout

```
Areas/Identity/Pages/Account/   Login, Register, Logout, AccessDenied, Lockout
Controllers/                    Home, Dashboard, Customer, Supplier, Product, Transaction, Payment, Invoice, Report
Repositories/                   Dapper implementations
Repositories/Interfaces/        IxxxRepository contracts
Models/, Models/ViewModels/     entities and view models
Views/                          Razor views (Shared/_AdminLayout, _StaffLayout, _AuthLayout)
wwwroot/js/                     AJAX scripts
Data/                           ApplicationDbContext (Identity), DbConnectionFactory, DatabaseInitializer, migrations
```

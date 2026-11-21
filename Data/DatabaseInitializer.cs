using Dapper;
using Microsoft.Data.SqlClient;

namespace LedgerPro.Data;

/// <summary>
/// Creates the business database and runs Database/ledgerpro-schema.sql the first time the app starts,
/// so a fresh checkout works without any manual SQL steps. Existing databases are left untouched.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task EnsureBusinessSchemaAsync(string connectionString, string schemaFile)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("DefaultConnection must specify a Database name.");

        // 1. Create the database if it doesn't exist (connect to master first).
        builder.InitialCatalog = "master";
        await using (var master = new SqlConnection(builder.ConnectionString))
        {
            await master.OpenAsync();
            var safeName = databaseName.Replace("]", "]]");
            await master.ExecuteAsync(
                $"IF DB_ID(@name) IS NULL CREATE DATABASE [{safeName}]",
                new { name = databaseName });
        }

        // 2. Create the tables once.
        await using var db = new SqlConnection(connectionString);
        await db.OpenAsync();

        var hasTables = await db.ExecuteScalarAsync<int>(
            "SELECT CASE WHEN OBJECT_ID('dbo.Customers', 'U') IS NULL THEN 0 ELSE 1 END");

        if (hasTables == 0)
        {
            if (!File.Exists(schemaFile))
                throw new FileNotFoundException("Database schema script not found.", schemaFile);

            await db.ExecuteAsync(await File.ReadAllTextAsync(schemaFile));
        }
    }
}

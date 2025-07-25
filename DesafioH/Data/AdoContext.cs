using Microsoft.Data.SqlClient;
using System.Data;

namespace DesafioH.Data
{
    public static class AdoContext
    {
        private const string MigrationsFolder = "Migrations";

        public static void Run(string connectionString)
        {
            
            EnsureDatabaseExists(connectionString);

            EnsureVersionTableExists(connectionString);

            var exePath = AppContext.BaseDirectory;
            var migrationsPath = Path.Combine(exePath, MigrationsFolder);

            if (!Directory.Exists(migrationsPath))
                throw new DirectoryNotFoundException($"No existe carpeta de scripts: {migrationsPath}");

            var scripts = Directory.GetFiles(migrationsPath, "*.sql")
                                   .OrderBy(f => f);

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            foreach (var scriptFile in scripts)
            {
                var version = Path.GetFileNameWithoutExtension(scriptFile);
                if (HasScriptBeenApplied(conn, version))
                    continue;

                var sql = File.ReadAllText(scriptFile);
                using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 600
                };
                cmd.ExecuteNonQuery();
                MarkScriptAsApplied(conn, version);
                Console.WriteLine($"[Migración aplicada] {version}");
            }
        }

        private static void EnsureVersionTableExists(string cs)
        {
            using var conn = new SqlConnection(cs);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF NOT EXISTS (
                  SELECT * FROM INFORMATION_SCHEMA.TABLES
                   WHERE TABLE_NAME = 'SchemaVersions'
                )
                CREATE TABLE SchemaVersions (
                  Version   NVARCHAR(100) PRIMARY KEY,
                  AppliedOn DATETIME2    NOT NULL DEFAULT SYSUTCDATETIME()
                );";
            cmd.ExecuteNonQuery();
        }

        private static bool HasScriptBeenApplied(SqlConnection conn, string version)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM SchemaVersions WHERE Version = @v";
            cmd.Parameters.AddWithValue("@v", version);
            return (int)cmd.ExecuteScalar() > 0;
        }

        private static void MarkScriptAsApplied(SqlConnection conn, string version)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO SchemaVersions (Version) VALUES (@v)";
            cmd.Parameters.AddWithValue("@v", version);
            cmd.ExecuteNonQuery();
        }
        private static void EnsureDatabaseExists(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using var conn = new SqlConnection(builder.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
        IF DB_ID(N'{databaseName}') IS NULL
          CREATE DATABASE [{databaseName}];
    ";
            cmd.ExecuteNonQuery();
        }

    }
}

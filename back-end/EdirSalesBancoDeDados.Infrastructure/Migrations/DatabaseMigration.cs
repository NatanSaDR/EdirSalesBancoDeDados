using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;


namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    public static class DatabaseMigration
    {
        public static void Migrate(string connectionString, IServiceProvider serviceProvider)
        {
            EnsureDatabaseCreated_SqlServer(connectionString);
            MogrationDatabase(serviceProvider);
        }
        private static void EnsureDatabaseCreated_SqlServer(string connectionString)
        {
            
            var connectrionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            
            var databaseName = connectrionStringBuilder.InitialCatalog;

            
            connectrionStringBuilder.Remove("Database");

            using var dbConnection = new SqlConnection(connectrionStringBuilder.ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("name", databaseName);

            var records = dbConnection.Query("SELECT * FROM sys.databases WHERE name = @name", parameters);

            if (records.Any() == false)
                dbConnection.Execute($"CREATE DATABASE {databaseName}");
        }

        private static void MogrationDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.ListMigrations();
            runner.MigrateUp();
        }
    }
}

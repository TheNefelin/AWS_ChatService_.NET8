using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace AWS_ChatService_Infrastructure.Configuration;

public class DapperConnectionFactory
{
    private readonly string _connectionString;

    public DapperConnectionFactory(IConfiguration configuration)
    {
    // Intentar primero la connection string completa
        _connectionString = configuration.GetConnectionString("PostgreSQL");
        
        // Si no existe o tiene variables, construirla desde env vars
        if (string.IsNullOrEmpty(_connectionString))
        {
            var host = configuration["DB_HOST"] ?? throw new ArgumentNullException("DB_HOST not found");
            var port = configuration["DB_PORT"] ?? throw new ArgumentNullException("DB_PORT not found");
            var database = configuration["DB_NAME"] ?? throw new ArgumentNullException("DB_NAME not found");
            var username = configuration["DB_USER"] ?? throw new ArgumentNullException("DB_USER not found");
            var password = configuration["DB_PASSWORD"] ?? throw new ArgumentNullException("DB_PASSWORD not found");
            
            _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }
        
        if (string.IsNullOrEmpty(_connectionString))
            throw new ArgumentNullException("Connection string could not be built");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
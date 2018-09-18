using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
 
namespace repository {
    public class ConnectionData {
        public string Host { get; private set; }
        public string Port { get; private set; }
        public string DatabaseName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public ConnectionData (string host, string port, string databaseName, string username, string password) {
            Host = host;
            Port = port;
            DatabaseName = databaseName;
            Username = username;
            Password = password;
        }
    }
    public class VersionContext : DbContext {

        private ConnectionData _connectionData;
        public DbSet<Version> Versions { get; set; }
        public DbSet<Product> Products { get; set; }

        public VersionContext(ConnectionData connectionData) {
            _connectionData = connectionData;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            string connectionString = $"Host={_connectionData.Host};Port={_connectionData.Port};Database={_connectionData.DatabaseName};" +
                                      $"Username={_connectionData.Username};Password={_connectionData.Password}";

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
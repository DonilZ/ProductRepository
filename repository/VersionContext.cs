using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
 
namespace repository {
    public class VersionContext : DbContext {

       // private ConnectionData _connectionData;
        public DbSet<Version> Versions { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=repository;Username=donilz;Password=1234");
        }
    }
}
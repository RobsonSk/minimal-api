using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using minimal_api.Domain.Entities;
using minimal_api.Domain.DTO;

namespace minimal_api.Infra.Db;

    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration _configurationAppSettings;
        public DatabaseContext(IConfiguration configurationAppSettings)
        {
            _configurationAppSettings = configurationAppSettings;
        }
        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<Vehicle> Vehicles { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Email = "adm@teste.com",
                    Password = "123456",
                    Profile = "admin"
                }
            );
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var stringConexao = _configurationAppSettings.GetConnectionString("MySql")?.ToString();
                if (!string.IsNullOrEmpty(stringConexao))
                {
                    optionsBuilder.UseMySql(
                        stringConexao,
                        ServerVersion.AutoDetect(stringConexao)
                    );
                }
            }
        }
    }

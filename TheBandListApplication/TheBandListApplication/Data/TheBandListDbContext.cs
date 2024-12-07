using Microsoft.EntityFrameworkCore;
using System;
using DotNetEnv;

namespace TheBandListApplication.Data
{
    internal class TheBandListDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Env.Load();

            string host = Environment.GetEnvironmentVariable("DB_HOST");
            string port = Environment.GetEnvironmentVariable("DB_PORT");
            string database = Environment.GetEnvironmentVariable("DB_NAME");
            string username = Environment.GetEnvironmentVariable("DB_USERNAME");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SslMode=Require;Trust Server Certificate=true;";

            optionsBuilder.UseNpgsql(connectionString);
        }

        //ici on déclare les DbSet pour chaque table de la base de données
    }
}
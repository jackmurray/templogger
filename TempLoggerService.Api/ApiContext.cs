using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using TempLoggerService.ModelsCore;
using TempLoggerService.ModelsCore.StoredProcedure;

namespace TempLoggerService.Api
{
    public class ApiContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }

        public static readonly Microsoft.Extensions.Logging.LoggerFactory _loggerFactory =
            new LoggerFactory(new[] {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>()
                .Property(t => t.Value)
                .HasColumnType("decimal(9,3)");

            modelBuilder.Entity<HourlyAverageTemperature>()
                .HasNoKey();
        }

        public StoredProcedure CreateStoredProcedure()
        {
            return new StoredProcedure(this.Database.GetDbConnection());
        }

        public StoredProcedure CreateStoredProcedure(string command)
        {
            return new StoredProcedure(this.Database.GetDbConnection(), command);
        }
    }

}
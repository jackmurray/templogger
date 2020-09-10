using Microsoft.EntityFrameworkCore;
using System;
using TempLoggerService.ModelsCore;

namespace TempLoggerService.Api
{
    public class ApiContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>()
                .Property(t => t.Value)
                .HasColumnType("decimal(9,3)");
        }
    }

}
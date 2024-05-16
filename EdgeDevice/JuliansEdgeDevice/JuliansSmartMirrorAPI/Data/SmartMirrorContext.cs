using Microsoft.EntityFrameworkCore;
using SmartMirror.Models;

public class SmartMirrorContext : DbContext
{
    public DbSet<SensorData> SensorData { get; set; }
    public DbSet<ConditionalRule> ConditionalRule { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=smartmirror.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConditionalRule>().HasData(new ConditionalRule
            {
                Id = 1,
                UpdateInterval = 1,
                TemperatureThreshold = 25,
                MotionEnabled = true
            });
    }
}
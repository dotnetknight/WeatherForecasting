using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WeatherForecasting.Domain.Entities;

namespace WeatherForecasting.Infrastructure;

public class WeatherDbContext(DbContextOptions<WeatherDbContext> options) : DbContext(options)
{
    public virtual DbSet<WeatherArchive> WeatherArchive => Set<WeatherArchive>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
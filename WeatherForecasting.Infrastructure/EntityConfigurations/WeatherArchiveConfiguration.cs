using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WeatherForecasting.Domain.Entities;

namespace WeatherForecasting.Infrastructure.EntityConfigurations;

public class WeatherArchiveConfiguration : IEntityTypeConfiguration<WeatherArchive>
{
    public void Configure(EntityTypeBuilder<WeatherArchive> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Latitude)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .IsRequired();

        builder.Property(x => x.TemperatureC)
            .IsRequired();

        builder.Property(x => x.TemperatureF)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => new
        {
            x.City,
            x.Longitude,
            x.Latitude,
            x.Date
        }).IsUnique();
    }
}
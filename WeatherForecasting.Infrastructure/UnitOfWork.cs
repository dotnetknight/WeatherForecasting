using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Infrastructure;

public class UnitOfWork(WeatherDbContext context) : IUnitOfWork
{
    private readonly WeatherDbContext _context = context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}

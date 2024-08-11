using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WeatherForecasting.Contracts.Models;
using WeatherForecasting.Domain.Common;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Infrastructure;

public class Repository<T>(WeatherDbContext context) : IRepository<T> where T : class
{
    private readonly WeatherDbContext _context = context;
    private static readonly char[] separator = [','];

    public async Task<T?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _context
        .Set<T>()
        .FindAsync([id], cancellationToken: cancellationToken);

    public async Task<IEnumerable<T>> GetAll(
        BaseResourceParameters? resourceParameters = null,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<T>().AsQueryable();

        if (filter is not null)
        {
            query = query.Where(filter);
        }
        if (orderBy is not null)
        {
            return await orderBy(query).ToListAsync(cancellationToken);
        }
        else
        {
            return await query.ToListAsync(cancellationToken);
        }
    }

    public async Task<T?> GetFirst(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<T>().AsQueryable();
        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<PagedList<T>> GetPage(
        BaseResourceParameters resourceParameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<T>().AsQueryable();

        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((resourceParameters.PageNumber - 1) * resourceParameters.PageSize)
                                .Take(resourceParameters.PageSize)
                                .ToListAsync(cancellationToken);

        return new PagedList<T>(items.AsQueryable(), count, resourceParameters.PageNumber, resourceParameters.PageSize);
    }

    public async Task Add(T entity, CancellationToken cancellationToken = default)
        => await _context
        .Set<T>()
        .AddAsync(entity, cancellationToken);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public void Delete(T entity)
        => _context.Set<T>().Remove(entity);
}
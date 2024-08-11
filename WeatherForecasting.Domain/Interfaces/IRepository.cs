using System.Linq.Expressions;
using WeatherForecasting.Contracts.Models;
using WeatherForecasting.Domain.Common;

namespace WeatherForecasting.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(
        BaseResourceParameters? resourceParameters = null,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default);

    Task<T?> GetFirst(
         Expression<Func<T, bool>> filter,
         CancellationToken cancellationToken = default);

    Task<PagedList<T>> GetPage(
        BaseResourceParameters resourceParameters,
        CancellationToken cancellationToken = default);

    Task<T?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task Add(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
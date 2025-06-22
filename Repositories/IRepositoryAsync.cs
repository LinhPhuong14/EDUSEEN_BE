namespace Sep490_Eduseen_BE.Repositories;

public interface IRepositoryAsync<T> where T : class
{
    Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
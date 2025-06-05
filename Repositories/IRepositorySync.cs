namespace Sep490_Eduseen_BE.Repositories;

public interface IRepositorySync<T> where T : class
{
    IEnumerable<T> Get();
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}
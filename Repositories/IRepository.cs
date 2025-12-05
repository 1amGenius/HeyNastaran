using System.Linq.Expressions;

namespace Nastaran_bot.Repositories;

/// <summary>
/// Defines a generic repository abstraction for CRUD operations and filtered queries.
/// </summary>
/// <typeparam name="T">Entity type stored in the repository.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <returns>All entities in the repository.</returns>
    public Task<IEnumerable<T>> FindAllAsync();

    /// <summary>
    /// Retrieves a single entity by its unique identifier.
    /// </summary>
    /// <param name="id">Entity identifier.</param>
    /// <returns>The matching entity, or null if not found.</returns>
    public Task<T> FindByIdAsync(string id);

    /// <summary>
    /// Retrieves entities associated with a specific Telegram ID.
    /// </summary>
    /// <param name="telegramId">Telegram user ID linked to the entities.</param>
    /// <returns>Matching entities.</returns>
    public Task<IEnumerable<T>> FindByTelegramIdAsync(long telegramId);

    /// <summary>
    /// Retrieves entities matching the provided filter expression.
    /// </summary>
    /// <param name="filter">Expression defining query conditions.</param>
    /// <returns>Matching entities.</returns>
    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    public Task CreateAsync(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">Entity with updated values.</param>
    public Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the entity to delete.</param>
    /// <returns>True if deletion succeeded; otherwise false.</returns>
    public Task<bool> DeleteAsync(string id);
}

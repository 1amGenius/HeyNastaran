using System.Linq.Expressions;

namespace Core.Repositories;

/// <summary>
/// Provides a generic abstraction for async CRUD operations and Telegram-oriented queries.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retrieves all entities stored in the repository.
    /// </summary>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>An asynchronous stream of all entities.</returns>
    public IAsyncEnumerable<TEntity> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>The matching entity, or null if no match is found.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided entity ID is null or empty.
    /// </exception>
    public Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all entities associated with a specific Telegram user ID.
    /// </summary>
    /// <param name="telegramId">The Telegram user identifier.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>An asynchronous stream of matching entities.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the provided Telegram ID is not a valid identifier (e.g., non-positive).
    /// </exception>
    public IAsyncEnumerable<TEntity> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves entities that satisfy the specified filter expression.
    /// </summary>
    /// <param name="predicate">An expression defining the filtering criteria.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>An asynchronous stream of matching entities.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided predicate is null.
    /// </exception>
    public IAsyncEnumerable<TEntity> QueryAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided entity is null.
    /// </exception>
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity containing updated values.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided entity is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided entity has invalid or missing identifier information.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the entity to update does not exist in the repository.
    /// </exception>
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>True if deletion succeeds; otherwise false.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided entity ID is null or empty.
    /// </exception>
    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

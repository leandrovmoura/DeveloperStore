using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier including items
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its sale number including items
    /// </summary>
    /// <param name="saleNumber">The sale number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales with optional filtering
    /// </summary>
    /// <param name="includeItems">Whether to include sale items</param>
    /// <param name="includeCancelled">Whether to include cancelled sales</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sales</returns>
    Task<List<Sale>> GetAllAsync(bool includeItems = true, bool includeCancelled = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="sale">The sale to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales by customer identifier
    /// </summary>
    /// <param name="customerId">The customer identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sales for the customer</returns>
    Task<List<Sale>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales by branch identifier
    /// </summary>
    /// <param name="branchId">The branch identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sales for the branch</returns>
    Task<List<Sale>> GetByBranchIdAsync(string branchId, CancellationToken cancellationToken = default);
}
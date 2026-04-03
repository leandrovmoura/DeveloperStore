using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database
    /// </summary>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier including items
    /// </summary>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a sale by its sale number including items
    /// </summary>
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <summary>
    /// Retrieves all sales with optional filtering
    /// </summary>
    public async Task<List<Sale>> GetAllAsync(bool includeItems = true, bool includeCancelled = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsQueryable();

        if (includeItems)
        {
            query = query.Include(s => s.Items);
        }

        if (!includeCancelled)
        {
            query = query.Where(s => !s.IsCancelled);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var existingSale = _context.Update(sale);

        await _context.SaveChangesAsync(cancellationToken);

        return existingSale.Entity;
    }

    /// <summary>
    /// Deletes a sale by its unique identifier
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Gets sales by customer identifier
    /// </summary>
    public async Task<List<Sale>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets sales by branch identifier
    /// </summary>
    public async Task<List<Sale>> GetByBranchIdAsync(string branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a paged list of sales
    /// </summary>
    public async Task<PagedResult<Sale>> GetPagedAsync(int pageNumber, int pageSize, bool includeItems = true, bool includeCancelled = false, string? orderBy = null, bool isDescending = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsQueryable();
        
        // 1. Apply filtering (cancelled/active)
        if (!includeCancelled) query = query.Where(s => !s.IsCancelled);
        
        // 2. Include items if needed
        if (includeItems) query = query.Include(s => s.Items);
        
        // 3. Get total count
        var totalCount = await query.CountAsync();

        // 4. Apply ordering
        query = ApplyOrdering(query, orderBy, isDescending);

        // 5. Apply pagination
        var items = await query.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        
        return new PagedResult<Sale> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
    }

    /// <summary>
    /// Apply ordering to the list
    /// </summary>
    private static IQueryable<Sale> ApplyOrdering(
        IQueryable<Sale> query, 
        string? orderBy,
        bool isDescending)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            return query.OrderByDescending(s => s.CreatedAt); // Default ordering
        }
        else
        {
            // Switch expression with all sortable fields
            return orderBy.ToLower() switch
            {
                "saledate" => isDescending ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate),
                "salenumber" => isDescending ? query.OrderByDescending(s => s.SaleNumber) : query.OrderBy(s => s.SaleNumber),
                "totalamount" => isDescending ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
                "customername" => isDescending ? query.OrderByDescending(s => s.CustomerName) : query.OrderBy(s => s.CustomerName),
                "branchname" => isDescending ? query.OrderByDescending(s => s.BranchName) : query.OrderBy(s => s.BranchName),
                "createdat" => isDescending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
                _ => query.OrderByDescending(s => s.CreatedAt)  // Default
            };
        }
    }
}
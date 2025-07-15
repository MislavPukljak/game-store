using Data.MongoDb.Entities;
using Data.MongoDb.Enums;
using Data.MongoDb.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Data.MongoDb.Repository;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly IMongoCollection<Product> _dbSet;
    private readonly IMongoCollection<Supplier> _dbSetSupplier;
    private readonly IMongoCollection<Category> _dbSetCategory;

    public ProductRepository(IMongoContext context)
        : base(context, "products")
    {
        _dbSet = context.GetCollection<Product>("products");
        _dbSetSupplier = context.GetCollection<Supplier>("suppliers");
        _dbSetCategory = context.GetCollection<Category>("categories");
    }

    public async Task<ProductPageInfo> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken)
    {
        var filterProduct = Builders<Product>.Filter.Empty;
        var all = await _dbSet.FindAsync(filterProduct, cancellationToken: cancellationToken);
        var products = all.ToList(cancellationToken);

        products = await FilterByCategories(products, filter.Category);
        products = await FilterBySuppliers(products, filter.Supplier);
        products = FilterByPrice(products, filter.PriceFrom, filter.PriceTo);
        products = FilterByPublishedDate(products);
        products = FilterByPartName(products, filter.NameStart);
        products = SortProducts(products, filter.Sort);

        var selectedProducts = products
            .Skip((filter.CurrentPage - 1) * (int)filter.ItemsPerPage)
            .Take((int)filter.ItemsPerPage)
            .ToList();

        var response = new ProductPageInfo
        {
            Products = selectedProducts,
            CurrentPage = filter.CurrentPage,
            TotalPages = (int)Math.Ceiling((double)products.Count / (int)filter.ItemsPerPage),
            TotalItems = products.Count,
            PageSize = (int)filter.ItemsPerPage,
        };

        return response;
    }

    public async Task<Product> GetByProductId(int id, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("ProductID", id);

        var suppliers = await _dbSet
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return suppliers;
    }

    public async Task<Product> GetByAliasAsync(string alias, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("Alias", alias);

        var suppliers = await _dbSet
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return suppliers;
    }

    public async Task DeleteProductAsync(string alias, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("Alias", alias);

        await _dbSet.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductBySuplierId(int id, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("SupplierID", id);

        var suppliers = await _dbSet
            .Find(filter)
            .ToListAsync(cancellationToken);

        return suppliers;
    }

    public async Task<IEnumerable<Product>> GetProductByCategoryId(int id, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("CategoryID", id);

        var suppliers = await _dbSet
            .Find(filter)
            .ToListAsync(cancellationToken);

        return suppliers;
    }

    public async Task<IEnumerable<Product>> GetProductBySupplier(string companyName, CancellationToken cancellationToken)
    {
        var supplierFilter = Builders<Supplier>.Filter.Eq("CompanyName", companyName);
        var supplier = await _dbSetSupplier.Find(supplierFilter).FirstOrDefaultAsync(cancellationToken);

        if (supplier == null)
        {
            return new List<Product>();
        }

        var productFilter = Builders<Product>.Filter.Eq("SupplierID", supplier.SupplierID);
        var products = await _dbSet.Find(productFilter).ToListAsync(cancellationToken);

        return products;
    }

    public async Task UpdateUnitInStock(string alias, Product product, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("Alias", alias);
        var update = Builders<Product>.Update.Set("UnitsInStock", product.UnitsInStock);

        await _dbSet.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    private async Task<List<Product>> FilterByCategories(List<Product> products, List<string?> categories)
    {
        var categoryFilter = Builders<Category>.Filter.Eq("CategoryName", categories);
        var category = await _dbSetCategory.Find(categoryFilter).ToListAsync();

        if (!category.Any())
        {
            return products;
        }

        var productFilter = products.Where(g => category.Exists(c => g.CategoryID == c.CategoryId)).ToList();

        return productFilter;
    }

    private async Task<List<Product>> FilterBySuppliers(List<Product> products, string? companyName)
    {
        var supplierFilter = Builders<Supplier>.Filter.Eq("CompanyName", companyName);
        var supplier = await _dbSetSupplier.Find(supplierFilter).FirstOrDefaultAsync();

        if (supplier == null)
        {
            return products;
        }

        var productFilter = products.Where(g => g.SupplierID == supplier.SupplierID).ToList();

        return productFilter;
    }

    private static List<Product> FilterByPrice(List<Product> products, decimal? minPrice, decimal? maxPrice)
    {
        if (minPrice.HasValue)
        {
            products = products.Where(x => x.UnitPrice >= minPrice).ToList();
        }

        if (maxPrice.HasValue)
        {
            products = products.Where(x => x.UnitPrice <= maxPrice).ToList();
        }

        return products;
    }

    private static List<Product> FilterByPublishedDate(List<Product> products)
    {
        return products;
    }

    private static List<Product> FilterByPartName(List<Product> products, string? partName)
    {
        return string.IsNullOrEmpty(partName) ? products : products.Where(g => g.ProductName.Contains(partName)).ToList();
    }

    private static List<Product> SortProducts(List<Product> products, OrderFilter.OrderBy? sort)
    {
        return sort switch
        {
            OrderFilter.OrderBy.MostPopular => products.OrderByDescending(product => product.ProductName).ToList(),
            OrderFilter.OrderBy.MostCommented => products.OrderByDescending(product => product.ProductName).ToList(),
            OrderFilter.OrderBy.PriceAsc => products.OrderBy(product => product.UnitPrice).ToList(),
            OrderFilter.OrderBy.PriceDesc => products.OrderByDescending(product => product.UnitPrice).ToList(),
            OrderFilter.OrderBy.ByDate => products.OrderByDescending(product => product.ProductName).ToList(),
            _ => products.OrderByDescending(product => product.ProductName).ToList(),
        };
    }
}
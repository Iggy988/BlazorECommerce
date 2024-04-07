using BlazorECommerce.Shared;

namespace BlazorECommerce.Client.Services.ProductService;

public interface IProductService
{
    event Action ProductsChanged;
    List<Product> Products { get; set; }
    string Message { get; set; } //to show message that product has been found
    Task GetProducts(string? categoryUrl = null);

    Task<ServiceResponse<Product>> GetProduct(int productId);

    Task SearchProduct(string searchText);
    Task<List<string>> GetProductSearchSuggestions(string searchText);


}

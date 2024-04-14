using BlazorECommerce.Shared;

namespace BlazorECommerce.Client.Services.ProductService;

public interface IProductService
{
    event Action ProductsChanged;
    List<Product> Products { get; set; }
    string Message { get; set; } //to show message that product has been found

    int CurrentPage { get; set; }   
    int PageCount { get; set; }  
    string LastSearchText { get; set; }

    Task GetProducts(string? categoryUrl = null);

    Task<ServiceResponse<Product>> GetProduct(int productId);

    Task SearchProduct(string searchText, int page);
    Task<List<string>> GetProductSearchSuggestions(string searchText);


}


global using System.Net.Http.Json;
using BlazorECommerce.Shared.DTO;

namespace BlazorECommerce.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public List<Product> Products { get; set; } = new List<Product>();
    public string Message { get; set; } = "Loading products...";
    public int CurrentPage { get; set; } = 1;
    public int PageCount { get; set; } = 0;
    public string LastSearchText { get; set; } = string.Empty;

    public event Action ProductsChanged;

    public async Task<ServiceResponse<Product>> GetProduct(int productId)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
        return result;
    }

    public async Task GetProducts(string? categoryUrl = null)
    {
        //if we dont get categoryUrl we return all product page, else we return page with specific category
        var result = categoryUrl == null ?
            await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured") :
            await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
        if (result is not null && result.Data != null)
            Products = result.Data;

        CurrentPage = 1;
        PageCount = 0;

        if (Products.Count == 0)
        {
            Message = "No products found";
        }

        ProductsChanged.Invoke();
    }

    public async Task<List<string>> GetProductSearchSuggestions(string searchText)
    {
        var result = await _httpClient
            .GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");
        return result.Data;
    }

    public async Task SearchProduct(string searchText, int page)
    {
        LastSearchText = searchText;
        var result = await _httpClient
            .GetFromJsonAsync<ServiceResponse<ProductSearchResultDTO>>($"api/product/search/{searchText}/{page}");

        if (result != null && result.Data != null)
        {
            Products = result.Data.Products;
            CurrentPage = result.Data.CurrentPage;
            PageCount = result.Data.Pages;
        }
        if (Products.Count == 0) Message = "No products found.";
        ProductsChanged?.Invoke();
    }
}

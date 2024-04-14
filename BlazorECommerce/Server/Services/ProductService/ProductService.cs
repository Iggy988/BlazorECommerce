
using BlazorECommerce.Shared;
using BlazorECommerce.Shared.DTO;

namespace BlazorECommerce.Server.Services.ProductService;

public class ProductService : IProductService
{
    private readonly DataContext _context;

    public ProductService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
    {
        var response = new ServiceResponse<List<Product>>
        {
            // to include variants 
            Data = await _context.Products.Include(p => p.Variants).ToListAsync()
        };
        return response;
    }

    public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
    {
        var response = new ServiceResponse<Product>();
        // to include variants and producttype
        var product = await _context.Products
            .Include(p => p.Variants)
            .ThenInclude(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
        {
            response.Success = false;
            response.Message = "Sorry, but this product does not exist.";
        }
        else
        {
            response.Data = product;
        }

        return response;
    }

    public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
    {
        var response = new ServiceResponse<List<Product>>
        {
            Data = await _context.Products
            .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
            .Include(p => p.Variants)
            .ToListAsync()
        };

        return response;
    }

    public async Task<ServiceResponse<ProductSearchResultDTO>> SearchProducts(string searchText, int page)
    {
        var pageResults = 2f; //sets the number of results per page to 2
        //calculates the total number of pages by dividing the total number of products found by
        //FindProductBySearchText(searchText) by the number of results per page. It rounds up to the nearest whole number using Math.Ceiling.
        var pageCount = Math.Ceiling((await FindProductBySearchText(searchText)).Count()/pageResults);
        //search for products in the _context.Products database where the product title or description contains the searchText.
        var products = await _context.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                                         ||
                                         p.Description.ToLower().Contains(searchText.ToLower()))
                            //It includes the product variants in the results
                            .Include(p => p.Variants)
                            //skips the products of the previous pages and takes only the products for the current page.
                            .Skip((page - 1) * (int)pageResults)
                            .Take((int)pageResults)
                            .ToListAsync();


        var response = new ServiceResponse<ProductSearchResultDTO>
        {
            Data = new ProductSearchResultDTO
            {
                Products = products,
                CurrentPage = page,
                Pages = (int)pageCount
            }
        };
        return response;
    }

    private async Task<List<Product>> FindProductBySearchText(string searchText)
    {
        return await _context.Products
                    .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                                 ||
                                 p.Description.ToLower().Contains(searchText.ToLower()))
                    .Include(p => p.Variants)
                    .ToListAsync();
    }

    public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
    {
        var products = await FindProductBySearchText(searchText);

        List<string> result = new();

        foreach (var product in products)
        {
            if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(product.Title);
            }

            if (product.Description is not null)
            {

                /*
                 var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();

                The char.IsPunctuation is a method that checks if a character is a punctuation.
                Distinct(): This method is used to ensure that each punctuation character is included only once in the result
                ToArray(): This method is used to convert the result into an array.
                For example, if product.Description is "Hello, World! How's it going?", the punctuation variable will be an array containing [',', '!', ''', '?'].
                var words = product.Description.Split().Select(s => s.Trim(punctuation));
                This line of code is splitting the product.Description string into words and removing any leading or trailing punctuation from each word.
                product.Description.Split(): This splits the product.Description string into words based on spaces.
                Select(s => s.Trim(punctuation)): This uses a LINQ query to remove any leading or trailing punctuation from each word. 
                The Trim() method removes characters from the start and end of a string. Here it’s being used with the punctuation array as an argument, so it removes any leading or trailing punctuation characters.
                For example, if product.Description is "Hello, World! How's it going?", the words variable will be an array containing ["Hello", "World", "How", "s", "it", "going"].
                 */

                var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
                var words = product.Description.Split().Select(s => s.Trim(punctuation));

                foreach (var word in words)
                {
                    if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                    {
                        result.Add(word);
                    }
                }
            }
        }

        return new ServiceResponse<List<string>> { Data = result };
    }

    public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
    {
        var response = new ServiceResponse<List<Product>>
        {
            Data = await _context.Products
                    .Where(p => p.Featured)
                    .Include(p => p.Variants )
                    .ToListAsync()
        };
        return response;
    }

}

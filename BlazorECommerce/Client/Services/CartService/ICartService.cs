using BlazorECommerce.Shared.DTO;

namespace BlazorECommerce.Client.Services.CartService;

public interface ICartService
{
    event Action OnChange; // whenever something changes in cart
    Task AddToCart(CartItem cartItem);
    Task<List<CartItem>> GetCartItems();
    Task<List<CartProductResponseDTO>> GetCartProducts();
    Task RemoveProductFromCart(int productId, int productTypeId);
    Task UpdateQuantity(CartProductResponseDTO products);
    Task StoreCartItems(bool emptyLocalCart);
}

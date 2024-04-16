using BlazorECommerce.Shared.DTO;

namespace BlazorECommerce.Client.Services.CartService;

public interface ICartService
{
    event Action OnChange; // whenever something changes in cart
    Task AddToCart(CartItem cartItem);
    Task<List<CartItem>> GetCartItems();
    Task<List<CartProductResponseDTO>> GetCartProducts();
}

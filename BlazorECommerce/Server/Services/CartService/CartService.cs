﻿using BlazorECommerce.Shared.DTO;
using System.Security.Claims;

namespace BlazorECommerce.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;
    //allows you to access various aspects of the HTTP request and response, such as headers,
    //cookies, query parameters, and user claims
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

   

    public async Task<ServiceResponse<List<CartProductResponseDTO>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = new ServiceResponse<List<CartProductResponseDTO>>
        {
            Data = new List<CartProductResponseDTO>()
        };

        foreach (var item in cartItems)
        {
            var product = await _context.Products
                .Where(p => p.Id == item.ProductId)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                continue;
            }

            var productVariant = await _context.ProductVariants
                .Where(v => v.ProductId == item.ProductId && v.ProductTypeId == item.ProductTypeId)
                .Include(v => v.ProductType)
                .FirstOrDefaultAsync();

            if (productVariant == null)
            {
                continue;
            }

            var cartProduct = new CartProductResponseDTO
            {
                ProductId = product.Id,
                Title = product.Title,
                ImageUrl = product.ImgUrl,
                Price = productVariant.Price,
                ProductType = productVariant.ProductType.Name,
                ProductTypeId = productVariant.ProductTypeId,
                Quantity = item.Quantity,
            };

            result.Data.Add(cartProduct);
        }

        

        return result;
    }

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

    public async Task<ServiceResponse<List<CartProductResponseDTO>>> StoreCartItems(List<CartItem> cartItems)
    {
        cartItems.ForEach(cartItem => cartItem.UserId = GetUserId());
        _context.CartItems.AddRange(cartItems);
        await _context.SaveChangesAsync();

        return await GetCartProducts(await _context.CartItems.Where(ci => ci.UserId == GetUserId()).ToListAsync());
    }

    public async Task<ServiceResponse<int>> GetCartItemsCount()
    {
        var count = (await _context.CartItems.Where(ci => ci.UserId == GetUserId()).ToListAsync()).Count;

        return new ServiceResponse<int>
        {
            Data = count
        };

    }
}

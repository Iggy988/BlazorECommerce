﻿using BlazorECommerce.Shared.DTO;

namespace BlazorECommerce.Server.Services.CartService;

public interface ICartService
{
    Task<ServiceResponse<List<CartProductResponseDTO>>> GetCartProducts(List<CartItem> cartItems);
}
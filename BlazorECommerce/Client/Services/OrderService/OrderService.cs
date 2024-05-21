
using BlazorECommerce.Shared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorECommerce.Client.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly HttpClient _http;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public OrderService(HttpClient http, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
    {
        _http = http;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task<OrderDetailsResponseDTO> GetOrderDetails(int orderId)
    {
        var result = await 
            _http.GetFromJsonAsync<ServiceResponse<OrderDetailsResponseDTO>>($"api/order/{orderId}");

        return result.Data;
    }

    public async Task<List<OrderOverviewResponseDTO>> GetOrders()
    {
        var result = await 
            _http.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponseDTO>>>("api/order");
        return result.Data;
    }

    public async Task<string> PlaceOrder()
    {
        if (await IsUserAuthenticated())
        {
            var result = await _http.PostAsync("api/payment/checkout", null);
            var url = await result.Content.ReadAsStringAsync();
            return url;
        }
        else
        {
            //_navigationManager.NavigateTo("login");
            return "login";
        }
    }


    private async Task<bool> IsUserAuthenticated()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        return authState.User?.Identity?.IsAuthenticated ?? false;
    }
}

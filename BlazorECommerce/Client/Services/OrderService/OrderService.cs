
using Microsoft.AspNetCore.Components;

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

    public async Task PlaceOrder()
    {
        if (await IsUserAuthenticated())
        {
            await _http.PostAsync("api/order", null);
        }
        else
        {
            _navigationManager.NavigateTo("login");
        }
    }


    private async Task<bool> IsUserAuthenticated()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        return authState.User?.Identity?.IsAuthenticated ?? false;
    }
}

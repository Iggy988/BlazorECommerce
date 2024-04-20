﻿
namespace BlazorECommerce.Client.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }
    public async Task<ServiceResponse<int>> Register(UserRegister request)
    {
        var result = await _http.PostAsJsonAsync("api/auth/register", request);

        //ServiceResponse<int>? serviceResponse = await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        //if (serviceResponse == null)
        //{
        //    throw new Exception("Deserialization failed");
        //}
        //return serviceResponse;

        return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
    }
}
global using BlazorECommerce.Shared;
global using BlazorECommerce.Client.Services.ProductService;
global using BlazorECommerce.Client.Services.CategoryService;
global using BlazorECommerce.Client.Services.CartService;
global using Blazored.LocalStorage;
global using BlazorECommerce.Client.Services.AuthService;

global using BlazorECommerce.Client.Services.OrderService;
global using BlazorECommerce.Client.Services.AddressService;

using BlazorECommerce.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;






var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();


await builder.Build().RunAsync();

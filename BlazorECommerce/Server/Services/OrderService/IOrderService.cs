using BlazorECommerce.Shared.DTO;

namespace BlazorECommerce.Server.Services.OrderService;

public interface IOrderService
{
    Task<ServiceResponse<bool>> PlaceOrder();
    Task<ServiceResponse<List<OrderOverviewResponseDTO>>> GetOrders();
}

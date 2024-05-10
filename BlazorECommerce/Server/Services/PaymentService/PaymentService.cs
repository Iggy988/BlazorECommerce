using Stripe;
using Stripe.Checkout;

namespace BlazorECommerce.Server.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _config;

    public PaymentService(ICartService cartService, IAuthService authService, IOrderService orderService, IConfiguration config)
    {
        _cartService = cartService;
        _authService = authService;
        _orderService = orderService;
        _config = config;

        StripeConfiguration.ApiKey = _config["AppSettings:StripeApiKey"]; 

        
      
    }

    public async Task<Session> CreateCheckoutSession()
    {
        var products = (await _cartService.GetDbCartProducts()).Data;
        var lineItem = new List<SessionLineItemOptions>();
        products.ForEach(product => lineItem.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = product.Price * 100,
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = product.Title,
                    Images = new List<string>
                    {
                        product.ImageUrl
                    }
                }
            },
            Quantity = product.Quantity
        }));

        var options = new SessionCreateOptions
        {
            CustomerEmail = _authService.GetUserEmail(),
            PaymentMethodTypes = new List<string>
            {
                "card"
            },
            LineItems = lineItem,
            Mode = "payment",
            SuccessUrl = "https://localhost:7118/order-success",
            CancelUrl = "https://localhost:7118/cart"
        };

        var service = new SessionService();
        Session session = service.Create(options);
        return session;
    }
}

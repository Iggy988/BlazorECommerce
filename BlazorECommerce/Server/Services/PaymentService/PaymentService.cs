using Stripe;
using Stripe.Checkout;

namespace BlazorECommerce.Server.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _config;

    const string secret = "whsec_b4cb9c0f6a5a6eef415b4c452fecfa04a2bba4c252d85368099929715cf1e76a";

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
            ShippingAddressCollection= new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string> { "US"}
                },
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

    public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                request.Headers["Stripe-Signature"],
                secret
                );
            if (stripeEvent.Type == Events.CheckoutSessionCompleted )
            {
                var session = stripeEvent.Data.Object as Session;
                var user = await _authService.GetUserByEmail(session.CustomerEmail);
                await _orderService.PlaceOrder(user.Id);
            }

            return new ServiceResponse<bool>
            {
                Data = true,
            };
        }
        catch (StripeException ex)
        {
            return new ServiceResponse<bool> 
            {
                Data = false, 
                Success = false, 
                Message = ex.Message
            };
        }
    }
}

namespace BlazorECommerce.Client.Services.AuthService;

public interface IAuthService
{
    //int -> user id
    Task<ServiceResponse<int>> Register(UserRegister request);
}


using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BlazorECommerce.Server.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    //IHttpContextAccessor is a built-in interface in ASP.NET Core that provides access to the HttpContext1HttpContext encapsulates
    //all HTTP-specific information about an individual HTTP request and response2.
    //allows you to access various aspects of the HTTP request and response, such as headers,
    //cookies, query parameters, and user claims
    public AuthService(DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    public string GetUserEmail() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    public async Task<ServiceResponse<string>> Login(string email, string password)
    {
        var response = new ServiceResponse<string>();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));

        if (user == null)
        {
            response.Success = false;
            response.Message = "User not found.";
        }
        else if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Wrong password.";
        }
        else
        {
            response.Data = CreateToken(user);
        }

        
        return response;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            //The ComputeHash method takes a byte array and returns the hash as a byte array.
            //Here, it’s computing the hash of the provided password
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            //The SequenceEqual method checks if two sequences are equal by comparing the elements by using the default equality
            //comparer for their type. Here, it’s comparing the computed hash of the provided password with the correct password hash.
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        if(await UserExists(user.Email))
        {
            return new ServiceResponse<int> 
            {
                Success = false, Message= "User already exists." 
            };
        }

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;  
        
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new ServiceResponse<int>
        {
            Data = user.Id,
            Message = "Registration Successfull!"
        };
    }

    public async Task<bool> UserExists(string email)
    {
        if (await _context.Users.AnyAsync(user => user.Email.ToLower().Equals(email.ToLower())))
        {
            return true;
        }
        return false;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        //we put password in this alhorithm, that will generate key
        using (var hmac = new HMACSHA512()) 
        {
            //generate key will be used for passwordSalt
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private string CreateToken(User user)
    {
        //claims to store in Json web token
        List<Claim> claims = new List<Claim> 
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "User not found"
            };
        }

        CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        await _context.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "Password has been changed."
        };
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
    }
}

using BlazorECommerce.Client.Pages;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorECommerce.Client;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _http;

    public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http)
    {
        _localStorageService = localStorageService;
        _http = http;
    }

    //get token from local storage, pass claims and then create new ClaimsIdentity
    //then notify components that want to be notify of this new ClaimsIdentity,
    //and then with that info, app will know is this user authenticated or not
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string authToken = await _localStorageService.GetItemAsStringAsync("authToken") ?? 
            throw new InvalidOperationException("Authentication token not found");

        var identity = new ClaimsIdentity();
        //user is currently in this stage unauthorize
        _http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(authToken))
        {
            try
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                _http.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", authToken.Replace("\"","")); //remove "
            }
            catch
            {
                await _localStorageService.RemoveItemAsync("authToken");
                identity = new ClaimsIdentity();
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

    //Convert a Base64 string to a byte array, even if the Base64 string is not properly padded
    private byte[] ParseBase64WithoutPadding(string base64)
    {
        //a valid Base64 string should have a length that is a multiple of 4. If it’s not, it means the string is not properly padded
        switch (base64.Length % 4)
        {
            // the string is padded with == or = respectively to make its length a multiple of 4
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }
        // After ensuring the base64 string is properly padded, it is converted to a byte array
        return Convert.FromBase64String(base64);
    }

    /*
 * JWT token
    eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.  -Header
    eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.  -Payload
    SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c  -Signature
 */

    //method is designed to extract claims from a JSON Web Token (JWT)
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        //The JWT is split into parts using the.character as a delimiter.
        //A JWT typically consists of three parts: header, payload, and signature, separated by..
        //The method takes the second part, which is the payload
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        //The payload is a JSON string that represents a set of claims.
        //The JsonSerializer.Deserialize<Dictionary<string, string>>(jsonBytes) method
        //is used to convert this JSON string into a dictionary of key-value pairs
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonBytes);
        //Finally, the method iterates over the key-value pairs in the dictionary, creating a Claim object for each pair.
        //The key of the pair is used as the claim type, and the value of the pair is used as the claim value
        var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value));

        return claims;
    }
}



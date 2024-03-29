using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using TaskArchitect.Models;

namespace TaskArchitect.Services
{
    public class AuthService
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<AuthResult> Register(RegisterModel model)
        {
            try
            {
                // Check if the username or email already exists in local storage
                var existingUser = await GetUserFromLocalStorage(model.Username, model.Email);
                if (existingUser != null)
                {
                    return new AuthResult { Success = false, Error = "Username or email already exists." };
                }

                // Create a new user object
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                // Store the user object in local storage
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", user.Username, System.Text.Json.JsonSerializer.Serialize(user));

                return new AuthResult { Success = true, User = user };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Error = ex.Message };
            }
        }

        public async Task<AuthResult> Login(LoginModel model)
        {
            try
            {
                // Retrieve the user from local storage based on the username
                var user = await GetUserFromLocalStorage(model.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    return new AuthResult { Success = false, Error = "Invalid username or password." };
                }

                // Store the user object in local storage to indicate a logged-in state
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", System.Text.Json.JsonSerializer.Serialize(user));

                return new AuthResult { Success = true, User = user };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Error = ex.Message };
            }
        }

        

        public async System.Threading.Tasks.Task Logout()
        {
            // Remove the current user from local storage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
        }

        private async Task<User> GetUserFromLocalStorage(string username, string email = null)
        {
            var usersJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", username);
            if (!string.IsNullOrEmpty(usersJson))
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(usersJson);
                if (email == null || user.Email == email)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
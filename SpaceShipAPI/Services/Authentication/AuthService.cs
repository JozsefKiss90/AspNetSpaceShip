using Microsoft.AspNetCore.Identity;
using SpaceshipAPI;
using SpaceShipAPI.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    
    public AuthService(UserManager<UserEntity> userManager,ILogger<AuthService> logger, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(string email, string username, string password, string role)
    {
        var user = new UserEntity { UserName = username, Email = email };
        var result = await _userManager.CreateAsync(user, password);


        if (!result.Succeeded)
        {
            return FailedRegistration(result, email, username);
        }
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"Failed to assign role '{role}' to user '{email}'. Errors: {errors}");
            await _userManager.DeleteAsync(user);
            return new AuthResult(false, email, username, "", "Failed to assign role");
        }

        return new AuthResult(true, email, username, "", "");
    }

    private static AuthResult FailedRegistration(IdentityResult result, string email, string username)
    {
        var authResult = new AuthResult(false, email, username, "", "");

        foreach (var error in result.Errors)
        {
            authResult.ErrorMessages.Add(error.Code, error.Description);
        }

        return authResult;
    }
    
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var managedUser = await _userManager.FindByEmailAsync(email);

        if (managedUser == null)
        {
            return InvalidEmail(email);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, password);
        if (!isPasswordValid)
        {
            return InvalidPassword(email, managedUser.UserName);
        }

        var roles = await _userManager.GetRolesAsync(managedUser);
        string role = roles.FirstOrDefault(); 
        _logger.LogInformation($"User role: {role}");
        var accessToken = _tokenService.CreateToken(managedUser, role);

        return new AuthResult(true, managedUser.Email, managedUser.UserName, accessToken, role);
    }

    private static AuthResult InvalidEmail(string email)
    {
        var result = new AuthResult(false, email, "", "", "");
        result.ErrorMessages.Add("Bad credentials", "Invalid email");
        return result;
    }

    private static AuthResult InvalidPassword(string email, string userName)
    {
        var result = new AuthResult(false, email, userName, "", "");
        result.ErrorMessages.Add("Bad credentials", "Invalid password");
        return result;
    }
}
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace ModuShop.Users.Endpoints;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class Login(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager)
    : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);

        if (user is null)
        {
            AddError("Invalid credentials");
            ThrowIfAnyErrors();
            return;
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);

        if (!result.Succeeded)
        {
            AddError("Invalid credentials");
            ThrowIfAnyErrors();
            return;
        }

        Response = new LoginResponse
        {
            Success = true,
            Message = "Login successful"
        };

        await Send.OkAsync(Response, ct);
    }
}
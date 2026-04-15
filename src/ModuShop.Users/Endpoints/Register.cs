using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;

namespace ModuShop.Users.Endpoints;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? UserId { get; set; }
    public List<string>? Errors { get; set; }
}

public class Register : Endpoint<RegisterRequest, RegisterResponse>
{
    public override void Configure()
    {
        Post("/register");
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Register a new user";
            s.Description = "Creates a new user account";
        });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        Response = new RegisterResponse
        {
            Success = true,
            Message = "Registration endpoint is working! (Not yet connected to Identity)"
        };

        await Send.OkAsync(Response, ct);
    }
}
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace ModuShop.Users.Endpoints;

public class GetUserRequest
{
	public string Id { get; set; } = string.Empty;
}

public class GetUserResponse
{
	public string Id { get; set; } = string.Empty;
	public string? Email { get; set; }
	public string? UserName { get; set; }
}

public class GetUserById(UserManager<IdentityUser> userManager)
	: Endpoint<GetUserRequest, GetUserResponse>
{
	public override void Configure()
	{
		Get("/users/{id}");
		AllowAnonymous();
	}

	public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
	{
		var user = await userManager.FindByIdAsync(req.Id);

		if (user is null)
		{
			AddError("User not found");
			ThrowIfAnyErrors();
			return;
		}

		Response = new GetUserResponse
		{
			Id = user.Id,
			Email = user.Email,
			UserName = user.UserName
		};

		await Send.OkAsync(Response, ct);
	}
}
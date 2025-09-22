using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using UniverseRift.Contexts;

public class TokenMiddleware
{
	private readonly RequestDelegate _next;
	private readonly AplicationContext _context;
	public TokenMiddleware(RequestDelegate next, AplicationContext context)
	{
		_context = context;
		this._next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var token = context.Request.Query["token"];
		var cookie = context.Request.Headers.Cookie;
		var TestHeader = context.Request.Headers["TestHeader"];

		//if (token != "12345678")
		//{
		//	context.Response.StatusCode = 403;
		//	await context.Response.WriteAsync("Token is invalid");
		//}
		//else
		//{
		//}
		await _next.Invoke(context);
	}
}
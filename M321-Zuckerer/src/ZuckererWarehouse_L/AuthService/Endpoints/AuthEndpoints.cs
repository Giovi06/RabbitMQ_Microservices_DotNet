using AuthService.Helpers;
using JwtLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace AuthService.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/Auth");

            group.MapPost("/SignIn", async ([FromBody]SignInDTO dto, RemoteAuthApi remoteAuthApi, JwtHelper jwtHelper) =>
            {
                if (!await remoteAuthApi.Authenticate(dto.Username, dto.Password))
                {
                    return TypedResults.Ok(new SignInResultDTO(false, "Authentication failed", null));
                }
                var jwt = jwtHelper.GenerateJwt(dto.Username);
                return TypedResults.Ok(new SignInResultDTO(true, "OK", jwt));
            });

            group.MapPost("/CheckJwt", ([FromBody] string token, JwtHelper jwtHelper) =>
            {
                var json = jwtHelper.VerifyJwt(token);
                return TypedResults.Ok(json);
            });
        }
    }

    public record SignInDTO(string Username, string Password);
    public record SignInResultDTO(bool Success, string? Message, string? Token);
 
}

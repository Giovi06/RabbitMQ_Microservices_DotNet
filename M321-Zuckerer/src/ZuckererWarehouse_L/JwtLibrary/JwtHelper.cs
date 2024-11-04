using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json.Nodes;

namespace JwtLibrary
{
    public class JwtHelper(IOptions<JwtOptions> options)
    {
        private readonly JwtOptions _options = options.Value;

        public string GenerateJwt(string subject)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var token = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_options.Secret)
                .AddClaim("iss", _options.Issuer)
                .AddClaim("iat", now)  // issued at
                .AddClaim("nbf", now)  // not before
                .AddClaim("exp", now + _options.TTLSeconds)  // expires
                .AddClaim("sub", subject)
                .Encode();
            return token;
        }

        public string VerifyJwt(string jwtString)
        {
            var json = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_options.Secret)
                .MustVerifySignature()
                .Decode(jwtString);
            return json;
        }

        public string? GetJwtFromRequest(HttpRequest request)
        {
            var parts = request.Headers["Authorization"].FirstOrDefault()?.Split(" ");
            if (parts == null) return null;
            if (parts.Length != 2) return null;
            if (!parts[0].Equals("bearer", StringComparison.CurrentCultureIgnoreCase)) return null;
            var json = VerifyJwt(parts[1]);
            return json;
        }

        public string? GetSubjectFromRequest(HttpRequest request)
        {
            var json = GetJwtFromRequest(request);
            if (json == null) return null;
            var jsonObj = JsonNode.Parse(json);
            return jsonObj?["sub"]?.ToString();
        }
    }

    public static class JwtHelpersExtension
    {
        public static void AddJwtHelpers(this IServiceCollection services)
        {
            services.AddSingleton<JwtHelper>();
        }
    }
}

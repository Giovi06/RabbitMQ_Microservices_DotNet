using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace JwtLibrary
{
    public class JwtMiddleware<TSubject>(RequestDelegate next, JwtHelper jwtHelper)
    {
        private readonly JwtHelper _jwtHelper = jwtHelper;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IJwtSubjectLookup<TSubject> subjectLookup)
        {
            try
            {
                var subject = _jwtHelper.GetSubjectFromRequest(context.Request);

                if (subject != null)
                {
                    context.Items["jwt.subject"] = subject;
                    context.Items["subject"] = await subjectLookup.LookupAsync(subject);
                }
            }
            catch
            {
                // ignore errors in getting the JWT and/or verification
                // context.Items["jwt.subject"] and context.Items["subject"] will not be populated --> request not authenticated
            }

            await _next(context);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware<TSubject>(this IApplicationBuilder app) => app.UseMiddleware<JwtMiddleware<TSubject>>();
    }
}

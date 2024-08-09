using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace HackerNewsApi.Caching;

public class CacheControlAttribute(int minutes) : Attribute, IAsyncActionFilter
{
    private readonly TimeSpan _maxAge = TimeSpan.FromMinutes(minutes);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        resultContext.HttpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = _maxAge
        };
        resultContext.HttpContext.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
    }
}
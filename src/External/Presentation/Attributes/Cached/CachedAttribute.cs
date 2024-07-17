using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;

namespace Presentation.Attributes.Cached
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute(int expirationTime = 5) : Attribute, IAsyncActionFilter
    {
        private readonly int _expirationTime = expirationTime;

        public static readonly List<string> KeyList = [];

        public static readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

        private static int FaturaId = 0;

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            var faturaId = (int)(context.HttpContext.Items["GrupoFaturaId"] ?? 0);
            if (FaturaId != faturaId)
            {
                FaturaId = faturaId;
                CleanCache();
            }

            if (context.HttpContext.Request.Method != "GET")
            {
                CleanCache();
                await next();
            }
            else if (
                _memoryCache.TryGetValue(
                    CreateCacheKey(context.HttpContext.Request),
                    out IActionResult cachedResult
                )
            )
            {
                context.Result = cachedResult;
            }
            else
            {
                if (!ShouldCache(context.HttpContext.Request))
                {
                    await next();
                }
                SaveResponseToCache(await next());
            }
        }

        private void SaveResponseToCache(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult okResult)
            {
                string cacheKey = CreateCacheKey(context.HttpContext.Request);

                _memoryCache.Set(
                    cacheKey,
                    okResult,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                        TimeSpan.FromMinutes(_expirationTime)
                    )
                );

                KeyList.Add(cacheKey);
            }
        }

        private static string CreateCacheKey(HttpRequest request)
        {
            string fullEncodedUrl = $"{request.GetEncodedUrl()}/{request.QueryString.Value}";
            string route =
                $"{request.Method}/{request.HttpContext.GetRouteData().Values.FirstOrDefault()}";

            return $"{fullEncodedUrl}{route}";
        }

        private static bool ShouldCache(HttpRequest request)
        {
            string path = request.Path.ToString();

            if (path.Contains("despesa/por-grupo") || path.Contains("despesa/todos-grupos"))
            {
                var query = request.Query;
                if (query.ContainsKey("filter") && !string.IsNullOrWhiteSpace(query["filter"]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void CleanCache()
        {
            foreach (var key in KeyList)
            {
                _memoryCache.Remove(key);
            }

            KeyList.Clear();
        }
    }
}

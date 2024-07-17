using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Attributes.Cached
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ClearCacheAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            CleanCache();
            await next();
        }

        private static void CleanCache()
        {
            foreach (var key in CachedAttribute.KeyList)
            {
                CachedAttribute._memoryCache.Remove(key);
            }

            CachedAttribute.KeyList.Clear();
        }
    }
}

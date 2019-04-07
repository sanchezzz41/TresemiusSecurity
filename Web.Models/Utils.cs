using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Web.Models
{
    public static class Utils
    {
        public static string GetLogin(this HttpContext context)
        {
            return context.User.Identity.Name;
        }

        public static string GetKey2Laba(this IMemoryCache memoryCache, string user)
        {
            return memoryCache.Get<string>(user);
        }
    }
}

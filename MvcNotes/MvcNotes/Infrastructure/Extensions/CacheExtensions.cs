using MvcNotes.Infrastructure.Cache;

namespace MvcNotes.Infrastructure
{
    public static class ICacheExtensions
    {
        public static T Get<T>(this ICache cache, string key) where T : class
        {
            return cache.Get(key) as T;
        }
    }
}
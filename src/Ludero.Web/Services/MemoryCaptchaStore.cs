using Ixnas.AltchaNet;
using Microsoft.Extensions.Caching.Memory;

namespace Ludero.Web.Services
{
    public class MemoryCaptchaStore(IMemoryCache cache) : IAltchaChallengeStore
    {
        private readonly IMemoryCache _cache = cache;

        public Task Store(string challenge, DateTimeOffset expiryUtc)
        {
            _cache.Set(challenge, true, expiryUtc);
            return Task.CompletedTask;
        }

        public Task<bool> Exists(string challenge)
        {
            var exists = _cache.TryGetValue(challenge, out _);
            return Task.FromResult(exists);
        }
    }
}

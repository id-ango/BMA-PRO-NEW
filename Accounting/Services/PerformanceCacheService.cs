using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Services
{
    /// <summary>
    /// Service untuk memory caching dengan TTL (Time To Live)
    /// Mengurangi database queries dan mempercepat response time
    /// </summary>
    public interface IPerformanceCacheService
    {
        T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? duration = null);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null);
        void Remove(string key);
        void RemoveByPattern(string keyPattern);
    }

    public class PerformanceCacheService : IPerformanceCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly int _defaultDurationMinutes;

        public PerformanceCacheService(IMemoryCache cache)
        {
            _cache = cache;
            _defaultDurationMinutes = 30; // Default 30 menit
        }

        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? duration = null)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            var value = factory();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(duration ?? TimeSpan.FromMinutes(_defaultDurationMinutes));

            _cache.Set(key, value, cacheOptions);
            return value;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            var value = await factory();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(duration ?? TimeSpan.FromMinutes(_defaultDurationMinutes));

            _cache.Set(key, value, cacheOptions);
            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveByPattern(string keyPattern)
        {
            // Note: IMemoryCache tidak support pattern removal
            // Harus implement custom tracking atau gunakan Redis untuk pattern removal
            // Untuk sekarang, hapus key spesifik yang diketahui
        }
    }
}

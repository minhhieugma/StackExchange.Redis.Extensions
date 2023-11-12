namespace StackExchange.Redis.Extensions;

public class RedisClient
{
    private ConnectionMultiplexer redis;
    private IDatabase db;

    public TimeSpan? DefaultExpiry { get; set; } = TimeSpan.FromMinutes(5);

    public async Task ConnectAsync(string connectionString)
    {
        (ConnectionMultiplexer Redis, IDatabase Db) result = await RedisExtensions.ConnectAsync(connectionString);

        redis = result.Redis;
        db = result.Db;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        return await db.GetAsync<T>(key);
    }

    public async Task<long> EvictAsync(params string[] keys)
    {
        return await db.EvictAsync(keys);
    }

    public async Task<long> EvictByPatternAsync(string pattern)
    {
        return await db.EvictByPatternAsync(pattern);
    }

    public async Task<long> EvictByTagsAsync(params string[] tags)
    {
        return await db.EvictByTagsAsync(tags);
    }

    public async Task<bool> SetAsync<T>(string key, T value, string[]? tags = null, TimeSpan? expiry = null)
    {
        return await db.SetAsync<T>(key, value, tags, expiry ?? DefaultExpiry);
    }
}
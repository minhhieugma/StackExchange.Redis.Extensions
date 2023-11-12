using System.Text.Json;
using System.Text.RegularExpressions;

namespace StackExchange.Redis.Extensions;

public static class RedisExtensions
{
    public static async Task<(ConnectionMultiplexer Redis, IDatabase Db)> ConnectAsync(string connectionString)
    {
        ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(connectionString);

        IDatabase db = redis.GetDatabase();

        return (redis, db);
    }

    public static async Task<T?> GetAsync<T>(this IDatabase db, string key)
    {
        using MemoryStream mem = new();

        RedisValue redisValue = await db.StringGetAsync(key);
        if (redisValue.HasValue == false)
            return default;

        return JsonSerializer.Deserialize<T>(redisValue!);
    }

    public static async Task<long> EvictAsync(this IDatabase db, params string[] keys)
    {
        return await db.KeyDeleteAsync(keys.Select(p => (RedisKey)p).ToArray(), CommandFlags.None);
    }


    public static async Task<long> EvictByPatternAsync(this IDatabase db, string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return 0;

        RedisResult result = await db.ExecuteAsync($"KEYS", pattern);

        List<string> keys = new();
        for (int i = 0; i < result.Length; i++)
        {
            keys.Add(result[i].ToString());
        }

        return await db.KeyDeleteAsync(keys.Select(p => (RedisKey)p).ToArray());

        static bool IsValidRegexPattern(string pattern, string testText = "", int maxSecondTimeOut = 1)
        {
            if (string.IsNullOrEmpty(pattern)) return false;
            Regex re = new(pattern, RegexOptions.None, new TimeSpan(0, 0, maxSecondTimeOut));
            try
            {
                _ = re.IsMatch(testText);
            }
            catch
            {
                return false;
            } //ArgumentException or RegexMatchTimeoutException

            return true;
        }
    }


    public static async Task<long> EvictByTagsAsync(this IDatabase db, params string[] tags)
    {
        List<RedisValue> keys = new();
        foreach (string? tag in tags)
        {
            RedisValue[]? tmp = await db.SetMembersAsync(tag);

            keys.AddRange(tmp);
            keys.Add(tag);
        }

        RedisKey[]? removingKeys = keys.Select(p => (RedisKey)p.ToString()).ToArray();

        return await db.KeyDeleteAsync(removingKeys, CommandFlags.None);
    }

    public static async Task<bool> SetAsync<T>(this IDatabase db, string key, T value, string[]? tags = null,
        TimeSpan? expiry = null)
    {
        using MemoryStream mem = new();
        await JsonSerializer.SerializeAsync(mem, value);

        ITransaction tran = db.CreateTransaction();

        foreach (string? tag in tags ?? Array.Empty<string>())
        {
            _ = tran.SetAddAsync(tag, key);
        }

        Task<bool> result = tran.StringSetAsync(key, mem.ToArray(), expiry);

        await tran.ExecuteAsync();

        return result.Result;
    }
}
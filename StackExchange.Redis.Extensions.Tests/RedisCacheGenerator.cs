using FluentAssertions;
using Redis.Extensions.Tests.Models;
using StackExchange.Redis.Extensions;

namespace Redis.Extensions.Tests;

public sealed class RedisCacheGenerator
{
    public const string ConnectionString = "localhost:6379,user=default,password=mypassword";

    [Fact]
    public async Task Generate()
    {
        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        foreach (int i in Enumerable.Range(0, 5000))
        {
            string key = $"USER:{i}";
            User user = new User
            {
                FirstName = $"Hieu {i}",
                LastName = $"Le {i}",
                CreatedDate = new DateTime(2020, 06, 01),
                Payload = User.CreateUsers(2000)
            };
            await client.SetAsync(key, user, expiry: TimeSpan.FromDays(60));
        }
        
        foreach (int i in Enumerable.Range(0, 5000))
        {
            string key = $"PERMISSIONS:{i}";
            Permission permission = new Permission
            {
                Id = i,
                Name = $"Permission {i}",
                Name2 = $"Permission {i}",
                Name3 = $"Permission {i}",
                Name4 = $"Permission {i}",
                Name5 = $"Permission {i}",
                Name6 = $"Permission {i}",
                CreatedDate = new DateTime(2020, 06, 01),
                Payload = User.CreateUsers(1000)
            };
            await client.SetAsync(key, permission, expiry: TimeSpan.FromDays(60));
        }
    }

}
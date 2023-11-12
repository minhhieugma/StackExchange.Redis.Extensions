using FluentAssertions;
using Redis.Extensions.Tests.Models;
using StackExchange.Redis;
using StackExchange.Redis.Extensions;

namespace Redis.Extensions.Tests;

public class RedisExtensionsTests
{
    public const string ConnectionString = "localhost:6379,user=default,password=mypassword";

    [Fact]
    public async Task Test1()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        await db.SetAsync("aaaa", 123);

        int val = await db.GetAsync<int>("aaaa");

        val.Should().Be(123);
    }

    [Fact]
    public async Task Evivt()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        await db.SetAsync("aaaa", 123);
        await db.EvictAsync("aaaa");

        int val = await db.GetAsync<int>("aaaa");

        val.Should().Be(0);
    }

    [Fact]
    public async Task NotFound_1()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        int? val = await db.GetAsync<int?>(Guid.NewGuid().ToString());
        val.Should().BeNull();
    }

    [Fact]
    public async Task NotFound_2()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        int? val = await db.GetAsync<int>(Guid.NewGuid().ToString());
        val.Should().Be(0);
    }

    [Fact]
    public async Task Set_Get_Object()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        User user = new()
        {
            FirstName = "Hieu",
            LastName = "Le",
            CreatedDate = new DateTime(2020, 06, 01)
        };

        await db.SetAsync("user1", user);
        User? val = await db.GetAsync<User>("user1");

        val.Should().NotBeNull();
        val.FirstName.Should().Be(user.FirstName);
        val.LastName.Should().Be(user.LastName);
        val.CreatedDate.Should().Be(user.CreatedDate);
    }


    [Fact]
    public async Task EvictByMatching()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        await db.SetAsync("aaaa", 123);
        await db.EvictByPatternAsync("*");

        int? val = await db.GetAsync<int?>("aaaa");

        val.Should().BeNull();
    }

    [Fact]
    public async Task EvictByMatching_Empty()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        await db.SetAsync("aaaa", 123);
        long result = await db.EvictByPatternAsync("");

        result.Should().Be(0);
    }

    [Fact]
    public async Task Tag()
    {
        (ConnectionMultiplexer _, IDatabase db) = await RedisExtensions.ConnectAsync(ConnectionString);

        string[] tagNames = new[] { "tag 1", "tag 2" };

        await db.SetAsync("aaaa", 123, tagNames);
        await db.EvictByTagsAsync(tagNames);

        int? val = await db.GetAsync<int?>("aaaa");

        val.Should().BeNull();
    }
}
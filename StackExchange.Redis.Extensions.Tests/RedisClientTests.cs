using FluentAssertions;
using Redis.Extensions.Tests.Models;
using StackExchange.Redis.Extensions;

namespace Redis.Extensions.Tests;

public class RedisClientTests
{
    public const string ConnectionString = "localhost:6379,user=default,password=mypassword";

    [Fact]
    public async Task Test1()
    {
        //RedisExtensions.Create();

        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        await client.SetAsync("aaaa", 123);

        int val = await client.GetAsync<int>("aaaa");

        val.Should().Be(123);
    }

    [Fact]
    public async Task Evivt()
    {
        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        await client.SetAsync("aaaa", 123);
        await client.EvictAsync("aaaa");

        int val = await client.GetAsync<int>("aaaa");

        val.Should().Be(0);
    }

    [Fact]
    public async Task NotFound_1()
    {
        //RedisExtensions.Create();

        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        int? val = await client.GetAsync<int?>(Guid.NewGuid().ToString());
        val.Should().BeNull();
    }

    [Fact]
    public async Task NotFound_2()
    {
        //RedisExtensions.Create();

        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        int? val = await client.GetAsync<int>(Guid.NewGuid().ToString());
        val.Should().Be(0);
    }

    [Fact]
    public async Task Set_Get_Object()
    {
        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        User user = new()
        {
            FirstName = "Hieu",
            LastName = "Le",
            CreatedDate = new DateTime(2020, 06, 01)
        };

        await client.SetAsync("user1", user);
        User? val = await client.GetAsync<User>("user1");

        val.Should().NotBeNull();
        val.FirstName.Should().Be(user.FirstName);
        val.LastName.Should().Be(user.LastName);
        val.CreatedDate.Should().Be(user.CreatedDate);
    }

    [Fact]
    public async Task EvictByMatching()
    {
        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        await client.SetAsync("aaaa", 123);
        await client.EvictByPatternAsync("*");

        int? val = await client.GetAsync<int?>("aaaa");

        val.Should().BeNull();
    }

    [Fact]
    public async Task EvictByMatching_Empty()
    {
        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        await client.SetAsync("aaaa", 123);
        long result = await client.EvictByPatternAsync("");

        result.Should().Be(0);
    }

    [Fact]
    public async Task Tag()
    {
        RedisClient client = new();
        await client.ConnectAsync(ConnectionString);

        string[] tagNames = new[] { "tag 1", "tag 2" };

        await client.SetAsync("aaaa", 123, tagNames);
        await client.EvictByTagsAsync(tagNames);

        int? val = await client.GetAsync<int?>("aaaa");

        val.Should().BeNull();
    }
}
namespace Redis.Extensions.Tests.Models;

public class User
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime CreatedDate { get; set; }
    
    // Random data to make the object bigger
    public List<User> Payload { get; set; } = new();
    
    public static List<User> CreateUsers(int count)
    {
        return Enumerable.Range(0, count).Select(i => new User
        {
            FirstName = $"Hieu {i}",
            LastName = $"Le {i}",
            CreatedDate = new DateTime(2020, 06, 01),
            Payload = new List<User>()
        }).ToList();
    }
}
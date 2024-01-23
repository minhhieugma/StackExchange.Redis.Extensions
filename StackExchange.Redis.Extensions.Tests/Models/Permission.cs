namespace Redis.Extensions.Tests.Models;

public sealed class Permission
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    
    public string Name2 { get; set; }
    public string Name3 { get; set; }
    public string Name4 { get; set; }
    public string Name5 { get; set; }
    public string Name6 { get; set; }
    
    public DateTime CreatedDate { get; set; }  
    
    // Random data to make the object bigger
    public List<User> Payload { get; set; } = new();

}
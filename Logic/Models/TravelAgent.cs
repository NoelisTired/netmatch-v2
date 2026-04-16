using System;

namespace Logic.Models;

public class Travelagent
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }

    public Travelagent(int id, string name, string email, string password)
    {
        this.Id = id;
        this.Name = name;
        this.Email = email;
        this.Password = password;
    }
}
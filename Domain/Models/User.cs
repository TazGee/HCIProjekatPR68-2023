using Domain.Enums;

namespace Domain.Models
{
    public class User
    {
        public long Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRoles Role { get; set; } = UserRoles.Visitor;

        public User() { }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public User(string username, string password, UserRoles role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
        public User(long id, string username, string password, UserRoles role)
        {
            Id = id;
            Username = username;
            Password = password;
            Role = role;
        }
    }
}

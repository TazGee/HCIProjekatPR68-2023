namespace Domain.Models
{
    public class User
    {
        public long Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Admin { get; set; } = false;

        public User() { }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public User(string username, string password, bool admin)
        {
            Username = username;
            Password = password;
            Admin = admin;
        }
        public User(long id, string username, string password, bool admin)
        {
            Id = id;
            Username = username;
            Password = password;
            Admin = admin;
        }
    }
}

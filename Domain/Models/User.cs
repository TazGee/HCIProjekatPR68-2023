namespace Domain.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Admin { get; set; }

        public User() { }

        public User(long id, string username, string password, bool admin)
        {
            Id = id;
            Username = username;
            Password = password;
            Admin = admin;
        }
    }
}

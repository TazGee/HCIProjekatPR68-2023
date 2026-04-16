using Domain.Models;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        public User AddUser(User user);
        public User FindUserUsingUsername(string username);
        public IEnumerable<User> AllUsers();
    }
}

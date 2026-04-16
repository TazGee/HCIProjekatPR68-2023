using Domain.Database;
using Domain.Models;
using Domain.Repositories;

namespace Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        IDataBase database;

        public UserRepository(IDataBase db)
        {
            database = db;
        }

        public User AddUser(User user)
        {
            try
            {
                User postoji = FindUserUsingUsername(user.Username);

                if (postoji.Username == string.Empty)
                {
                    user.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    database.Tabele.Users.Add(user);

                    database.SaveChanges();

                    return user;
                }

                return new User();
            }
            catch
            {
                return new User();
            }
        }

        public User FindUserUsingUsername(string username)
        {
            try
            {
                foreach (User user in database.Tabele.Users)
                {
                    if (user.Username == username)
                        return user;
                }

                return new User();
            }
            catch
            {
                return new User();
            }
        }

        public IEnumerable<User> AllUsers()
        {
            try
            {
                return database.Tabele.Users;
            }
            catch
            {
                return [];
            }
        }
    }
}

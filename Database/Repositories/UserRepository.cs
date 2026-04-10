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

        public User AddUser(User korisnik)
        {
            try
            {
                User postoji = FindUserUsingUsername(korisnik.Username);

                if (postoji.Username == string.Empty)
                {
                    korisnik.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    database.Tabele.Users.Add(korisnik);

                    database.SaveChanges();

                    return korisnik;
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
                foreach (User korisnik in database.Tabele.Users)
                {
                    if (korisnik.Username == username)
                        return korisnik;
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
            catch (Exception ex)
            {
                return [];
            }
        }
    }
}

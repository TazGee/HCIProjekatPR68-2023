using Domain.Models;
using Domain.Repositories;
using Domain.Services;

namespace Services.AuthService
{
    public class AuthService : IAuthService
    {
        readonly IUserRepository UserRepository;

        public AuthService(IUserRepository userRepo)
        {
            UserRepository = userRepo;
        }

        public (bool, User) Login(string korisnickoIme, string lozinka)
        {
            User found = UserRepository.FindUserUsingUsername(korisnickoIme);

            if (found.Username != string.Empty && found.Password == lozinka)
            {
                return (true, found);
            }
            else
            {
                return (false, new User());
            }
        }

        public (bool, User) Register(User newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.Password))
            {
                return (false, new User());
            }

            User found = UserRepository.FindUserUsingUsername(newUser.Username);

            if (found.Username == string.Empty)
            {
                User added = UserRepository.AddUser(newUser);

                if (added.Username != string.Empty)
                {
                    return (true, added);
                }
            }

            return (false, new User());
        }
    }
}

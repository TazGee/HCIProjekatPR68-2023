using Domain.Models;

namespace Domain.Services
{
    public interface IAuthService
    {
        public (bool, User) Login(string username, string password);
        public (bool, User) Register(User newUser);
    }
}

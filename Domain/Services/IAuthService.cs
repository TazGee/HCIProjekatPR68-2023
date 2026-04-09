using Domain.Models;

namespace Domain.Services
{
    public interface IAuthService
    {
        public (bool, User) Prijava(string korisnickoIme, string lozinka);
        public (bool, User) Registracija(User noviKorisnik);
    }
}

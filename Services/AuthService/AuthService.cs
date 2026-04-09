using Domain.Models;
using Domain.Repositories;
using Domain.Services;

namespace Services.AuthService
{
    public class AuthService : IAuthService
    {
        readonly IUserRepository KorisniciRepozitorijum;

        public AuthService(IUserRepository korisniciRepozitorijum)
        {
            KorisniciRepozitorijum = korisniciRepozitorijum;
        }

        public (bool, User) Prijava(string korisnickoIme, string lozinka)
        {
            User pronadjen = KorisniciRepozitorijum.FindUserUsingUsername(korisnickoIme);

            if (pronadjen.Username != string.Empty && pronadjen.Password == lozinka)
            {
                return (true, pronadjen);
            }
            else
            {
                return (false, new User());
            }
        }

        public (bool, User) Registracija(User noviKorisnik)
        {
            if (noviKorisnik == null || string.IsNullOrWhiteSpace(noviKorisnik.Username) || string.IsNullOrWhiteSpace(noviKorisnik.Password))
            {
                return (false, new User());
            }

            User pronadjen = KorisniciRepozitorijum.FindUserUsingUsername(noviKorisnik.Username);

            if (pronadjen.Username == string.Empty)
            {
                return (true, noviKorisnik);
            }
            else
            {
                return (false, new User());
            }
        }
    }
}

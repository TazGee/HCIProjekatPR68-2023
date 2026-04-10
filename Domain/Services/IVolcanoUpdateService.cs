using Domain.Models;

namespace Domain.Services
{
    public interface IVolcanoUpdateService
    {
        bool UpdateVolcano(Volcano vol, string name, string drzava, string visina);
    }
}

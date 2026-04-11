using Domain.Models;
using Domain.Repositories;

namespace Domain.Services
{
    public interface IAddVolcanoService
    {
        public bool AddVolcano(Volcano volcano);
    }
}

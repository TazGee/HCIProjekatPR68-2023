using Domain.Models;

namespace Domain.Repositories
{
    public interface IVolcanoRepository
    {
        public Volcano AddVolcano(Volcano vol);
        public bool DeleteVolcano(string volanoName);
        public Volcano FindVolcanoUsingName(string name);
        public IEnumerable<Volcano> AllVolcanoes();
    }
}

using Domain.Repositories;
using Domain.Services;

namespace Services.VolcanoDeleteService
{
    public class VolcanoDeleteService : IVolcanoDeleteService
    {
        IVolcanoRepository volcanoRepository;

        public VolcanoDeleteService (IVolcanoRepository volcanoRepository)
        {
            this.volcanoRepository = volcanoRepository;
        }

        public bool DeleteVolcano(string volcanoName)
        {
            return volcanoRepository.DeleteVolcano(volcanoName);
        }
    }
}

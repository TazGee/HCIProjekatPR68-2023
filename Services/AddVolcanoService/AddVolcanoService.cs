using Domain.Models;
using Domain.Repositories;
using Domain.Services;

namespace Services.AddVolcanoService
{
    public class AddVolcanoService : IAddVolcanoService
    {
        IVolcanoRepository volcanoRepo;

        public AddVolcanoService(IVolcanoRepository volcanoRepo)
        {
            this.volcanoRepo = volcanoRepo; 
        }

        public bool AddVolcano(Volcano volcano)
        {
            try
            {
                volcanoRepo.AddVolcano(volcano);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

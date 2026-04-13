using Domain.Database;
using Domain.Repositories;
using Domain.Services;

namespace Services.VolcanoDeleteService
{
    public class VolcanoDeleteService : IVolcanoDeleteService
    {
        IVolcanoRepository volcanoRepository;
        IDataBase database;

        public VolcanoDeleteService (IVolcanoRepository volcanoRepository, IDataBase database)
        {
            this.volcanoRepository = volcanoRepository;
            this.database = database;
        }

        public bool DeleteVolcano(string volcanoName)
        {
            if (!volcanoRepository.DeleteVolcano(volcanoName)) return false;
            return database.SaveChanges();
        }
    }
}

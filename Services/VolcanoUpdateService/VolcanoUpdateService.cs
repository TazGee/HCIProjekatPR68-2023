using Domain.Database;
using Domain.Models;
using Domain.Services;

namespace Services.VolcanoUpdateService
{
    public class VolcanoUpdateService : IVolcanoUpdateService
    {
        IDataBase database;

        public VolcanoUpdateService(IDataBase database) 
        {
            this.database = database;
        }

        public bool UpdateVolcano(Volcano vol, string name, string country, string height, string photoPath)
        {
            try
            {
                vol.Name = name;
                vol.Country = country;
                vol.AddTime = DateTime.UtcNow;
                vol.Height = int.Parse(height);
                vol.PhotoPath = photoPath;

                database.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

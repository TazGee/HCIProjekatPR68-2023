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

        public bool UpdateVolcano(Volcano vol, string name, string drzava, string visina, string photoPath)
        {
            try
            {
                vol.NazivVulkana = name;
                vol.Drzava = drzava;
                vol.DatumDodavanja = DateTime.UtcNow;
                vol.Visina = int.Parse(visina);
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

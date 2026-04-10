using Domain.Database;
using Domain.Models;
using Domain.Repositories;
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

        public bool UpdateVolcano(Volcano vol, string name, string drzava, string visina)
        {
            try
            {
                vol.NazivVulkana = name;
                vol.Drzava = drzava;
                vol.DatumDodavanja = DateTime.UtcNow;
                vol.Visina = int.Parse(visina);

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

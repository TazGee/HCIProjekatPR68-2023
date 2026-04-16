using Domain.Database;
using Domain.Models;
using Domain.Repositories;

namespace Database.Repositories
{
    public class VolcanoRepository : IVolcanoRepository
    {
        IDataBase database;

        public VolcanoRepository(IDataBase db)
        {
            database = db;
        }

        public Volcano AddVolcano(Volcano vol)
        {
            try
            {
                Volcano volcano = FindVolcanoUsingName(vol.Name);

                if (volcano.Name == string.Empty)
                {
                    vol.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    database.Tabele.Volcanoes.Add(vol);

                    database.SaveChanges();

                    return vol;
                }

                return new Volcano();
            }
            catch
            {
                return new Volcano();
            }
        }
        public bool DeleteVolcano(string volanoName)
        {
            try
            {
                Volcano volcano = FindVolcanoUsingName(volanoName);

                if (volcano.Name != string.Empty)
                {
                    database.Tabele.Volcanoes.Remove(volcano);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public Volcano FindVolcanoUsingName(string name)
        {
            try
            {
                foreach (Volcano v in database.Tabele.Volcanoes)
                {
                    if (v.Name == name)
                        return v;
                }

                return new Volcano();
            }
            catch
            {
                return new Volcano();
            }
        }

        public IEnumerable<Volcano> AllVolcanoes()
        {
            try
            {
                return database.Tabele.Volcanoes;
            }
            catch
            {
                return [];
            }
        }
    }
}

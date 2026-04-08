using Domain.Database;
using Newtonsoft.Json;

namespace Database.DataBase
{
    public class JSONDataBase : IDataBase
    {
        const string JSONFileName = "jsondatabase.json";

        public DataBaseTable Tabele { get; set; }

        public JSONDataBase()
        {
            try
            {
                if (File.Exists(JSONFileName))
                {
                    using StreamReader sr = new(JSONFileName);
                    string json = sr.ReadToEnd();
                    Tabele = JsonConvert.DeserializeObject<DataBaseTable>(json) ?? new();
                }
                else
                    Tabele = new();
            }
            catch
            {
                Tabele = new();
            }
        }

        public bool SacuvajPromene()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Tabele,
                              new JsonSerializerSettings
                              {
                                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                  Formatting = Newtonsoft.Json.Formatting.Indented
                              });
                using StreamWriter sw = new(JSONFileName);
                sw.Write(json);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

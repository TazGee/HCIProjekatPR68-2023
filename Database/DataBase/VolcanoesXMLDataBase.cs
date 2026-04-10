using Domain.Database;
using System.Xml.Serialization;

namespace Database.DataBase
{
    public class VolcanoesXMLDataBase : IDataBase
    {
        const string XMLFileName = @"Data\vulcans.xml";

        public DataBaseTable Tabele { get; set; }

        public VolcanoesXMLDataBase()
        {
            try
            {
                if (File.Exists(XMLFileName))
                {
                    XmlSerializer serializer = new(typeof(DataBaseTable));

                    using FileStream fs = new(XMLFileName, FileMode.Open);
                    Tabele = (DataBaseTable?)serializer.Deserialize(fs) ?? new();
                }
                else
                {
                    Tabele = new();
                    SaveChanges();
                }
            }
            catch
            {
                Tabele = new();
            }
        }

        public bool SaveChanges()
        {
            try
            {
                XmlSerializer serializer = new(typeof(DataBaseTable));

                using FileStream fs = new(XMLFileName, FileMode.Create);
                serializer.Serialize(fs, Tabele);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

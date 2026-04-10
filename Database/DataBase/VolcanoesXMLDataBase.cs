using Domain.Database;
using System.Xml.Serialization;

namespace Database.DataBase
{
    public class VolcanoesXMLDataBase : IDataBase
    {
        const string XMLFileName = @"..\..\..\Data\volcanoes.xml";

        public DataBaseTable Tabele { get; set; }

        public VolcanoesXMLDataBase()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XMLFileName);
                string? directory = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(fullPath))
                {
                    XmlSerializer serializer = new(typeof(DataBaseTable));

                    using FileStream fs = new(fullPath, FileMode.Open);
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
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XMLFileName);

                XmlSerializer serializer = new(typeof(DataBaseTable));

                using FileStream fs = new(fullPath, FileMode.Create);
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

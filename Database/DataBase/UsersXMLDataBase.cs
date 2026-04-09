using Domain.Database;
using System.Xml.Serialization;

namespace Database.DataBase
{
    public class UsersXMLDataBase : IDataBase
    {
        const string XMLFileName = @"Data\users.xml";

        public DataBaseTable Tabele { get; set; }

        public UsersXMLDataBase()
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

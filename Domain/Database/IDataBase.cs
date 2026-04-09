namespace Domain.Database
{
    public interface IDataBase
    {
        public DataBaseTable Tabele { get; set; }

        public bool SaveChanges();
    }
}

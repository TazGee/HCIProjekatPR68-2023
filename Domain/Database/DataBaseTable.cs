using Domain.Models;

namespace Domain.Database
{
    public class DataBaseTable
    {
        public List<User> Users { get; set; } = [];
        public List<Volcano> Volcanoes { get; set; } = [];

        public DataBaseTable() { }
    }
}

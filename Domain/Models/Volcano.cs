namespace Domain.Models
{
    public class Volcano
    {
        public long Id { get; set; } = 0;
        public string NazivVulkana { get; set; } = string.Empty;
        public string Drzava { get; set; } = string.Empty;
        public int Visina { get; set; } = 0;

        public Volcano() { }
        public Volcano(string naziv, string drzava, int visina)
        {
            NazivVulkana = naziv;
            Drzava = drzava;
            Visina = visina;
        }
        public Volcano(long id, string naziv, string drzava, int visina)
        {
            Id = id;
            NazivVulkana = naziv;
            Drzava = drzava;
            Visina = visina;
        }
    }
}

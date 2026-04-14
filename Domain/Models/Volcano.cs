using System;
using System.ComponentModel;

namespace Domain.Models
{
    public class Volcano : INotifyPropertyChanged
    {
        public long Id { get; set; } = 0;
        public string NazivVulkana { get; set; } = string.Empty;
        public string Drzava { get; set; } = string.Empty;
        public int Visina { get; set; } = 0;
        public string PhotoPath { get; set; } = string.Empty;
        public string RTFPath { get; set; } = string.Empty;
        public DateTime DatumDodavanja { get; set; } = DateTime.UtcNow;
        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                PromenjenChecked(nameof(IsSelected));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        void PromenjenChecked(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Volcano() { }
        public Volcano(string naziv, string drzava, int visina, string photopath, string rtfpath, DateTime datetime)
        {
            NazivVulkana = naziv;
            Drzava = drzava;
            Visina = visina;
            PhotoPath = photopath;
            RTFPath = rtfpath;
            DatumDodavanja = datetime;
        }
        public Volcano(long id, string naziv, string drzava, int visina, string photopath, string rtfpath, DateTime datetime)
        {
            Id = id;
            NazivVulkana = naziv;
            Drzava = drzava;
            Visina = visina;
            PhotoPath = photopath;
            RTFPath = rtfpath;
            DatumDodavanja = datetime;
        }
    }
}

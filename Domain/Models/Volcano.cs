using System;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace Domain.Models
{
    public class Volcano : INotifyPropertyChanged
    {
        public long Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int Height { get; set; } = 0;
        public string PhotoPath { get; set; } = string.Empty;
        public string LocalPhotoPath {  get; set; } = string.Empty;
        public string RTFPath { get; set; } = string.Empty;
        public DateTime AddTime { get; set; } = DateTime.UtcNow;
        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                CheckedChanged(nameof(IsSelected));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        void CheckedChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Volcano() { }
        public Volcano(string name, string country, int height, string photopath, string rtfpath, DateTime addTime)
        {
            Name = name;
            Country = country;
            Height = height;
            PhotoPath = photopath;
            RTFPath = rtfpath;
            AddTime = addTime;
        }
        public Volcano(long id, string name, string country, int height, string photopath, string rtfpath, DateTime addTime)
        {
            Id = id;
            Name = name;
            Country = country;
            Height = height;
            PhotoPath = photopath;
            RTFPath = rtfpath;
            AddTime = addTime;
        }
    }
}

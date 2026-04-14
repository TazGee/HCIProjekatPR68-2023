using Domain.Models;
using Domain.Services;
using Microsoft.Win32;
using Services.SetPhotoService;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Presentation
{
    public partial class DodajVulkan : Window
    {
        IAddVolcanoService addVolcanoService;
        IStorePhotoService storePhotoService;
        IStoreRTFService storeRTFService;

        ListWindow lw;

        string photoPath = String.Empty;
        string rtfPath = String.Empty;

        public DodajVulkan(IAddVolcanoService addVolcanoService, ListWindow lw, IStorePhotoService storePhotoService, IStoreRTFService storeRTFService)
        {
            this.addVolcanoService = addVolcanoService;
            this.lw = lw;
            this.storePhotoService = storePhotoService;
            this.storeRTFService = storeRTFService;

            InitializeComponent();
        }


        private void AddVulkan(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) return;
            if (!KreirajRTF()) return;

            if (addVolcanoService.AddVolcano(new Volcano(NazivVulkana.Text, Drzava.Text, int.Parse(Visina.Text), photoPath, rtfPath, DateTime.UtcNow)))
            {
                MessageBox.Show("Uspesno ste dodali vulkan!", "Uspesno!", MessageBoxButton.OK, MessageBoxImage.Information);

                lw.AzurirajListuVulkana();

                Close();
            }
            else
            {
                MessageBox.Show("Greska prilikom dodavanja vulkana!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        bool CheckInput()
        {
            if (NazivVulkana.Text == "") { MessageBox.Show("Morate popuniti polje za naziv!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (Drzava.Text == "") { MessageBox.Show("Morate popuniti polje za drzavu!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (Visina.Text == "") { MessageBox.Show("Morate popuniti polje za visinu!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

            if(photoPath == String.Empty || photoPath == null) { MessageBox.Show("Morate izabrati sliku!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

            return true;
        }

        private void PromeniSliku(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Slike (*.png;*.jpg)|*.png;*.jpg";
            dialog.Title = "Izaberi sliku";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                photoPath = storePhotoService.CopyPhotoToPath(DateTime.UtcNow.Millisecond, path);

                if (photoPath != String.Empty)
                {
                    MessageBox.Show("Uspesno ste postavili sliku!", "Uspesno!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                SlikaVulkana.Source = new BitmapImage(new Uri(photoPath));
            }
        }
        private bool KreirajRTF()
        {
            try
            {
                rtfPath = storeRTFService.StoreRTF(GetRTF());
                return true;
            }
            catch 
            {
                return false;
            }
        }
        private byte[] GetRTF()
        {
            TextRange range = new TextRange(RTFField.Document.ContentStart, RTFField.Document.ContentEnd);

            using (MemoryStream ms = new MemoryStream())
            {
                range.Save(ms, DataFormats.Rtf);
                return ms.ToArray();
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            lw.AzurirajListuVulkana();
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}

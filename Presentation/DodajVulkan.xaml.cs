using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Domain.Models;
using Domain.Services;
using Microsoft.Win32;
using Services.SetPhotoService;

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
            if (rtfPath == String.Empty || rtfPath == null) { MessageBox.Show("Morate izabrati rtf fajl!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

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
        private void PromeniRTF(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "RTF (*.rtf)|*.rtf";
            dialog.Title = "Izaberi RTF fajl";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                rtfPath = storeRTFService.StoreRTF(DateTime.UtcNow.Millisecond, path);

                if (rtfPath != String.Empty)
                {
                    MessageBox.Show("Uspesno ste postavili rtf fajl!", "Uspesno!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                IzborRTFFajla.Content = path;
                IzborRTFFajla.Background = Brushes.LightGreen;
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

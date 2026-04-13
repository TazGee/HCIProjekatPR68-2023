using Domain.Database;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentation
{
    public partial class VulkanInfo : Window
    {
        Volcano vulkan = new Volcano();
        ListWindow listWindow;

        IVolcanoUpdateService volcanoUpdateService;
        IStorePhotoService storePhotoService;

        bool menja = false;
        string photoPath;

        public VulkanInfo(Volcano vulkan, ListWindow listWindow, IVolcanoUpdateService volcanoUpdateService, IStorePhotoService storePhotoService, User korisnik)
        {
            this.vulkan = vulkan;
            this.listWindow = listWindow;
            this.volcanoUpdateService = volcanoUpdateService;
            this.storePhotoService = storePhotoService;

            photoPath = vulkan.PhotoPath;

            InitializeComponent();

            UpdateInfo();

            if (!korisnik.Admin) IzmeniSacuvajVulkanButton.Visibility = Visibility.Hidden;
        }

        void UpdateInfo()
        {
            NazivVulkana.Text = vulkan.NazivVulkana;
            DrzavaVulkana.Text = vulkan.Drzava;
            VisinaVulkana.Text = vulkan.Visina.ToString();
            DatumDodavanja.Text = vulkan.DatumDodavanja.ToString();

            if(vulkan.PhotoPath == String.Empty || vulkan.PhotoPath == null || vulkan.PhotoPath == "../../../Resources/volcano.png")
            {
                SlikaVulkana.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/volcano.png"));
            }
            else
            {
                SlikaVulkana.Source = new BitmapImage(new Uri(vulkan.PhotoPath));
            }

            LoadRtf(vulkan.RTFPath);
        }

        private void LoadRtf(string path)
        {
            TextRange range = new TextRange(RtfViewer.Document.ContentStart, RtfViewer.Document.ContentEnd);

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                range.Load(fs, DataFormats.Rtf);
            }
        }

        private void IzmeniSacuvajVulkan(object sender, RoutedEventArgs e)
        {
            if(menja)
            {
                if (!CheckInput()) return;

                if (MessageBox.Show("Da li sigurno zelite da sacuvate promene?", "Cuvanje promena...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if(!AzurirajVulkan())
                    {
                        MessageBox.Show("Doslo je do greske prilikom cuvanja!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    SpremiZaInfo();
                }
            }
            else
            {
                SpremiZaEdit();
            }

            menja = !menja;
        }
        bool CheckInput()
        {
            if (NazivVulkana.Text == "") { MessageBox.Show("Morate popuniti polje za naziv!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (DrzavaVulkana.Text == "") { MessageBox.Show("Morate popuniti polje za drzavu!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (VisinaVulkana.Text == "") { MessageBox.Show("Morate popuniti polje za visinu!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

            return true;
        }

        private void PromeniSliku(object sender, RoutedEventArgs e)
        {
            if(menja)
            {
                OpenFileDialog dialog = new OpenFileDialog(); 
                
                dialog.Filter = "Slike (*.png;*.jpg)|*.png;*.jpg";
                dialog.Title = "Izaberi sliku";

                if (dialog.ShowDialog() == true)
                {
                    string path = dialog.FileName;

                    photoPath = storePhotoService.CopyPhotoToPath(vulkan.Id, path);

                    if (photoPath != String.Empty)
                    {
                        MessageBox.Show("Uspesno ste promenili sliku!", "Uspesno!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    SlikaVulkana.Source = new BitmapImage(new Uri(photoPath));
                }
            }
        }

        bool AzurirajVulkan()
        {
            return volcanoUpdateService.UpdateVolcano(vulkan, NazivVulkana.Text, DrzavaVulkana.Text, VisinaVulkana.Text, photoPath);
        }

        void SpremiZaEdit()
        {
            IzmeniSacuvajVulkanButton.Content = "Sacuvaj vulkan";
            IzmeniSacuvajVulkanButton.Background = Brushes.LightGreen;

            VisinaVulkana.IsReadOnly = false;
            DrzavaVulkana.IsReadOnly = false;
            NazivVulkana.IsReadOnly = false;

            VisinaVulkana.Background = Brushes.LightGray;
            DrzavaVulkana.Background = Brushes.LightGray;
            NazivVulkana.Background = Brushes.LightGray;

            PromenaSlikeButton.Visibility = Visibility.Visible;
        }
        void SpremiZaInfo()
        {
            IzmeniSacuvajVulkanButton.Content = "Izmeni vulkan";
            IzmeniSacuvajVulkanButton.Background = Brushes.LightGray;

            VisinaVulkana.IsReadOnly = true;
            DrzavaVulkana.IsReadOnly = true;
            NazivVulkana.IsReadOnly = true;

            VisinaVulkana.Background = Brushes.Transparent;
            DrzavaVulkana.Background = Brushes.Transparent;
            NazivVulkana.Background = Brushes.Transparent;

            PromenaSlikeButton.Visibility = Visibility.Hidden;

            UpdateInfo();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            listWindow.AzurirajListuVulkana();

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

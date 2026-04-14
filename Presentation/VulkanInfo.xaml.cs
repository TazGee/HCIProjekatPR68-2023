using Domain.Models;
using Domain.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Presentation
{
    public partial class VulkanInfo : Window
    {
        Volcano vulkan = new Volcano();
        ListWindow listWindow;

        IVolcanoUpdateService volcanoUpdateService;
        IStorePhotoService storePhotoService;

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

            if (korisnik.Admin) SpremiZaEdit();
            else SpremiZaInfo();
        }

        void UpdateInfo()
        {
            NazivVulkanaText.Text = vulkan.NazivVulkana;
            DrzavaText.Text = "Drzava: " + vulkan.Drzava;
            VisinaText.Text = "Visina: " + vulkan.Visina;
            DatumDodavanja.Text = vulkan.DatumDodavanja.ToString();

            if (string.IsNullOrEmpty(vulkan.PhotoPath) || !File.Exists(vulkan.PhotoPath))
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
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                RtfViewer.Document.Blocks.Clear();
                RtfViewer.Document.Blocks.Add(new Paragraph(new Run("Nema opisa.")));
                return;
            }

            TextRange range = new TextRange(RtfViewer.Document.ContentStart, RtfViewer.Document.ContentEnd);

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                range.Load(fs, DataFormats.Rtf);
            }
        }


        private void SacuvajVulkan(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) return;

            if (MessageBox.Show("Da li sigurno zelite da sacuvate promene?", "Cuvanje promena...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if(!AzurirajVulkan())
                {
                    MessageBox.Show("Doslo je do greske prilikom cuvanja!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            listWindow.AzurirajListuVulkana();
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

        bool AzurirajVulkan()
        {
            TextRange range = new TextRange(RtfViewer.Document.ContentStart, RtfViewer.Document.ContentEnd);

            using (FileStream fs = new FileStream(vulkan.RTFPath, FileMode.Create))
            {
                range.Save(fs, DataFormats.Rtf);
            }

            return volcanoUpdateService.UpdateVolcano(vulkan, NazivVulkana.Text, DrzavaVulkana.Text, VisinaVulkana.Text, photoPath);
        }

        void SpremiZaEdit()
        {
            IzmeniSacuvajVulkanButton.Content = "Sacuvaj vulkan";
            IzmeniSacuvajVulkanButton.Background = Brushes.LightGreen;

            NazivVulkanaText.Text = "";
            DrzavaText.Text = "Drzava:";
            VisinaText.Text = "Visina:";

            NazivVulkana.Text = vulkan.NazivVulkana;
            DrzavaVulkana.Text = vulkan.Drzava;
            VisinaVulkana.Text = vulkan.Visina.ToString();

            VisinaVulkana.Background = Brushes.LightGray;
            DrzavaVulkana.Background = Brushes.LightGray;
            NazivVulkana.Background = Brushes.LightGray;

            PromenaSlikeButton.Visibility = Visibility.Visible;
            NazivVulkanaText.Visibility = Visibility.Collapsed;
        }
        void SpremiZaInfo()
        {
            IzmeniSacuvajVulkanButton.Content = "Izmeni vulkan";
            IzmeniSacuvajVulkanButton.Background = Brushes.LightGray;

            VisinaVulkana.Visibility = Visibility.Hidden;
            DrzavaVulkana.Visibility = Visibility.Hidden;
            NazivVulkana.Visibility = Visibility.Hidden;
            RtfViewer.IsReadOnly = true;

            PromenaSlikeButton.Visibility = Visibility.Hidden; 
            IzmeniSacuvajVulkanButton.Visibility = Visibility.Hidden;
            PromenaSlikeButton.Visibility = Visibility.Hidden;

            UpdateInfo();

            listWindow.AzurirajListuVulkana();
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

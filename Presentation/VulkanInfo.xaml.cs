using Domain.Models;
using Domain.Services;
using Microsoft.Win32;
using Services.RTFTextEditingService;
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
    public partial class VulkanInfo : Window
    {
        Volcano vulkan = new Volcano();
        ListWindow listWindow;

        IVolcanoUpdateService volcanoUpdateService;
        IStorePhotoService storePhotoService;
        IStoreRTFService storeRTFService;
        IRTFTextEditingService rtfTextEditingService;

        string photoPath;

        public VulkanInfo(Volcano vulkan, ListWindow listWindow, IVolcanoUpdateService volcanoUpdateService, IStorePhotoService storePhotoService, User korisnik, IStoreRTFService storeRTFService, IRTFTextEditingService rtfTextEditingService)
        {
            this.vulkan = vulkan;
            this.listWindow = listWindow;
            this.volcanoUpdateService = volcanoUpdateService;
            this.storePhotoService = storePhotoService;
            this.storeRTFService = storeRTFService;
            this.rtfTextEditingService = rtfTextEditingService;

            photoPath = vulkan.PhotoPath;

            InitializeComponent();

            UpdateInfo();

            if (korisnik.Admin)
            {
                SpremiZaEdit();
                rtfTextEditingService.LoadFonts(FontCombo);
                rtfTextEditingService.LoadColors(ColorCombo);
            }
            else SpremiZaInfo();
        }

        void UpdateInfo()
        {
            NazivVulkanaText.Text = vulkan.NazivVulkana;
            DrzavaText.Text = "Drzava: " + vulkan.Drzava;
            VisinaText.Text = "Visina:  " + vulkan.Visina;
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
                RTFField.Document.Blocks.Clear();
                RTFField.Document.Blocks.Add(new Paragraph(new Run("Nema opisa.")));
                return;
            }

            TextRange range = new TextRange(RTFField.Document.ContentStart, RTFField.Document.ContentEnd);

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                range.Load(fs, DataFormats.Rtf);
            }
        }

        public void BoldBtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, RTFField);
        }
        public void ItalicBtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, RTFField);
        }
        public void UnderlineBtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, RTFField);
        }
        public void FontCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontCombo.SelectedItem is ComboBoxItem item)
            {
                RTFField.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(item.Content.ToString()));
            }
        }
        public void FontSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeCombo.SelectedItem is ComboBoxItem item)
            {
                RTFField.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, double.Parse(item.Content.ToString()));
            }
        }
        public void ColorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorCombo.SelectedItem is ComboBoxItem item)
            {
                Color color = (Color)item.Tag;

                RTFField.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            }
        }
        public void RTFField_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object weight = RTFField.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            BoldBtn.IsChecked = (weight != DependencyProperty.UnsetValue && weight.Equals(FontWeights.Bold));

            object style = RTFField.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            ItalicBtn.IsChecked = (style != DependencyProperty.UnsetValue && style.Equals(FontStyles.Italic));

            object underline = RTFField.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineBtn.IsChecked = (underline != DependencyProperty.UnsetValue && underline.Equals(TextDecorations.Underline));
        }

        private void SacuvajVulkan(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) return;
            if (!storeRTFService.UpdateRTF(GetRTF(), vulkan.RTFPath))
            {
                MessageBox.Show("Doslo je do greske prilikom cuvanja rtf fajla!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
        private byte[] GetRTF()
        {
            TextRange range = new TextRange(RTFField.Document.ContentStart, RTFField.Document.ContentEnd);

            using (MemoryStream ms = new MemoryStream())
            {
                range.Save(ms, DataFormats.Rtf);
                return ms.ToArray();
            }
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
            TextRange range = new TextRange(RTFField.Document.ContentStart, RTFField.Document.ContentEnd);

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
            DrzavaText.Text = "Drzava: ";
            VisinaText.Text = "Visina:  ";

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
            RTFField.IsReadOnly = true;

            PromenaSlikeButton.Visibility = Visibility.Hidden; 
            IzmeniSacuvajVulkanButton.Visibility = Visibility.Hidden;
            PromenaSlikeButton.Visibility = Visibility.Hidden;
            RTFTextEditing.Visibility = Visibility.Collapsed;

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

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
using Domain.Enums;

namespace Presentation
{
    public partial class VulkanInfo : Window
    {
        Volcano volcano = new Volcano();
        ListWindow listWindow;

        IVolcanoUpdateService volcanoUpdateService;
        IStorePhotoService storePhotoService;
        IStoreRTFService storeRTFService;

        string photoPath;

        public VulkanInfo(Volcano volcano, ListWindow listWindow, IVolcanoUpdateService volcanoUpdateService, IStorePhotoService storePhotoService, User user, IStoreRTFService storeRTFService, IRTFTextEditingService rtfTextEditingService)
        {
            this.volcano = volcano;
            this.listWindow = listWindow;
            this.volcanoUpdateService = volcanoUpdateService;
            this.storePhotoService = storePhotoService;
            this.storeRTFService = storeRTFService;

            photoPath = volcano.PhotoPath;

            InitializeComponent();

            UpdateInfo();

            if (user.Role == UserRoles.Admin)
            {
                ShowEdit();
                rtfTextEditingService.LoadFonts(FontCombo);
                rtfTextEditingService.LoadColors(ColorCombo);
            }
            else ShowInfo();
        }

        void UpdateInfo()
        {
            VolcanoNameText.Text = volcano.Name;
            CountryText.Text = "Drzava: " + volcano.Country;
            HeightText.Text = "Visina:  " + volcano.Height;
            AddDate.Text = volcano.AddTime.ToString();

            if (string.IsNullOrEmpty(volcano.PhotoPath) || !File.Exists(volcano.PhotoPath))
            {
                VolcanoPhoto.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/volcano.png"));
            }
            else
            {
                string localPhotoPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, photoPath));
                VolcanoPhoto.Source = new BitmapImage(new Uri(localPhotoPath));
            }

            LoadRtf(volcano.RTFPath);
        }
        private void UpdateWordCount()
        {
            TextRange textRange = new TextRange(RTFField.Document.ContentStart, RTFField.Document.ContentEnd);
            string text = textRange.Text;

            int wordCount = text.Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries).Length;

            WordCountText.Text = $"Word count: {wordCount}";
        }
        private void LoadRtf(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                RTFField.Document.Blocks.Clear();
                RTFField.Document.Blocks.Add(new Paragraph(new Run("No desctiption...")));
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

            UpdateWordCount();
        }

        private void SaveVolcano(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) return;
            if (!storeRTFService.UpdateRTF(GetRTF(), volcano.RTFPath))
            {
                MessageBox.Show("The was an error while trying to save RTF file!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Do you really want to save the changes you made?", "Saving changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if(!UpdateVolcano())
                {
                    MessageBox.Show("There was an error while trying to save changes!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            listWindow.UpdateVolcanoList();
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
            if (VolcanoName.Text == "") { MessageBox.Show("Volcano name field must be filled!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (VolcanoCountry.Text == "") { MessageBox.Show("Volcano country field must be filled!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (VolcanoHeight.Text == "") { MessageBox.Show("Volcano height field must be filled!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

            return true;
        }

        private void ChangePhoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog(); 
                
            dialog.Filter = "Photos (*.png;*.jpg)|*.png;*.jpg";
            dialog.Title = "Choose a photo";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                photoPath = storePhotoService.CopyPhotoToPath(volcano.Id, path);

                if (photoPath != String.Empty)
                {
                    MessageBox.Show("Photo successfully changed!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                string localPhotoPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, photoPath));
                VolcanoPhoto.Source = new BitmapImage(new Uri(localPhotoPath));
            }
        }

        bool UpdateVolcano()
        {
            TextRange range = new TextRange(RTFField.Document.ContentStart, RTFField.Document.ContentEnd);

            using (FileStream fs = new FileStream(volcano.RTFPath, FileMode.Create))
            {
                range.Save(fs, DataFormats.Rtf);
            }

            return volcanoUpdateService.UpdateVolcano(volcano, VolcanoName.Text, VolcanoCountry.Text, VolcanoHeight.Text, photoPath);
        }

        void ShowEdit()
        {
            SaveChangesButton.Content = "Save volcano";
            SaveChangesButton.Background = Brushes.LightGreen;

            VolcanoNameText.Text = "";
            CountryText.Text = "Country: ";
            HeightText.Text = "Height:  ";

            VolcanoName.Text = volcano.Name;
            VolcanoCountry.Text = volcano.Country;
            VolcanoHeight.Text = volcano.Height.ToString();

            VolcanoHeight.Background = Brushes.LightGray;
            VolcanoCountry.Background = Brushes.LightGray;
            VolcanoName.Background = Brushes.LightGray;

            ChangePhotoButton.Visibility = Visibility.Visible;
            VolcanoNameText.Visibility = Visibility.Collapsed;
        }
        void ShowInfo()
        {
            VolcanoHeight.Visibility = Visibility.Hidden;
            VolcanoCountry.Visibility = Visibility.Hidden;
            VolcanoName.Visibility = Visibility.Hidden;
            RTFField.IsReadOnly = true;

            ChangePhotoButton.Visibility = Visibility.Hidden;
            SaveChangesButton.Visibility = Visibility.Hidden;
            RTFTextEditing.Visibility = Visibility.Collapsed;
            WordCountText.Visibility = Visibility.Collapsed;

            UpdateInfo();

            listWindow.UpdateVolcanoList();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            listWindow.UpdateVolcanoList();

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

using Domain.Models;
using Domain.Services;
using Microsoft.Win32;
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
    public partial class AddVolcano : Window
    {
        IAddVolcanoService addVolcanoService;
        IStorePhotoService storePhotoService;
        IStoreRTFService storeRTFService;

        ListWindow lw;

        string photoPath = String.Empty;
        string rtfPath = String.Empty;

        public AddVolcano(IAddVolcanoService addVolcanoService, ListWindow lw, IStorePhotoService storePhotoService, IStoreRTFService storeRTFService, IRTFTextEditingService rtfTextEditingService)
        {
            this.addVolcanoService = addVolcanoService;
            this.lw = lw;
            this.storePhotoService = storePhotoService;
            this.storeRTFService = storeRTFService;

            InitializeComponent();

            rtfTextEditingService.LoadFonts(FontCombo);
            rtfTextEditingService.LoadColors(ColorCombo);
        }
        private void BoldBtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, RTFField);
        }
        private void ItalicBtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, RTFField);
        }
        private void UnderlineBtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, RTFField);
        }
        private void FontCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontCombo.SelectedItem is ComboBoxItem item)
            {
                RTFField.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(item.Content.ToString()));
            }
        }
        private void FontSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeCombo.SelectedItem is ComboBoxItem item)
            {
                RTFField.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, double.Parse(item.Content.ToString()));
            }
        }
        private void ColorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorCombo.SelectedItem is ComboBoxItem item)
            {
                Color color = (Color)item.Tag;

                RTFField.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            }
        }
        private void RTFField_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object weight = RTFField.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            BoldBtn.IsChecked = (weight != DependencyProperty.UnsetValue && weight.Equals(FontWeights.Bold));

            object style = RTFField.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            ItalicBtn.IsChecked = (style != DependencyProperty.UnsetValue && style.Equals(FontStyles.Italic));

            object underline = RTFField.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineBtn.IsChecked = (underline != DependencyProperty.UnsetValue && underline.Equals(TextDecorations.Underline));

            UpdateWordCount();
        }

        private void UpdateWordCount()
        {
            TextRange textRange = new TextRange(RTFField.Document.ContentStart, RTFField.Document.ContentEnd);
            string text = textRange.Text;

            int wordCount = text.Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries).Length;

            WordCountText.Text = $"Word count: {wordCount}";
        }
        private void AddVolcanoClick(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) return;
            if (!CreateRTF()) return;

            if (addVolcanoService.AddVolcano(new Volcano(VolcanoName.Text, Country.Text, int.Parse(Height.Text), photoPath, rtfPath, DateTime.UtcNow)))
            {
                MessageBox.Show("Volcano successfully added!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);

                lw.UpdateVolcanoList();

                Close();
            }
            else
            {
                MessageBox.Show("There was an error while trying to add the volcano!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        bool CheckInput()
        {
            if (VolcanoName.Text == "") { MessageBox.Show("Volcano name field can't be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (Country.Text == "") { MessageBox.Show("Volcano name field can't be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (Height.Text == "") { MessageBox.Show("Volcano name field can't be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

            if(photoPath == String.Empty || photoPath == null) { MessageBox.Show("You must choose a photo!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

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

                photoPath = storePhotoService.CopyPhotoToPath(DateTime.UtcNow.Millisecond, path);

                if (photoPath != String.Empty)
                {
                    MessageBox.Show("Photo successfully set!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                string localPhotoPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, photoPath));
                VolcanoPhoto.Source = new BitmapImage(new Uri(localPhotoPath));
            }
        }
        private bool CreateRTF()
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
            lw.UpdateVolcanoList();
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

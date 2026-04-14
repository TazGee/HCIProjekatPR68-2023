using Domain.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Services.RTFTextEditingService
{
    public class RTFTextEditingService : IRTFTextEditingService
    {
        public void LoadFonts(ComboBox fontCombo)
        {
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = font.Source;
                item.FontFamily = font;
                fontCombo.Items.Add(item);
            }
        }
        public void LoadColors(ComboBox colorCombo)
        {
            var properties = typeof(Colors).GetProperties();

            foreach (var p in properties)
            {
                Color color = (Color)p.GetValue(null);

                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };

                Border rect = new Border
                {
                    Width = 16,
                    Height = 16,
                    Background = new SolidColorBrush(color),
                    Margin = new Thickness(2)
                };

                TextBlock text = new TextBlock
                {
                    Text = p.Name,
                    Margin = new Thickness(5, 0, 0, 0)
                };

                panel.Children.Add(rect);
                panel.Children.Add(text);

                ComboBoxItem item = new ComboBoxItem
                {
                    Content = panel,
                    Tag = color
                };

                colorCombo.Items.Add(item);
            }
        }
    }
}

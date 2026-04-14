using System.Windows.Controls;

namespace Domain.Services
{
    public interface IRTFTextEditingService
    {
        public void LoadFonts(ComboBox fontCombo);
        public void LoadColors(ComboBox colorCombo);
    }
}

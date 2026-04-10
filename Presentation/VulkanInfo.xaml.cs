using System;
using System.Collections.Generic;
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
using Domain.Database;
using Domain.Models;
using Domain.Repositories;

namespace Presentation
{
    public partial class VulkanInfo : Window
    {
        Volcano vulkan = new Volcano();
        IDataBase database;
        ListWindow listWindow;

        bool menja = false;

        public VulkanInfo(Volcano vulkan, IDataBase database, ListWindow listWindow)
        {
            this.vulkan = vulkan;
            this.database = database;
            this.listWindow = listWindow;

            InitializeComponent();

            UpdateInfo();
        }

        void UpdateInfo()
        {
            NazivVulkana.Text = vulkan.NazivVulkana;
            DrzavaVulkana.Text = vulkan.Drzava;
            VisinaVulkana.Text = vulkan.Visina.ToString();
            DatumDodavanja.Text = vulkan.DatumDodavanja.ToString();
        }

        private void IzmeniSacuvajVulkan(object sender, RoutedEventArgs e)
        {
            if(menja)
            {
                if (NazivVulkana.Text == "") MessageBox.Show("Morate popuniti polje za naziv!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
                if (DrzavaVulkana.Text == "") MessageBox.Show("Morate popuniti polje za drzavu!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
                if (VisinaVulkana.Text == "") MessageBox.Show("Morate popuniti polje za visinu!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);

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

        bool AzurirajVulkan()
        {
            vulkan.NazivVulkana = NazivVulkana.Text;
            vulkan.Drzava = DrzavaVulkana.Text;
            vulkan.DatumDodavanja = DateTime.UtcNow;

            database.SaveChanges();

            try
            {
                vulkan.Visina = int.Parse(VisinaVulkana.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void SpremiZaEdit()
        {
            IzmeniSacuvajVulkanButton.Content = "Sacuvaj vulkan";
            IzmeniSacuvajVulkanButton.Background = Brushes.LightGreen;

            VisinaVulkana.IsReadOnly = false;
            DrzavaVulkana.IsReadOnly = false;
            NazivVulkana.IsReadOnly = false;

            VisinaVulkana.Background = Brushes.White;
            DrzavaVulkana.Background = Brushes.White;
            NazivVulkana.Background = Brushes.White;
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

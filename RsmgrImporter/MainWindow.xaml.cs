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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RsmgrImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Database_Import_Click(object sender, RoutedEventArgs e)
        {
            DatabaseImportWindow win1 = new DatabaseImportWindow();
            win1.Show();
        }

        private void Text_Import_Click(object sender, RoutedEventArgs e)
        {
            ImportTextWindow win1 = new ImportTextWindow();
            win1.Show();
        }

        private void EndOfDay_Click(object sender, RoutedEventArgs e)
        {
            EndOfDayWindow win1 = new EndOfDayWindow();
            win1.Show();
        }
    }
}

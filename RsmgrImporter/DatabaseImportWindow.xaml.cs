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

namespace RsmgrImporter
{
    /// <summary>
    /// Interaction logic for DatabaseImportWindow.xaml
    /// </summary>
    public partial class DatabaseImportWindow : Window
    {
        private Logger logging = new Logger();

        public DatabaseImportWindow()
        {
            InitializeComponent();
        }

        private void btnFullSync_Click(object sender, RoutedEventArgs e)
        {
            AccessQueries accquer = new AccessQueries();
            SQLGetStatements getter = new SQLGetStatements();
            txtDBSyncStatus.Text = "";

            if (cbEmployees.IsChecked.Value == true)
            {
                int intvalue = 0;
                string currowcount = "";
                try { currowcount = getter.GetAppConfig("EmpRowCount"); }
                catch (Exception e) { logging.writeToLog("Error: Convert EmpRowCount to Int : " + e.Message + "; Value = " + currowcount); }

                try { intvalue = Convert.ToInt32(currowcount); }
                catch (Exception e) { logging.writeToLog("Error: Convert EmpRowCount to Int : " + e.Message + "; Value = " + currowcount); }

                txtDBSyncStatus.Text += "Importing Employees...\n";
                accquer.ImportEmployees();  // Import Employees
            }

            if (cbStores.IsChecked.Value == true)
            {
                txtDBSyncStatus.Text += "Stores Imported...\n";
                accquer.ImportDepartments();// Import Departments
            }
           
            if (cbCustomers.IsChecked.Value == true)
            {
                txtDBSyncStatus.Text += "Customers Imported...\n";
                accquer.ImportCustomers();  // Import Customers
            }

            if (cbCategories.IsChecked.Value == true)
            {
                txtDBSyncStatus.Text += "Seasons Imported...\n";
                // Import Seasons
                txtDBSyncStatus.Text += "Sizes Imported...\n";
                // Import Sizes
                txtDBSyncStatus.Text += "Divisions Imported...\n"; // Import Divisions
                txtDBSyncStatus.Text += "Classes Imported...\n"; // Import Classes

            }

            if (cbItems.IsChecked.Value == true)
            {
                txtDBSyncStatus.Text += "Styles Imported...\n"; // Import Styles
                txtDBSyncStatus.Text += "Inventory Imported...\n"; // Import Inventory
                accquer.ImportItems();
            }

            if (cbVendors.IsChecked.Value == true)
            {
                txtDBSyncStatus.Text += "Vendors Imported...\n"; // Import Vendors
                accquer.ImportVendors();
            }

            //txtDBSyncStatus.Text += "Approvals Imported...\n";
            //// Import Approvals
            //txtDBSyncStatus.Text += "Charge Customers Imported...\n";
            //// Import Charge Customers.
            //txtDBSyncStatus.Text += "Cost Codes Imported...\n";
            //// Import CostCode
            //txtDBSyncStatus.Text += "Credits Imported...\n";
            //// Import Credits
            //txtDBSyncStatus.Text += "Orders Imported...\n";
            //// Import Orders
            //txtDBSyncStatus.Text += "Sales Imported...\n";
            //// Import Sales
            //txtDBSyncStatus.Text += "Tickets Imported...\n";
            //// Import Tickets
        }

        private void btnDeltaSync_Click(object sender, RoutedEventArgs e)
        {
            txtDBSyncStatus.Text = "";

        }
    }
}

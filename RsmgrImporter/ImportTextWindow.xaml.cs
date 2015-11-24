using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace RsmgrImporter
{
    /// <summary>
    /// Interaction logic for ImportTextWindow.xaml
    /// </summary>
    public partial class ImportTextWindow : Window
    {
        public ImportTextWindow()
        {
            InitializeComponent();
        }
        #region Class Declerations
        private string FilePath = null;
        private int CountUpdInv = 0;
        private int CountInsInv2 = 0;
        private int CountInsInvTOP = 0;
        private int CountInsSales = 0;
        private int CountInsTickets = 0;
        private int CountInsCustomers = 0;
        private int CountInsApprovals = 0;
        private int CountOthers = 0;
        private int CountInsCharges = 0;
        private int CountInsCredits = 0;
        private int CountUpdChargeCustomers = 0;
        private int CountChargeCustomers2 = 0;
        private int CountDeleteCredits = 0;
        private int CountDeleteInventory = 0;
        private int CountDelApp = 0;
        private int CountUpdInv2 = 0;
        private int CountInsEmail = 0;
        private int CountInsLayaway = 0;
        private int CountUpdOrders = 0;
        private int CountUpdOrderSpecs = 0;
        private int CountUpdInvB = 0;
        private int CountInsStyles = 0;
        private int CountUpdStyles = 0;
        private int CountInsLayawayPay = 0;
        private int CountUpdLayaway = 0;
        private int CountInsOrderSpecs2 = 0;
        private int CountInsOrderSpecs = 0;
        #endregion

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
        #region Declerations
        CountUpdInv = 0;
        CountInsInv2 = 0;
        CountInsInvTOP = 0;
        CountInsSales = 0;
        CountInsTickets = 0;
        CountInsCustomers = 0;
        CountInsApprovals = 0;
        CountOthers = 0;
        CountInsCharges = 0;
        CountInsCredits = 0;
        CountUpdChargeCustomers = 0;
        CountChargeCustomers2 = 0;
        CountDeleteCredits = 0;
        CountDeleteInventory = 0;
        CountDelApp = 0;
        CountUpdInv2 = 0;
        CountInsEmail = 0;
        CountInsLayaway = 0;
        CountUpdOrders = 0;
        CountUpdOrderSpecs = 0;
        CountUpdInvB = 0;
        CountInsStyles = 0;
        CountUpdStyles = 0;
        CountInsLayawayPay = 0;
        CountUpdLayaway = 0;
        CountInsOrderSpecs = 0;
        CountInsOrderSpecs2 = 0;
        #endregion

        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        openFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        if (openFileDialog.ShowDialog() == true)
        {
            FilePath = openFileDialog.FileName;
        }
        lblFile.Content = "File: " + FilePath;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader(FilePath);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                ParseLineOfText(line);
            }
            string results = "Update Inventory = " + CountUpdInv.ToString() +
                "\n Update Inventory = " + CountInsInv2.ToString() +
                "\n Insert Sales = " + CountInsSales.ToString() +
                "\n Insert Tickets = " + CountInsTickets.ToString() +
                "\n Insert Customers = " + CountInsCustomers.ToString() +
                "\n Insert Approvals = " + CountInsApprovals.ToString() +
                "\n Insert Charges = " + CountInsCharges.ToString() +
                "\n Insert Credits = " + CountInsCredits.ToString() +
                "\n Customer Charges = " + CountUpdChargeCustomers.ToString() +
                "\n Customer Charges 2 = " + CountChargeCustomers2.ToString() +
                "\n Inventory TOP = " + CountInsInvTOP.ToString() +
                "\n Delete Credits = " + CountDeleteCredits.ToString() +
                "\n Delete Inventory = " + CountDeleteInventory.ToString() +
                "\n Delete Approvals = " + CountDelApp.ToString() +
                "\n Update Inventory2 = " + CountUpdInv2.ToString() +
                "\n Insert Email = " + CountInsEmail.ToString() +
                "\n Insert Layway = " + CountInsLayaway.ToString() +
                "\n Update Orders = " + CountUpdOrders.ToString() +
                "\n Update Order Specs = " + CountUpdOrderSpecs.ToString() +
                "\n Update Inventory B = " + CountUpdInvB.ToString() +
                "\n Insert Styles = " + CountInsStyles.ToString() +
                "\n Update Styles = " + CountUpdStyles.ToString() +
                "\n Insert Layaway Payments" + CountInsLayawayPay.ToString() +
                "\n Update Layaway " + CountUpdLayaway.ToString() +
                "\n Insert OrderSpecs = " + CountInsOrderSpecs.ToString() +
                "\n Insert OrderSpecs2 = " + CountInsOrderSpecs2.ToString() +
                "\n Others = " + CountOthers.ToString();
            

            PopUp popup = new PopUp("Counts", results);
            popup.Show();
        }

        private void ParseLineOfText(string text)
        {
 
            switch (text.Substring(0,Math.Min(4,text.Length)))
            {
                case "Dele":
                    ParseDeletes(text);
                    break;
                case "dele":
                    ParseDeletes(text);
                    break;
                case "DELE":
                    ParseDeletes(text);
                    break;
                case "Inse":
                    ParseInserts(text);
                    break;
                case "inse":
                    ParseInserts(text);
                    break;
                case "INSE":
                    ParseInserts(text);
                    break;
                case "UPDA":
                    ParseUpdates(text);
                    break;
                case "upda":
                    ParseUpdates(text);
                    break;
                case "Upda":
                    ParseUpdates(text);
                    break;
                default:
                    PopUp popup = new PopUp("No Case for Items", text);
                    popup.Show();
                    // write to log to be reimported.
                    break;
            }

            
            
            

        }

        private void ParseDeletes(string text)
        {
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase;
            if (text.StartsWith("Delete * from Credits", comparer))
            {
                CountDeleteCredits = CountDeleteCredits + 1;
            }
            else if (text.StartsWith("Delete * from Inventory", comparer))
            {
                CountDeleteInventory = CountDeleteInventory + 1;
            }

            else if (text.StartsWith("Delete * from Approvals", comparer))
            {
                CountDelApp = CountDelApp + 1;
            }
        }
        
        private void ParseInserts(string text)
        {
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase;

            if (text.StartsWith("Insert INTO Inventory2", comparer))
            {
                CountInsInv2 = CountInsInv2 + 1;
            }
            else if (text.StartsWith("Insert into Sales Values", comparer))
            {
                CountInsSales = CountInsSales + 1;
            }
            else if (text.StartsWith("Insert into Tickets Values", comparer))
            {
                CountInsTickets = CountInsTickets + 1;
            }
            else if (text.StartsWith("Insert into Customers Values", comparer))
            {
                CountInsCustomers = CountInsCustomers + 1;
            }
            else if (text.StartsWith("INSERT Into Approvals Values", comparer))
            {
                CountInsApprovals = CountInsApprovals + 1;
            }
            else if (text.StartsWith("Insert into Charges Values", comparer))
            {
                CountInsCharges = CountInsCharges + 1;
            }
            else if (text.StartsWith("Insert into Credits Values", comparer))
            {
                CountInsCredits = CountInsCredits + 1;
            }
            else if (text.StartsWith("Insert Into Inventory Select TOP 1", comparer))
            {
                CountInsInvTOP = CountInsInvTOP + 1;
            }
            else if (text.StartsWith("Insert Into Email Values", comparer))
            {
                CountInsEmail = CountInsEmail + 1;
            }
            else if (text.StartsWith("INSERT Into Layaway Values", comparer))
            {
                CountInsLayaway = CountInsLayaway + 1;
            }

            else if (text.StartsWith("Insert Into Styles", comparer))
            {
                CountInsStyles = CountInsStyles + 1;
            }

            else if (text.StartsWith("INSERT INTO OrderSpecs VALUES", comparer))
            {
                CountInsOrderSpecs = CountInsOrderSpecs + 1;
            }
            else if (text.StartsWith("INSERT INTO OrderSpecs2 VALUES", comparer))
            {
                CountInsOrderSpecs2 = CountInsOrderSpecs2 + 1;
            }
        }

        private void ParseUpdates(string text)
        {
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase;

            if (text.StartsWith("Update Inventory Set Units", comparer))
            {
                CountUpdInv = CountUpdInv + 1;
            }
            else if (text.StartsWith("Update ChargeCustomers set Payments =", comparer))
            {
                CountUpdChargeCustomers = CountUpdChargeCustomers + 1;
            }
            else if (text.StartsWith("Update ChargeCustomers set Charges =", comparer))
            {
                CountChargeCustomers2 = CountChargeCustomers2 + 1;
            }
            else if (text.StartsWith("Update Inventory2 set Units", comparer))
            {
                CountUpdInv2 = CountUpdInv2 + 1;
            }
            else if (text.StartsWith("Update Orders", comparer))
            {
                CountUpdOrders = CountUpdOrders + 1;
            }
            else if (text.StartsWith("Update OrderSpecs", comparer))
            {
                CountUpdOrderSpecs = CountUpdOrderSpecs + 1;
            }
            else if (text.StartsWith("Update Inventory", comparer))
            {
                CountUpdInvB = CountUpdInvB + 1;
            }
            else if (text.StartsWith("Update Styles", comparer))
            {
                CountUpdStyles = CountUpdStyles + 1;
            }
        }
    }
}

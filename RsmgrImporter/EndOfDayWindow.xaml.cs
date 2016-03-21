using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace RsmgrImporter
{
    /// <summary>
    /// Interaction logic for EndOfDayWindow.xaml
    /// </summary>
    public partial class EndOfDayWindow : Window
    {
        private string writefile = System.AppDomain.CurrentDomain.BaseDirectory + @"output.txt"; // write to output.txt strings it cannot convert
        private Logger logger = new Logger();
        private SQLInsertStatements si = new SQLInsertStatements();
        private SQLGetStatements ss = new SQLGetStatements();
        private string FilePath = null;
        private Char comma = ',';
        private Char space = ' ';

        public EndOfDayWindow()
        {
            InitializeComponent();
            if (!File.Exists(writefile)) { File.Create(writefile); }
        }
        
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
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
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase;
            string line; // Initiates a variable for a line of the file
            List<string> insertcustomers = new List<string>();
            List<string> inserttickets = new List<string>();
            List<string> lines = new List<string>();
            
            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("Insert into Tickets Values", comparer))
                {
                    inserttickets.Add(line);
                }
                else if (line.StartsWith("Insert into Customers Values", comparer))
                {
                    insertcustomers.Add(line);
                }
                else
                {
                    lines.Add(line);
                }
            }

            foreach (string text in insertcustomers)
            {
                ParseLineOfText(text);
            }
            foreach (string text in inserttickets)  // iterate through all ticket inserts second
            {
                TextImporter ti = new TextImporter();
                ti.InsertTicketsEoD(text); // goes to TextImport.cs => InsertTicketsEoD
            }
            foreach (string text in lines) // iterate through everything else last
            {
                ParseLineOfText(text);
            }

        }

        private void ParseLineOfText(string text)
        {
            switch (text.Substring(0, Math.Min(4, text.Length)).ToLower())
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
                    // PopUp popup = new PopUp("No Case for Items", text);
                    // popup.Show();
                    // WriteToOutput(text);
                    logger.writeToImports(text);
                    break;
            }
        }

        private void WriteToOutput(string text)
        {
            try { File.AppendAllText(writefile, text + "\n"); }
            catch (Exception e)
            {
                PopUp popup = new PopUp("Importer Message", "Error writing to Output. " + e.Message);
                popup.Show();
            }
        }

        private void ParseDeletes(string text)
        {
            // Find the matching string, if found delete items from SQL.
            // If not found, then output the line into a text file.
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase;
            if (text.StartsWith("Delete * from Credits", comparer))
            {
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Delete * from Inventory", comparer))
            {
                logger.writeToImports(text);
            }

            else if (text.StartsWith("Delete * from Approvals", comparer))
            {
                logger.writeToImports(text);
            }
            else
            {
                logger.writeToImports(text);
            }
        }

        private void ParseInserts(string text)
        {
            // Find the matching string, if found insert items from SQL.
            // If not found, then output the line into a text file.
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase;

            if (text.StartsWith("Insert INTO Inventory2", comparer))
            {
                // TODO: Write to Inventory
                // Don't need this one.
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Insert into Sales Values", comparer))
            {
                // TODO: Write to Tickets
                // This is done through Tickets
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Insert into Tickets Values", comparer)) //            !---------Completed----------!
            {
                TextImporter ti = new TextImporter(); 
                ti.InsertTicketsEoD(text); // goes to TextImport.cs => InsertTicketsEoD
            }
            else if (text.StartsWith("Insert into Customers Values", comparer))
            {
                // TODO: Write to Customers Database
                logger.writeToImports(text);
            }
            else if (text.StartsWith("INSERT Into Approvals Values", comparer))
            {
                // TODO: Write to Approvals
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Insert into Charges Values", comparer))
            {
                // TODO: Write to Charges
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Insert into Credits Values", comparer))
            {
                // (CreditNumber, Amount, CustomerID, DateSold) = Ticket Update (TicketID, PaymentAmount, PaymentType, CompletedDate)
                // TODO: Write to Credits
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Insert Into Inventory Select TOP 1", comparer))
            {
                // TODO: Write to Inventory
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Insert Into Email Values", comparer))
            {
                // TODO: Write to Person
                logger.writeToImports(text);
            }
            else if (text.StartsWith("INSERT Into Layaway Values", comparer))
            {
                // TODO: Write to Tickets
                logger.writeToImports(text);
            }

            else if (text.StartsWith("Insert Into Styles", comparer))
            {
                // TODO: Write to Items
                logger.writeToImports(text);
            }

            else if (text.StartsWith("INSERT INTO OrderSpecs VALUES", comparer))
            {
                // TODO: Write to Invoices
                logger.writeToImports(text);
            }
            else if (text.StartsWith("INSERT INTO OrderSpecs2 VALUES", comparer))
            {
                // TODO: Write to Invoices
                logger.writeToImports(text);
            }
            else
            {
                // TODO: Write to 
                logger.writeToImports(text);
            }
        }

        private void ParseUpdates(string text)
        {
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase;

            if (text.StartsWith("Update Inventory Set Units", comparer))
            {
                // TODO: Get Itemid Update Inventory
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Update ChargeCustomers set Payments =", comparer))
            {
                // TODO: Get Itemid Update Inventory
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Update ChargeCustomers set Charges =", comparer))
            {
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Update Inventory2 set Units", comparer))
            {
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Update Orders", comparer))
            {
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Update OrderSpecs", comparer))
            {
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Update Inventory", comparer))
            {
                logger.writeToImports(text);
            }
            else if (text.StartsWith("Update Styles", comparer))
            {
                logger.writeToImports(text);
            }
            else
            {
                logger.writeToImports(text);
            }
        }
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace RsmgrImporter
{
    /// <summary>
    /// Interaction logic for ImportInventoryWindow.xaml
    /// </summary>
    public partial class ImportInventoryWindow : Window
    {
        //private string writefile = @"ImpInvMissed_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"; // write to output.txt strings it cannot convert
        private Logger logger = new Logger();
        private SQLInsertStatements si = new SQLInsertStatements();
        private SQLGetStatements ss = new SQLGetStatements();
        private SQLUpdateStatements su = new SQLUpdateStatements();
        private static HashSet<string> previousLines = new HashSet<string>();
        private string FilePath = null;
        private Char comma = ',';
        private Char space = ' ';

        public ImportInventoryWindow()
        {
            InitializeComponent();
            //writefile = writefile.Replace(":", "");
            //writefile = writefile.Replace("/", "");
            //if (!File.Exists(writefile)) { File.Create(writefile); }
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
            #region Declerations
            int departmentid = 0; // declare the departmentid
            int itemid = 0; // declare the itemid
            int inventoryid = 0; // declare the inventoryid
            int quantity = 0; // declare the quantity
            string status = ""; // get status for item_quantity
            string tfcontents = "Beginning the import..."; // string for textblock
            DataTable dt = new DataTable(); // declare datatable
            #endregion

            txtFileContents.Text = tfcontents; // write string to textblock
            
            if (tbDeptNum.Text != null || tbDeptNum.Text != "") // check to see if department number textbox contains text or is empty
            {
                departmentid = ss.GetDepartmentID(tbDeptNum.Text); // get the departmentid, need from SQLGetStatements
            }

            if (departmentid > 0 && FilePath != null) // check to see if departmentid returned a number and that the file to import is not empty
            {
                StreamReader file = new StreamReader(FilePath); // Read from the text file
                string line; // declare the string for the lines
                int linecount = 0;
                ClearInventoryValues(departmentid); // clears the inventory values and sets quantities to 0, updates status to "InActive".

                while ((line = file.ReadLine()) != null) // assign string line to the line from a file and confirm it is not null.
                {
                    itemid = ss.GetItemIDByBarcode(line); // get the itemid from SQLGetStatements by barcode
                    linecount++;
                    if (itemid <= 1) { itemid = si.InsertItems("", 1, line, "Created to insert into ItemQuantity", 0, 0, 0, 0, 0, 0, "Unknown", "", "", "", "", "", "", "", "", "", ""); }

                    if (previousLines.Contains(line)) { // The line has been used before
                        dt = ss.GetItemQuantity(itemid, departmentid); // get the current quantity infromation
                        if (dt.Rows.Count > 0) {
                            quantity = dt.Rows[0].Field<int>("quantity"); // get the current quantity
                            quantity = quantity + 1; // add 1 to the quantity
                            if (su.SQLItemQuantityUpdateQty(itemid, departmentid, quantity, "Active")) // Update inventory
                            {
                                si.InsertInventoryLog(itemid, "", departmentid, 1, "Inventory increased by 1 for item.  The current quantity is " + quantity.ToString()); // insert into Inventory Log
                            }
                        }
                        else if (dt.Rows.Count <= 0)// if ItemQuantity does not exist
                        {
                            si.InsertItemQuantities(itemid, departmentid, 1, "Active");
                            si.InsertInventoryLog(itemid, "", departmentid, 1, "Added to inventory quantities with default quantity of 1.");
                        }
                        else
                        {
                            logger.WriteToOutput(line);
                            //WriteToOutput(line);
                        }                
                        dt.Clear();
                    }
                    else
                    {
                        dt = ss.GetItemQuantity(itemid, departmentid); // get the itemquantity from itemid and departmentid
                        if (dt.Rows.Count > 0) // if ItemQuantity exists
                        {
                            quantity = dt.Rows[0].Field<int>("quantity"); // assign "quantity" to quantity variable
                            status = dt.Rows[0].Field<string>("status"); // assign "status" to status variable
                            inventoryid = dt.Rows[0].Field<int>("item_quantity_id");    // assign "item_quantity_id" to inventoryid variable
                            previousLines.Add(line); // Add the line to previous lines.
                            if (su.SQLItemQuantityUpdateQty(itemid, departmentid, 1, "Active")) // try to update quantity
                            {
                                si.InsertInventoryLog(itemid, "", departmentid, 1, "Inventory reset and then increased by 1 for item.  Previous quantity was: " + quantity.ToString()); // insert into inventorylog
                            }
                        }
                        else if (dt.Rows.Count <= 0)// if ItemQuantity does not exist
                        {
                            // TODO: Input a new inventory item.
                            si.InsertItemQuantities(itemid, departmentid, 1, "Active");
                            si.InsertInventoryLog(itemid, "", departmentid, 1, "Added to inventory quantities with default quantity of 1.");
                        }
                        else // check for exceptions
                        {
                            logger.WriteToOutput(line);
                            //WriteToOutput(line);
                        }
                        dt.Clear();
                    }
                }
                previousLines.Clear();
                tfcontents = tfcontents + Environment.NewLine + "Completed the Import!";
                txtFileContents.Text = tfcontents;
                PopUp pop = new PopUp("Inventory Scan Complete", "Inventory Scan for Store " + tbDeptNum.Text + " has finished." + Environment.NewLine + "There were " + linecount.ToString() + " items that were proceesed");
                pop.Show();
            }
        }

        private void ClearInventoryValues(int departmentid)
        {
            DataTable dt = new DataTable();
            int itemquantityid = 0;
            int itemid = 0;
            int quantity = 0;
            dt = ss.GetItemQuantityForDepartment(departmentid);
            foreach (DataRow row in dt.Rows)
            {
                itemquantityid = Convert.ToInt32(row[4]);
                itemid = Convert.ToInt32(row[5]);
                try { quantity = Convert.ToInt32(row[2]); } catch (Exception) { quantity = 1; }

                if (row[3].ToString() == "Inserted by System")
                {
                    su.SQLItemQuantityUpdateQtyByID(itemquantityid, 0, "InActive"); // Updates quantity and status
                    si.InsertInventoryLog(itemid, "", departmentid, 1, "Inventory cleared for item.  Previous quantity was: " + quantity.ToString()); // insert into inventorylog
                }
                else if (row[3].ToString() == "Active")
                {
                    su.SQLItemQuantityUpdateQtyByID(itemquantityid, 0, "InActive"); // Updates quantity and status
                    si.InsertInventoryLog(itemid, "", departmentid, 1, "Inventory cleared for item.  Previous quantity was: " + quantity.ToString()); // insert into inventorylog
                }
                else if (quantity != 0)
                {
                    su.SQLItemQuantityUpdateQtyByID(itemquantityid, 0, "InActive"); // Updates quantity and status
                    si.InsertInventoryLog(itemid, "", departmentid, 1, "Inventory cleared for item.  Previous quantity was: " + quantity.ToString()); // insert into inventorylog
                }
                else
                {
                    // logger.writeToLog("ImportInventoryWindow.xaml.cs/ClearInventoryValues failed at Line 151. Status is " + row[3].ToString() + " and Quantity is " + quantity.ToString() + ".");
                }
            }            
            dt.Clear();
        }

    }
}

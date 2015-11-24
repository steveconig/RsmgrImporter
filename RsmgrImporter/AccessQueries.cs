using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace RsmgrImporter
{
    class AccessQueries
    {
        private string AccessString = ConfigurationManager.ConnectionStrings["AccessDBSConnString"].ConnectionString;
        private DataTable res = new DataTable();
        private Logger logging = new Logger();

        public void ImportEmployees()
        {
            int intvalue = 0;
            string currowcount = "";
            SQLInsertStatements importer = new SQLInsertStatements();
            SQLGetStatements getter = new SQLGetStatements();
            OleDbConnection con = new OleDbConnection(AccessString);
            con.Open();
            OleDbCommand cmd = new OleDbCommand("Select * FROM Employees", con);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); // DB Adapter for data transfer, Execute(cmd)
            adapter.Fill(res); // Fill (results) datatable with Adapter data.
            try { currowcount = getter.GetAppConfig("EmpRowCount"); }
            catch(Exception e) { logging.writeToLog("Error: Convert EmpRowCount to Int : " + e.Message + "; Value = " + currowcount); }

            try { intvalue = Convert.ToInt32(currowcount); }
            catch (Exception e) { logging.writeToLog("Error: Convert EmpRowCount to Int : " + e.Message + "; Value = " + currowcount); }

            if (res.Rows.Count != intvalue)
            {
                if (intvalue == 0) { importer.InsertAppConfig("EmpRowCount", res.Rows.Count.ToString()); }
                
                foreach (DataRow row in res.Rows)
                {
                    int addressid = importer.InsertAddress(row[3].ToString(), "", "", row[4].ToString(), row[5].ToString(), row[6].ToString(), "USA", "Imported from Access");
                    int personid = importer.InsertPerson(row[1].ToString(), row[0].ToString(), addressid, "", row[7].ToString(), row[8].ToString(), "", "", row[10].ToString(), row[9].ToString(), "Imported from Access", "", "Active");
                    int employeeid = importer.InsertEmployee(row[2].ToString(), "", "", personid, row[11].ToString(), "");
                }

                try { res.Clear(); }
                catch (DataException e)
                {
                    logging.writeToLog("Error: Clear Results Employee DataTable : " + e.Message);
                }
            }
        }

        public void ImportCustomers()
        {
            //AccessDB = LastName, FirstName, Address, City, State, ZipCode, HomePhone, CellPhone, Email, Birthday, JoinDate, CustomerID, 
            //              0           1        2      3       4       5       6           7       8       9           10          11
            //ChargeCustomer, StoreNumber, SPECIAL, PAYMENTS, ACCTBAL, WORKPHN, Limit, SSN, SSN2
            //       12           13          14        15      16        17      18    19   20

            int intcharge = 0;
            int personid = 0;
            int customerid = 0;
            string strcharge = "";
            decimal declimit = 0;
            string strjoindate = "";
            string strstore = "";
            int intstore = 0;
            decimal balance = 0;
            int addressid = 0;
            string custcount = "";

            OleDbConnection con = new OleDbConnection(AccessString);
            DataTable results = new DataTable();
            OleDbCommand cmd = new OleDbCommand("Select * FROM Customers", con);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); // DB Adapter for data transfer, Execute(cmd)
            try
            {
                con.Open();
                adapter.Fill(results);
                con.Close();
            }
            catch (Exception e)
            {
                logging.writeToLog("Error: Connect to Access for Customers DB : " + e.Message);
            }

            SQLInsertStatements cimporter = new SQLInsertStatements();
            SQLGetStatements cgetter = new SQLGetStatements();

            int intvalue = 0;
            try { intvalue = Convert.ToInt32(cgetter.GetAppConfig("CusRowCount")); }
            catch (Exception e)
            {
                logging.writeToLog("Error: Convert CusRowCount to Int : " + e.Message);
            }

            if (results.Rows.Count != intvalue)
            {
                if (intvalue == 0)
                {
                    custcount = cimporter.InsertAppConfig("CusRowCount", results.Rows.Count.ToString());
                }

                foreach (DataRow row in results.Rows)
                {
                    strjoindate = row[10].ToString();
                    strstore = row[11].ToString();
                        // convert row12 string to int charge account
                    strcharge = row[12].ToString().ToLower(); // convert to lowercase
                    if (strcharge == "yes") { intcharge = 1; }  // convert string to int if "yes" 
                        // convert row18 to charge limit
                    if (row[18] == null) { declimit = 0; } else { 
                        try { declimit = Convert.ToDecimal(row[18].ToString()); }
                        catch(Exception e)
                        {
                            declimit = 0;
                            logging.writeToLog("Error: Convert Customer.row18 to declimit : " + e.Message + "\n Value: " + row[18].ToString());
                        }
                    }
                        // convert row16 to balance
                    if (row[16] == null) { balance = 0; } else {
                        try { balance = Convert.ToDecimal(row[16]); }
                        catch(Exception e)
                        {
                            balance = 0;
                            logging.writeToLog("Error: Convert Customer.row16 to balance : " + e.Message + "\n Value: " + row[16].ToString());
                        }   
                    }

                    if (strjoindate.Length < 7) { strjoindate = ""; }

                    if (strstore.Length > 1) { strstore.TrimStart('0'); }
                    if (strstore == null || strstore == "") { strstore = "1"; }
                    try { intstore = Convert.ToInt32(strstore); }
                    catch (FormatException e)
                    {
                        logging.writeToLog("Error: Convert Customer.strStore to intStore : " + e.Message + "\n String Value: " + strstore);
                        intstore = 1;
                    }

                    addressid = cimporter.InsertAddress(row[2].ToString(), "", "", row[3].ToString(), row[4].ToString(), row[5].ToString(), "USA", "Imported from Access");
                    personid = cimporter.InsertPerson(row[1].ToString(), row[0].ToString(), addressid, "", row[6].ToString(), row[7].ToString(), row[17].ToString(), "", row[9].ToString(), row[8].ToString(), "Imported from Access", "", "Active");
                    customerid = cimporter.InsertCustomer(row[11].ToString(), personid, strjoindate, intcharge, declimit, 0, intstore, balance, 1);
                }

                try { results.Clear(); }
                catch (Exception e)
                {
                    logging.writeToLog("Error: Clear Results Customer DataTable" + e.Message);
                }
            }
        }

        public void ImportDepartments()
        {
            //AccessDB = StoreName, StoreID 
            //               0         1  
            int addressid = 0;
            int departmentid = 0;

            OleDbConnection conn = null;
            DataTable results = new DataTable();
            conn = new OleDbConnection(AccessString);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand("Select * FROM Stores", conn);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); // DB Adapter for data transfer, Execute(cmd)
            adapter.Fill(results); // Fill (results) datatable with Adapter data.

            foreach (DataRow row in results.Rows)
            {
                SQLInsertStatements importer = new SQLInsertStatements();

                // Insert into Address get ID
                addressid = importer.InsertAddress("", "", "", "", "", "", "USA", "Imported from Access");
                // Insert into Person get ID
                departmentid = importer.InsertDepartment(row[0].ToString(), row[1].ToString(), addressid, "Imported from Access", "Active");
            }

            try { results.Clear(); }
            catch (Exception e)
            {
                logging.writeToLog("Error: Clearing the results in Department of dataTable. " + e.Message);
            }
        }

        public void ImportVendors()
        {   // Vendor Name , Address, City, State, ZipCode, PhoneNumber, FaxNumber, Contact
            //   	0			1	   2      3       4          5           6        7

            int addressid = 0;
            int personid = 0;
            int vendorid = 0;

            OleDbConnection con = null;
            DataTable dt = new DataTable();
            con = new OleDbConnection(AccessString);
            con.Open();
            OleDbCommand cmd = new OleDbCommand("Select * FROM Vendors", con);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); // DB Adapter for data transfer, Execute(cmd)
            adapter.Fill(dt); // Fill (results) datatable with Adapter data.
            SQLInsertStatements importer = new SQLInsertStatements();

            foreach (DataRow row in dt.Rows)
            {
                addressid = importer.InsertAddress(row[2].ToString(), "", "", row[3].ToString(), row[4].ToString(), row[5].ToString(), "USA", "Imported from Access");
                personid = importer.InsertPerson(row[7].ToString(), "", addressid, "", "", "", "", "", "", "", "", "", "Active");
                vendorid = importer.InsertVendors(row[0].ToString(), row[1].ToString(), row[6].ToString(), addressid, personid, "Imported from Access");
            }

            try { dt.Clear(); }
            catch (Exception e)
            {
                logging.writeToLog("Error: Clear Results Vendors DataTable : " + e.Message);
            }
        }

        public void ImportItems()
        {
            //Inventory= Style, Color, Description, iSize, Season, Division, Department, Class, Barcode, Cost, Retail, Units, Branch, 
            //             0      1         2         3      4        5         6          7       8      9      10     11      12         
            //          DateOrdered, DateReceived, Vendor, Sizes, Ordered, Received, Sold, DateSold, Sales, Label
            //              13           14         15      16      17         18     19      20      21     22
            //Styles =  Vendor, VendorName, Season, Style, Color, Description, Sizes, Division, Department, Class, Cost, Retail
            //            0         1          2      3      4         5         6       7          8         9     10     11
            #region Access Connection
            using (OleDbConnection con = new OleDbConnection(AccessString))
            {
                using (OleDbCommand cmd = new OleDbCommand("Select * FROM Inventory WHERE DateOrdered > @date AND Branch = @store", con))
                {
                    cmd.Parameters.Add(new OleDbParameter("@date", OleDbType.Date)).Value = new DateTime(2014, 12, 31);
                    cmd.Parameters.Add(new OleDbParameter("@store", OleDbType.VarChar)).Value = "4";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); 
                    try
                    {
                        con.Open();
                        adapter.Fill(res);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Failed to execute Access ImportItems. " + e.Message);
                    }
                    con.Close();
                }
            }
            #endregion
            #region Declerations
            SQLInsertStatements importer = new SQLInsertStatements();
            SQLGetStatements getter = new SQLGetStatements();
            int intvalue = 0;
            int vendorid = 0;
            int invoiceid = 0;
            int invoiceitemid = 0;
            int itemid = 0;
            string invoicenum = "";
            string vendornumber = "";
            decimal cost = 0;
            decimal retail = 0;
            
            #endregion

            try { intvalue = Convert.ToInt32(getter.GetAppConfig("EmpRowCount")); }
            catch (Exception e) { logging.writeToLog("Error: Convert EmpRowCount to Int. " + e.Message); }

            if (res.Rows.Count != intvalue)
            {
                importer.InsertAppConfig("EmpRowCount", res.Rows.Count.ToString());

                foreach (DataRow row in res.Rows)
                {
                    vendornumber = row[15].ToString();
                    if (row[9] == null) { cost = 0; } //row9 to cost
                    else
                    {
                        try { cost = Convert.ToDecimal(row[9]); }
                        catch (Exception e)
                        { logging.writeToLog("Error: Convert Items.row9: " + row[9].ToString() + " to cost failed. " + e.Message); }
                    }
                    if (row[10] == null) { cost = 0; }
                    else
                    {
                        try { retail = Convert.ToDecimal(row[10]); }
                        catch (Exception e)
                        { logging.writeToLog("Error: Convert Customer.row10: " + row[10].ToString() + " to retail failed. " + e.Message); }
                    }
                    try { vendorid = getter.GetVendorID(vendornumber); }
                    catch(Exception e) { logging.writeToLog("Warning: Unable to retrieve VendorID from ImportItems : " + e.Message); }
                    if (vendorid < 1) { vendorid = 1; }
                    try { invoiceid = getter.GetInvoiceIDforImports(vendorid); }
                    catch(Exception e) { logging.writeToLog("Warning: Unable to retrieve InvoiceID from ImportItems : " + e.Message); }
                    if (invoiceid < 1)
                    {
                        invoicenum = "X" + vendornumber + "X";
                        invoiceid = importer.InsertInvoices(invoicenum, row[13].ToString(), row[14].ToString(), vendorid, 1, "Created by System", "none");
                    }
                    itemid = importer.InsertItems(row[0].ToString(), 1, row[8].ToString(), row[2].ToString(), cost, retail, 0, 0, 0, 1, "Active", row[0].ToString(), row[1].ToString(), row[4].ToString(), row[16].ToString(), row[3].ToString(), "", "", row[5].ToString(), row[6].ToString(), row[7].ToString());
                    // insert invoice items using itemid and vendorid.
                    invoiceitemid = importer.InsertInvoiceItems(invoiceid, itemid, "Created by System", "", 0, 0, 0, 0, 0, 1, 0);
                }

                try { res.Clear(); }
                catch (DataException e)
                {
                    logging.writeToLog("Error: Clear Results Items DataTable" + e.Message);
                }
            }
        }
    }
}

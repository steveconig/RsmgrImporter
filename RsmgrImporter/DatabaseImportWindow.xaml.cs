using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
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
        #region Declerations
        private Logger logging = new Logger();
        SQLInserts si = new SQLInserts();
        SQLSelects ss = new SQLSelects();
        SQLUpdates su = new SQLUpdates();
        AccessQueries accquer = new AccessQueries();
        private Conversions conv = new Conversions();
        private string eargs = "";
        private const string defaultStoreID = "11111111-0111-1000-1000-000011112222";
        #endregion

        public DatabaseImportWindow()
        {
            InitializeComponent();
        }

        private void btnFullSync_Click(object sender, RoutedEventArgs e)
        {

            AccessQueries accquer = new AccessQueries();
            //txtDBSyncStatus.Text = "";

            if (cbStores.IsChecked.Value == true)
            {
                DataTable dt = new DataTable();
                accquer.ImportDepartments();
                //txtDBSyncStatus.Text += "Stores Imported...\n";
                string addressid = "", departmentid = "";

                foreach (DataRow row in dt.Rows)
                {
                    // Insert into Address get ID
                    addressid = si.InsertAddress("", "", "", "", "", "", "USA", "Imported from Access");
                    // Insert into Person get ID
                    departmentid = si.InsertDepartment(row[0].ToString(), row[1].ToString(), addressid, "Imported from Access", "Active");
                }

                try { dt.Clear(); }
                catch (Exception) {
                    logging.writeToLog("Error: Clearing the results in Department of dataTable. ");
                }
            }

            if (cbEmployees.IsChecked.Value == true)
            {
                accquer.ImportEmployees();  // Import Employees
                //txtDBSyncStatus.Text += "Employees Imported...\n";
            }

            if (cbCustomers.IsChecked.Value == true)
            {
                DataTable dt = new DataTable();
                dt = accquer.ImportCustomers();
                Customers(dt);
                //txtDBSyncStatus.Text += "Customers Imported...\n";
            }
            if (cbVendors.IsChecked.Value == true)
            {
                DataTable dt = new DataTable();
                eargs = eargs + "vendors ";
                //txtDBSyncStatus.Text += "Vendors Imported...\n"; // Import Vendors
                dt = accquer.ImportVendors();
                VendorImport(dt);

            }
            if (cbCategories.IsChecked.Value == true)
            {
                //txtDBSyncStatus.Text += "Seasons Imported...\n";
                //txtDBSyncStatus.Text += "Sizes Imported...\n";
                //txtDBSyncStatus.Text += "Divisions Imported...\n"; // Import Divisions
                //txtDBSyncStatus.Text += "Classes Imported...\n"; // Import Classes
                // TODO: import categories
            }
            if (cbItems.IsChecked.Value == true) // Import Items
            {
                eargs = eargs + "items ";
                Items();
            }
            if (cbTickets.IsChecked.Value == true)
            {
                DataTable tickets = new DataTable(); // Tickets =>   Date | TicketNumber | Total | Payment | Customer | Employee
                string date = dpTickets.SelectedDate.ToString();
                DateTime start = DateTime.Now; DateTime end = DateTime.Now;
                start = conv.ConvertToDate(date);
                if (start.ToString("MM/dd/yyyy") == DateTime.Now.ToString("MM/dd/yyyy"))
                {
                    start = start.AddMonths(-4);
                }
                for (var dte = start; dte <= end; dte = dte.AddDays(1))
                {
                    // Insert into tickets, ticket_items and ticket_payments
                    tickets = accquer.GetTicketsByDate(dte.ToString("MM/dd/yyyy"));
                    Tickets(tickets);
                    tickets.Clear();
                }
                PopUp pop = new PopUp("Tickets Imported.", "Tickets have finished importing.");
                pop.Show();
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

            AccessQueries accquer = new AccessQueries();
            //txtDBSyncStatus.Text = "";
            DateTime end = DateTime.Now; DateTime start = new DateTime();
            string gac = "", writedate = end.ToString("MM/dd/yyyy"), storenum = "";
            string query = "SELECT TOP 1 * FROM app_config WHERE ukey = 'previous_sync'";

            if (ss.IfExists(query))
            { // if previuos_sync exists in database
                gac = ss.GetAppConfig("previous_sync");
            }
            else {
                si.InsertAppConfig("previous_sync", writedate);
                gac = "02/01/2016";
            }

            // converts string gac to DateTime start
            start = conv.ConvertToDate(gac, -7);


            if (cbStores.IsChecked.Value == true)
            {
                
            }
            if (cbEmployees.IsChecked.Value == true)
            {
                
            }
            if (cbCustomers.IsChecked.Value == true)
            {
                DataTable customers = new DataTable();
                DataTable chargecustomers = new DataTable();
                // TODO: Get Acct Balances to show up
                // Access Query => Field<decimal>("ACCTBAL")
                // Iterate throught he list and do the following:
                for (var dte = start; dte <= end; dte = dte.AddDays(1))
                {
                    // Check to see if customer exists in the table
                    // Import Customers and ChargeCustomers by joindate from previous_import -> now.
                    customers = accquer.ImportCustomers(dte.ToString());
                    foreach (DataRow row in customers.Rows)
                    {

                    }
                    customers.Clear();
                }
            }
            if (cbVendors.IsChecked.Value == true)
            {
                

            }
            if (cbCategories.IsChecked.Value == true)
            {
                
            }
            if (cbItems.IsChecked.Value == true)
            {
                DataTable items = new DataTable();
                // Import items
                for (var dt = start; dt <= end; dt = dt.AddDays(1))
                {
                    items = accquer.GetItems(storenum, dt);
                    foreach (DataRow row in items.Rows)
                    {
                        // look for new items where date received is today
                        // input the information into the database
                    }
                    items.Clear();
                }
                
            }
            if (cbTickets.IsChecked.Value == true) // Import tickets from date
            {
                DataTable tickets = new DataTable();
                for (var dt = start; dt <= end; dt = dt.AddDays(1))
                {
                    tickets = accquer.GetTicketsByDate(dt.ToString("MM/dd/yyyy"));
                    SyncTickets(tickets, dt.Date);
                    tickets.Clear();
                }
            }
            
            su.UpdateAppConfig("previous_sync", writedate);
        }
            


        private void Items()
        {
            AccessQueries accquer = new AccessQueries();
            DataTable dt1 = new DataTable();
            DateTime itemdate = DateTime.Now.AddYears(-3);
            string vendorid = "", invoiceid = "", invoiceitemid = "", itemid = "", quantityid = "", departmentid = "";
            int units = 0;
            string invoicenum = "";
            string vendornumber = "";
            decimal cost = 0;
            decimal retail = 0;

            if (dpItemDate.Text != null || dpItemDate.Text != "")
            {
                itemdate = conv.ConvertToDate(dpItemDate.Text, -1095);
            } // TODO: Validate if it is an actual date.  Change to DatePicker.

            // 1 = Paragould, 2 = Jackson, 4 = Jonesboro
            if (tbDeptNum.Text == "1" || tbDeptNum.Text == "2" || tbDeptNum.Text == "4") 
            {
                dt1 = accquer.GetItems(tbDeptNum.Text, itemdate);
            }
            else { dt1 = accquer.GetItems("1", itemdate); } // Query Access with parameter of a store number and cutoff date.


            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow row in dt1.Rows)
                {
                    vendornumber = row[15].ToString();
                    if (row[9] == null) { cost = 0; } else {
                        cost = conv.ConvertToDecimal(row[9].ToString());
                    }

                    if (row[10] == null) { retail = 0; } // row 10 to Retail
                    else { 
                        retail = conv.ConvertToDecimal(row[10].ToString());
                    }

                    if (row[11] == null) { units = 1; } else {
                        units = conv.ConvertToInt(row[11].ToString());
                    }

                    try { vendorid = ss.GetVendorID(vendornumber); }
                    catch (Exception) { logging.writeToLog("Warning: Unable to retrieve VendorID = " + vendornumber + " from ImportItems. "); }
                    if (vendorid == "" || vendorid == null) { vendorid = "1"; }

                    try { invoiceid = ss.GetInvoiceIDforImports(vendorid); }
                    catch (Exception) { logging.writeToLog("Warning: Unable to retrieve InvoiceID = " + vendorid.ToString() + " from ImportItems."); }

                    if (invoiceid == "" || invoiceid == null) {
                        invoicenum = "X" + vendornumber + "X";
                        invoiceid = si.InsertInvoices(invoicenum, row[13].ToString(), row[14].ToString(), vendorid, "22331111-0111-1000-1000-000011112222", "Created by System", "none"); // Invoices
                    }

                    itemid = si.InsertItems(row[0].ToString(), "44441111-0111-1000-1000-000011112222", row[8].ToString(), row[2].ToString(), cost, retail, 0, 0, 0, "", "Active", row[0].ToString(), row[1].ToString(), row[4].ToString(), row[16].ToString(), row[3].ToString(), "", "", row[5].ToString(), row[6].ToString(), row[7].ToString()); // Items

                    DataTable dt2 = ss.GetDepartmentByDeptNum(tbDeptNum.Text);
                    try
                    {
                        departmentid = dt2.Rows[0].Field<string>("id");
                    }
                    catch (Exception)
                    {
                        departmentid = "11111111-0111-1000-1000-000011112222";
                    }
                     // TODO: Add this to a list and search the list for quicker results.
                    // Find the date of the item and retrieve the invoiceid
                    // Insert invoice with datetime
                    invoiceitemid = si.InsertInvoiceItems(invoiceid, itemid, "Created by System", "", 0, 0, 0, 0, 0, departmentid, 0); // Invoice Items
                    quantityid = si.InsertItemQuantities(itemid, departmentid, units, "Inserted by System"); // Insert into ItemQuantities
                }

                try { dt1.Clear(); }
                catch (Exception) { logging.writeToLog("Error: Clearing Results in Items DataTable"); }
            }
            PopUp pop = new PopUp("Items Imported.", "Items have finished importing.");
            pop.Show();
        }

        private void Customers(DataTable results)
        {

            int intcharge = 0, intvalue = 0;
            string strcharge = "", strjoindate = "", strstore = "", custcount = "", addressid = "", personid = "", customerid = "", intstore = "";
            decimal declimit = 0, balance = 0;

            intvalue = conv.ConvertToInt(ss.GetAppConfig("CusRowCount"));

            if (results.Rows.Count != intvalue)
            {
                if (intvalue == 0)
                {
                    custcount = si.InsertAppConfig("CusRowCount", results.Rows.Count.ToString());
                }

                foreach (DataRow row in results.Rows)
                {
                    strjoindate = row[10].ToString();
                    strstore = row[11].ToString();
                    // convert row12 string to int charge account
                    strcharge = row[12].ToString().ToLower(); // convert to lowercase
                    // convert string to int if "yes"
                    if (strcharge == "yes") { intcharge = 1; }   
                    // convert row18 to charge limit
                    if (row[18] == null) { declimit = 0; }
                    else {
                        try { declimit = Convert.ToDecimal(row[18].ToString()); }
                        catch (Exception e)
                        {
                            declimit = 0;
                            logging.writeToLog("Error: Convert Customer.row18 to declimit : " + e.Message + "\n Value: " + row[18].ToString());
                        }
                    }
                    // convert row16 to balance
                    if (row[16] == null) { balance = 0; }
                    else { conv.ConvertToDecimal(row[16].ToString()); }
                    if (strjoindate.Length < 7) { strjoindate = ""; }
                    if (strstore.Length > 1) { strstore.TrimStart('0'); }
                    if (strstore == null || strstore == "") { strstore = "11111111-0111-1000-1000-000011112222"; }
                    
                    addressid = si.InsertAddress(row[2].ToString(), "", "", row[3].ToString(), row[4].ToString(), row[5].ToString(), "USA", "Imported from Access");
                    personid = si.InsertPerson(row[1].ToString(), row[0].ToString(), addressid, "", row[6].ToString(), row[7].ToString(), row[17].ToString(), "", row[9].ToString(), row[8].ToString(), "Imported from Access", "", "Active");
                    customerid = si.InsertCustomer(row[11].ToString(), personid, strjoindate, intcharge, declimit, 0, intstore, balance, 1);
                }

                try { results.Clear(); }
                catch (Exception e)
                {
                    logging.writeToLog("Error: Clear Results Customer DataTable" + e.Message);
                }
            }
            PopUp pop = new PopUp("Customers Imported.", "Customers have finished importing.");
            pop.Show();
        }

        /// <summary>
        /// AccessDB => LastName | FirstName | Address | City | State | ZipCode | HomePhone | CellPhone | Email | Birthday | JoinDate | CustomerID | ChargeCustomer | StoreNumber | SPECIAL | PAYMENTS | 
        ///     ACCTBAL | WORKPHN | Limit | SSN | SSN2
        /// </summary>
        /// <param name="results"></param>
        private void SyncCustomers(DataTable results)
        {
            // declerations
            int intcharge = 0;
            string strcharge = "", strjoindate = "", strstore = "", addressid = "", customerid = "", personid = "", storeid = "";
            decimal declimit = 0, balance = 0;

            // check the count vs the value from app_config, if they don't equal then 
            if (results.Rows.Count > 0)
            { 
                // results = accquer.customers(date)    
                foreach (DataRow row in results.Rows)
                {
                    // convert row 10 to join date
                    strjoindate = row[10].ToString();
                    // convert row 11 to store number
                    strstore = row[11].ToString();
                    // convert row12 string to int charge account
                    strcharge = row[12].ToString().ToLower(); // convert to lowercase
                    // convert string to int if "yes" otherwise it is 0
                    if (strcharge == "yes") { intcharge = 1; }
                    else { intcharge = 0; }
                    // convert row18 to charge limit
                    if (row[18] == null) { declimit = 0; }
                    else {
                        // try to convert the row 18 to charge limit (decimal)
                        try { declimit = Convert.ToDecimal(row[18].ToString()); }
                        catch (Exception e) {
                            declimit = 0;  // otherwise the limit is set to 0
                            logging.writeToLog("Error: Convert Customer.row18 to declimit : " + e.Message + "\n Value: " + row[18].ToString());
                        }
                    }
                    // convert row16 to balance
                    if (row[16] == null) { balance = 0; }
                    else {
                        try { balance = Convert.ToDecimal(row[16]); }
                        catch (Exception e)
                        {
                            balance = 0;
                            logging.writeToLog("Error: Convert Customer.row16 to balance : " + e.Message + "\n Value: " + row[16].ToString());
                        }
                    }

                    if (strjoindate.Length < 7) { strjoindate = ""; }

                    if (strstore.Length > 1) { strstore.TrimStart('0'); }
                    if (strstore == null || strstore == "") { strstore = defaultStoreID; }
                    
                    addressid = si.InsertAddress(row[2].ToString(), "", "", row[3].ToString(), row[4].ToString(), row[5].ToString(), "USA", "Imported from Access");
                    personid = si.InsertPerson(row[1].ToString(), row[0].ToString(), addressid, "", row[6].ToString(), row[7].ToString(), row[17].ToString(), "", row[9].ToString(), row[8].ToString(), "Imported from Access", "", "Active");
                    customerid = si.InsertCustomer(row[11].ToString(), personid, strjoindate, intcharge, declimit, 0, storeid, balance, 1);

                }
            }
        }

        private void Tickets(DataTable results)
        {
            #region Declerations
            DataTable ticketitems = new DataTable();
            decimal total = 0, cost = 0, retail = 0, quantity = 1, discount = 0, discount2 = 0;
            string date, ticketnumber, paymenttype, customerfname, customerlname, employeefname, employeelname, cmdtext = "", dis = "";
            string[] customername;
            string[] employeename;
            int customerid = 1, employeeid = 1, departmentid = 1, ticketid = 1, itemid = 1, itemqty = 1;
            #endregion

            // parse tickets datatable
            foreach (DataRow row in results.Rows) // Tickets =>   Date | TicketNumber | Total | Payment | Customer | Employee
            {
                cmdtext = "SELECT TOP 1 * FROM [dbo].[tickets] WHERE invoice_number = '" + row[1].ToString() + "'";
                if (ss.IfExists(cmdtext))
                {
                    logging.writeToLog("The invoice_number " + row[1].ToString() + " is already in the SQL Database.");
                    // NEXT: Maybe update the ticket fields?
                }
                else
                {
                    customerfname = row[4].ToString();
                    customerlname = "";
                    customername = customerfname.Split(' ');
                    if (customername.Length < 1) { customerid = 1; }
                    else if (customername.Length == 2)
                    {
                        customerfname = customername[0];
                        customerlname = customername[1];
                        customerid = ss.GetCustomerIDByName(customerfname, customerlname);
                    }
                    else if (customername.Length == 3)
                    {
                        customerfname = customername[0] + " " + customername[1];
                        customerlname = customername[2];
                        customerid = ss.GetCustomerIDByName(customerfname, customerlname);
                    }
                    else { customerid = 1; }
                    employeefname = row[5].ToString();
                    employeelname = "";
                    employeename = employeefname.Split(' ');
                    if (employeename.Length < 1) { employeeid = 1; }
                    else if (employeename.Length == 2)
                    {
                        employeefname = employeename[0];
                        employeelname = employeename[1];
                        employeeid = ss.GetEmployeeIDByName(employeefname, employeelname);
                    }
                    else if (employeename.Length == 3)
                    {
                        employeefname = employeename[0] + employeename[1];
                        employeelname = employeename[2];
                        employeeid = ss.GetEmployeeIDByName(employeefname, employeelname);
                    }
                    else { employeeid = 1; }

                    date = row[0].ToString();
                    ticketnumber = row[1].ToString();
                    total = Convert.ToDecimal(row[2].ToString());
                    paymenttype = row[3].ToString();

                    // Insert into SQL => tickets
                    ticketid = si.InsertTicket(date, date, customerid, employeeid, "Imported from Acccess database.", paymenttype, ticketnumber);
                    // Insert TicketPayments => SQL
                    si.InsertTicketPayments(ticketid, paymenttype, total); // inserted into SQL => ticket_payments
                    // Get TicketItems <= Access
                    ticketitems = accquer.ImportSalesByTicket(ticketnumber); // Import ticket items into database  
                    // line                  
                    int line = 1;
                    // Sales => Barcode, Style, Color, Description, Size, Vendor, PurchaseDate, Cost, Retail, Discount, SalePrice, CustomerID, EmployeeID, TicketInfo, TicketNumber, Branch, Discount2
                    foreach (DataRow rown in ticketitems.Rows)  
                    {
                        itemid = ss.GetItemIDByBarcode(rown[0].ToString());
                        dis = rown[9].ToString();
                        if (dis != null || dis != "")
                        {
                            if (dis.EndsWith("%")) { dis = dis.TrimEnd(new char[] { '%', ' ' }); }
                            conv.ConvertToDecimal(dis);
                        }
                        else { discount = 0; }
                        dis = rown[16].ToString();
                        if (dis != null || dis != "")
                        {
                            if (dis.EndsWith("%")) { dis = dis.TrimEnd(new char[] { '%', ' ' }); }
                            discount2 = conv.ConvertToDecimal(dis);
                        }
                        else { discount2 = 0; }

                        cost = conv.ConvertToDecimal(rown[7].ToString());
                        retail = conv.ConvertToDecimal(rown[8].ToString());
                        
                        si.InsertTicketItems(ticketid, itemid, "Imported from Access database.", rown[0].ToString(), line, quantity, cost, retail, discount, discount2, "");
                        line = line++;
                        // adjust inventory for the item sold.
                        itemqty = Convert.ToInt16(quantity);
                        su.SQLItemQuantityUpdateQty(itemid, departmentid, itemqty, "Changed by TicketInserts");
                    }
                }   
            }
        }

        private void VendorImport(DataTable results)
        {
            int addressid = 0, personid = 0, vendorid = 0;
            foreach (DataRow row in results.Rows) {
                addressid = si.InsertAddress(row[2].ToString(), "", "", row[3].ToString(), row[4].ToString(), row[5].ToString(), "USA", "Imported from Access");
                personid = si.InsertPerson(row[7].ToString(), "", addressid, "", "", "", "", "", "", "", "", "", "Active");
                vendorid = si.InsertVendors(row[0].ToString(), row[1].ToString(), row[6].ToString(), addressid, personid, "Imported from Access");
            }

            try { results.Clear(); }
            catch (Exception e) { logging.writeToLog("Error: Clear Results Vendors DataTable : " + e.Message); }

        }

        private void SyncTickets(DataTable tickets, DateTime sdate)
        {
            #region Declerations
            string ticketnumber = "", comments = "", paymenttype = "", tquery = "", fullname = "", firstname = "", lastname = "", barcode = "", status = "", branchnumber = "";
            string cust1 = "", cust2 = "", cust3 = "", cust4 = "", cust5 = "", cust6 = "", cust7 = "", cust8 = "", cust9 = "", cust10 = "";
            int customerid = 0, employeeid = 0, ticketid = 0, namelength = 0, ticketpaymentid = 0, itemid = 0, line = 0, branchid = 0, ticitemid = 0, qty = 0, saleprice = 0;
            string[] splitname, branchsplit;
            decimal total = 0, qtypurchased = 0, costprice = 0, unitprice = 0, discount = 0, discount2 = 0;
            DataTable ticdt = new DataTable(); DataTable ticitems = new DataTable(); DataTable itemqty = new DataTable();
            #endregion

            foreach (DataRow row in tickets.Rows)
            {
                // Check to see if ticket exists.
                ticketnumber = row.Field<string>("TicketNumber");
                branchsplit = ticketnumber.Split('-'); branchnumber = branchsplit[1]; branchid = ss.GetDepartmentID(branchnumber);

                try { total = row.Field<decimal>("Total"); }
                catch (Exception) { total = 0; logging.writeToLog("Error: Unable to convert " + row.Field<decimal>("Total").ToString() + " to decimal for ticket_number " + ticketnumber + "."); }
                
                // Split name by space from Customer field, lastname is equal to length - 1
                fullname = row.Field<string>("Customer"); splitname = fullname.Split(' '); namelength = splitname.Length; lastname = splitname[namelength - 1];
                // determine firstname based on the namelength perameter
                if (namelength == 2) { firstname = splitname[0]; }
                else if (namelength == 3) { firstname = splitname[0] + " " + splitname[1]; }
                else if (namelength == 4) { firstname = splitname[0] + " " + splitname[1] + " " + splitname[2]; }
                else if (namelength == 5) { firstname = splitname[0] + " " + splitname[1] + " " + splitname[2] + " " + splitname[4]; }
                // Get customerid from firstname, lastname
                customerid = ss.GetCustomerIDByName(firstname, lastname);
                splitname = fullname.Split(' ');
                namelength = splitname.Length;
                lastname = splitname[namelength - 1];
                // Get employeeid from firstname, lastname
                fullname = row.Field<string>("Employee");
                splitname = fullname.Split(' ');
                namelength = splitname.Length;
                lastname = splitname[namelength - 1];
                if (namelength == 2) { firstname = splitname[0]; }
                else if (namelength == 3) { firstname = splitname[0] + " " + splitname[1]; }
                employeeid = ss.GetEmployeeIDByName(firstname, lastname);
                // Get PaymentType from row[3]
                paymenttype = row[3].ToString();
                // Check to see if ticket exists
                tquery = "Select TOP 1 * FROM tickets WHERE invoice_number = '" + ticketnumber + "'";
                ticdt = ss.GetDataTableFromQuery(tquery);
                if (ticdt.Rows.Count > 0)
                {
                    ticketid = Convert.ToInt32(ticdt.Rows[0][0].ToString());
                    su.UpdateTickets(ticketid, sdate.ToString(), sdate.ToString(), customerid, employeeid, comments, paymenttype, ticketnumber);
                }
                else
                {
                    // Inserts the Ticket into SQL
                    ticketid = si.InsertTicket(sdate.ToString(), sdate.ToString(), customerid, employeeid, comments, paymenttype, ticketnumber);
                    // Inserts the Ticket_Payments into SQL
                    ticketpaymentid = si.InsertTicketPayments(ticketid, paymenttype, total);
                    // Gets the TicketItems from Access Database
                    ticitems = accquer.ImportSalesByTicket(ticketnumber);
                    line = 0;
                    foreach (DataRow item in ticitems.Rows) {
                        // Define the fields for items
                        line = line + 1;
                        barcode = item.Field<string>("Barcode");
                        qtypurchased = 1;
                        costprice = conv.ConvertToDecimal(item[7].ToString());
                        unitprice = conv.ConvertToDecimal(item[8].ToString());
                        discount = conv.ConvertToDecimal(item[9].ToString());
                        discount2 = conv.ConvertToDecimal(item[16].ToString());
                        saleprice = conv.ConvertToInt(item[10].ToString());
                        status = "Active";
                        itemid = ss.GetItemIDByBarcode(barcode);

                        // Check to see if the item exists in the database.
                        if (itemid <= 1) {
                            // Create item if it does not exist.
                            itemid = si.InsertItems("", 0, barcode, item.Field<string>("Description"), costprice, unitprice, 0, 0, 0, 0, "Active", cust1, cust2, cust3, cust4, cust5, cust6, cust7, cust8, cust9, cust10);
                        }
                        si.InsertTicketItems(ticketid, itemid, "Imported from Access ", barcode, line, qtypurchased, costprice, unitprice, discount, discount2, status);
                        itemqty = ss.GetItemQuantity(itemid, branchid);
                        // handle item quantities
                        if (itemqty.Rows.Count == 1) {
                            qty = Convert.ToInt32(itemqty.Rows[0][3].ToString()) - 1;
                            su.SQLItemQuantityUpdateQty(itemid, branchid, qty, "Unknown");
                        } else {
                            si.InsertItemQuantities(itemid, branchid, 0, "Unknown");
                        }
                        itemqty.Clear();
                    }
                    ticitems.Clear();
                }
                ticdt.Clear();
            }
            tickets.Clear();
        }

        private void SyncItems(DataTable items, DateTime sdate)
        {
            string query = "";
            DataTable dtitem = new DataTable();
            foreach (DataRow item in items.Rows)
            {
                query = "Select TOP 1 * FROM items WHERE barcode = '" + item[8] + "'";
                if (ss.IfExists(query)) // does query exist
                {

                }

                // TODO: foreach item that was purchased on date insert into Items Table in SQL.
            }
            dtitem.Clear();
        }

        private void btnSyncBalances_Click(object sender, RoutedEventArgs e)
        {
            string fn, ln;
            string id = "";
            decimal bal = 0;

            DataTable cust = new DataTable();
            cust = accquer.ImportCustomersWithBalance();
            foreach (DataRow customer in cust.Rows)
            {
                fn = customer.Field<string>("FirstName");//[1].ToString();
                ln = customer.Field<string>("LastName");//[0].ToString();
                bal = conv.ConvertToDecimal(customer[16].ToString()); //.Field<decimal>("ACCTBAL") conv.ConvertToDecimal()
                id = ss.GetCustomerIDByName(fn, ln);

                su.UpdateCustomerBalance(id, bal);
            }
            cust.Clear();
        }
    }
}

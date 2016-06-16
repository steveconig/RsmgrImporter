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

        /// <summary>
        /// AccessDB = LastName, FirstName, Address, City, State, ZipCode, HomePhone, CellPhone, Email, Birthday, JoinDate, CustomerID, 
        ///     ChargeCustomer, StoreNumber, SPECIAL, PAYMENTS, ACCTBAL, WORKPHN, Limit, SSN, SSN2
        /// </summary>
        /// <returns></returns>
        public DataTable ImportCustomers()
        {            
            DataTable results = new DataTable();
            using (OleDbConnection con = new OleDbConnection(AccessString))
            {
                using(OleDbCommand cmd = new OleDbCommand("Select * FROM Customers", con))
                {
                    using(OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
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
                    }
                    con.Close();
                }
            }
            return results;
        }

        public DataTable ImportCustomersWithBalance()
        {
            DataTable results = new DataTable();
            using (OleDbConnection con = new OleDbConnection(AccessString))
            {
                using (OleDbCommand cmd = new OleDbCommand("Select * FROM Customers WHERE ACCTBAL > 0", con))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
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
                    }
                    con.Close();
                }
            }
            return results;
        }

        /// <summary>
        /// AccessDB = LastName, FirstName, Address, City, State, ZipCode, HomePhone, CellPhone, Email, Birthday, JoinDate, CustomerID, 
        ///     ChargeCustomer, StoreNumber, SPECIAL, PAYMENTS, ACCTBAL, WORKPHN, Limit, SSN, SSN2
        /// </summary>
        /// <param name="date">string date</param>
        /// <returns></returns>
        public DataTable ImportCustomers(string date)
        {
            //AccessDB = LastName, FirstName, Address, City, State, ZipCode, HomePhone, CellPhone, Email, Birthday, JoinDate, CustomerID, 
            //              0           1        2      3       4       5       6           7       8       9           10          11
            //ChargeCustomer, StoreNumber, SPECIAL, PAYMENTS, ACCTBAL, WORKPHN, Limit, SSN, SSN2
            //       12           13          14        15      16        17      18    19   20
            string query = "Select * FROM Customers WHERE JoinDate = '" + date + "'";
            using (DataTable results = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
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
                        }
                        con.Close();
                    }
                }
                return results;
            }
        }

        /// <summary>
        /// AccessDB = StoreName, StoreID
        /// </summary>
        public DataTable ImportDepartments()
        {
            string query = "Select * FROM Stores";
            using (DataTable results = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(results);
                                con.Close();
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Connect to Access for ImportDepartments DB : " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return results;
            }
        }

        /// <summary>
        /// Vendors = Vendor Name | Address | City | State | ZipCode | PhoneNumber | FaxNumber | Contact
        /// </summary>
        public DataTable ImportVendors()
        {
            string query = "Select * FROM Vendors";
            using (DataTable dt = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(dt);
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Failed to execute Access ImportItems. " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return dt;
            }
        }

        /// <summary>
        ///  Inventory= Style | Color | Description | iSize | Season | Division | Department | Class | Barcode | Cost | Retail | Units | Branch 
        ///    | DateOrdered | DateReceived | Vendor | Sizes | Ordered | Received | Sold | DateSold | Sales | Label
        /// </summary>
        /// <param name="storenumber"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable GetItems(string storenumber, DateTime date)
        {
            string query = "Select * FROM Inventory WHERE DateOrdered > #01/01/2013# AND Branch = '" + storenumber + "'";
            using (DataTable dt = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        date = new DateTime(2013, 01, 01).Date;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@date", date);
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(dt);
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Failed to execute Access ImportItems. " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// Get Items from Access Database where date = @date and branch = @store
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable GetItemsByDate(DateTime date, string store)
        {
            string query = "Select * FROM Inventory WHERE DateOrdered > #" + date + "# AND Branch = '" + store + "'";
            using (DataTable dt = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(dt);
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Failed to execute Access ImportItems. " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// Tickets =>   Date | TicketNumber | Total | Payment | Customer | Employee
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DataTable ImportTickets(DateTime date)
        {
            DataTable dt = new DataTable();
            string query = "Select * FROM Tickets WHERE Date = '" + date + "'";
            using (OleDbConnection con = new OleDbConnection(AccessString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        try
                        {
                            con.Open();
                            adapter.Fill(dt);
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Error: Failed to execute Access ImportTickets. " + e.Message);
                        }
                    }
                    con.Close();
                }
            }
            return dt;

        }

        /// <summary>
        /// Tickets =>   Date | TicketNumber | Total | Payment | Customer | Employee
        /// </summary>
        /// <param name="date"></param>
        /// <returns>DataTable of Tickets</returns>
        public DataTable GetTicketsByDate(string date)
        {
            DataTable dt = new DataTable();
            string query = "Select * FROM Tickets WHERE Date = '" + date + "'";
            using (OleDbConnection con = new OleDbConnection(AccessString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        try
                        {
                            con.Open();
                            adapter.Fill(dt);
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Error: Failed to execute Access ImportTickets. " + e.Message);
                        }
                    }
                    con.Close();
                }
                return dt;
            }
        }

        /// <summary>
        /// Sales => Barcode | Style | Color | Description | Size | Vendor | PurchaseDate | Cost | Retail | Discount | SalePrice | CustomerID | EmployeeID | TicketInfo | TicketNumber | Branch |  Discount2
        /// </summary>
        /// <param name="TicketNumber">Ticket Number</param>
        /// <returns>List of sales by ticket number.</returns>
        public DataTable ImportSalesByTicket(string ticketNumber)
        {
            using (DataTable dt = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand("Select * FROM Sales WHERE TicketNumber = '" + ticketNumber+ "'", con))
                    {
                        //cmd.Parameters.Add(new OleDbParameter("@ticketnumber", OleDbType.VarWChar)).Value = ticketNumber;
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(dt);
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Failed to execute Access ImportSalesByTickets. " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// Sales => Barcode | Style | Color | Description | Size | Vendor | PurchaseDate | Cost | Retail | Discount | SalePrice | CustomerID | EmployeeID | TicketInfo | TicketNumber | Branch |  Discount2
        /// </summary>
        /// <param name="ticketnumber"></param>
        /// <returns>DataTable</returns>
        public DataTable ImportSales(string ticketnumber)
        {
            string query = "Select * FROM Sales WHERE TicketNumber > " + @ticketnumber;
            using (DataTable dt = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(dt);
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Failed to execute Access ImportSales. " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// Sales => Barcode | Style | Color | Description | Size | Vendor | PurchaseDate | Cost | Retail | Discount | SalePrice | CustomerID | EmployeeID | TicketInfo | TicketNumber | Branch |  Discount2
        /// </summary>
        /// <param name="ticketnumber"></param>
        /// <returns></returns>
        public DataTable GetSalesByTicket(string ticketnumber)
        {
            string query = "Select * FROM Sales WHERE TicketNumber = @ticketnumber";
            using (DataTable dt = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.Add(new OleDbParameter("@ticketnumber", OleDbType.Date)).Value = ticketnumber;
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(dt);
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Failed to execute Access ImportSales. " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// ChargeCustomers => CustomerID | Limit | PreviousBalance | ChargeDate | PayDate | MonthsBalance | TotalBalance | Charges | Payments
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ImportChargeCustomers()
        {
            using (DataTable results = new DataTable())
            {
                using (OleDbConnection con = new OleDbConnection(AccessString))
                {
                    using (OleDbCommand cmd = new OleDbCommand("Select * FROM ChargeCustomers", con))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            try
                            {
                                con.Open();
                                adapter.Fill(results);
                                con.Close();
                            }
                            catch (Exception e)
                            {
                                logging.writeToLog("Error: Connect to Access for ChargeCustomers DB : " + e.Message);
                            }
                        }
                        con.Close();
                    }
                }
                return results;
            }
        }


    }
}

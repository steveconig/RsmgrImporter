using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RsmgrImporter
{
    class Tickets
    {
        private Logger logger = new Logger();
        private Conversions conv = new Conversions();
        private SQLGetStatements ss = new SQLGetStatements();
        private SQLInsertStatements si = new SQLInsertStatements();
        private SQLUpdateStatements su = new SQLUpdateStatements();
        private AccessQueries accquer = new AccessQueries();

        /// <summary>
        /// DataTable of Tickets
        /// </summary>
        public DataTable TicketTable = new DataTable();

        public void SetTicketTable(DataTable dt)
        {
            TicketTable = dt;
        }

        public void ClearTicketTable()
        {
            TicketTable.Clear();
        }

        public void ExportTicketsToSQL()
        {
            if (TicketTable.Rows.Count > 0)
            {
                #region Declerations
                decimal total = 0, cost = 0, retail = 0, quantity = 1, discount = 0, discount2 = 0;
                string date, ticketnumber, paymenttype, customerfname, customerlname, employeefname, employeelname, cmdtext = "", dis = "";
                string[] customername;
                string[] employeename;
                int customerid = 1, employeeid = 1, departmentid = 1, ticketid = 1, itemid = 1, itemqty = 1;
                #endregion
                // TicketTable Row
                foreach (DataRow row in TicketTable.Rows)
                {
                    // TODO: Execute imports into SQL.
                    cmdtext = "SELECT TOP 1 * FROM [dbo].[tickets] WHERE invoice_number = '" + row[1].ToString() + "'";
                    if (ss.IfExists(cmdtext))
                    {
                        logger.writeToLog("The invoice_number " + row[1].ToString() + " is already in the SQL Database.");
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
                        si.InsertTicketPayments(ticketid, paymenttype, total); // inserted into SQL => ticket_payments
                        TicketItemTable = accquer.ImportSalesByTicket(ticketnumber); // Import ticket items into database                    
                        int line = 1;
                        // Sales => Barcode, Style, Color, Description, Size, Vendor, PurchaseDate, Cost, Retail, Discount, SalePrice, CustomerID, EmployeeID, TicketInfo, TicketNumber, Branch, Discount2


                    }

                    // !----------------------------------------------------------------------------!
                }
            }
        }

        /// <summary>
        /// DataTable of Ticket Items
        /// </summary>
        public DataTable TicketItemTable = new DataTable();

        public void SetTicketItemTable(DataTable dt)
        {
            TicketItemTable = dt;
        }

        public void ClearTicketItemTable()
        {
            TicketItemTable.Clear();
        }

        public void ExportTicketItemToSQL()
        {
            if (TicketItemTable.Rows.Count > 0)
            {
                #region Declerations
                decimal total = 0, cost = 0, retail = 0, quantity = 1, discount = 0, discount2 = 0;
                string date, ticketnumber, paymenttype, customerfname, customerlname, employeefname, employeelname, cmdtext = "", dis = "";
                string[] customername;
                string[] employeename;
                int customerid = 1, employeeid = 1, departmentid = 1, ticketid = 1, itemid = 1, itemqty = 1, line = 1;
                #endregion
                foreach (DataRow row in TicketItemTable.Rows)
                {
                    
                    // TODO: Execute imports into SQL.
                    itemid = ss.GetItemIDByBarcode(row[0].ToString());
                    dis = row[9].ToString();
                    if (dis != null || dis != "")
                    {
                        if (dis.EndsWith("%")) { dis = dis.TrimEnd(new char[] { '%', ' ' }); }
                        conv.ConvertToDecimal(dis);
                    }
                    else { discount = 0; }
                    dis = row[16].ToString();
                    if (dis != null || dis != "")
                    {
                        if (dis.EndsWith("%")) { dis = dis.TrimEnd(new char[] { '%', ' ' }); }
                        discount2 = conv.ConvertToDecimal(dis);
                    }
                    else { discount2 = 0; }

                    cost = conv.ConvertToDecimal(row[7].ToString());
                    retail = conv.ConvertToDecimal(row[8].ToString());

                    si.InsertTicketItems(ticketid, itemid, "Imported from Access database.", row[0].ToString(), line, quantity, cost, retail, discount, discount2, "");
                    line = line++;
                    // adjust inventory for the item sold.
                    itemqty = Convert.ToInt16(quantity);
                    su.SQLItemQuantityUpdateQty(itemid, departmentid, itemqty, "Changed by TicketInserts");
                }
            }
            ClearTicketItemTable();
        }
    }
}

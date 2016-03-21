using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RsmgrImporter
{
    class TextImporter
    {
        private string writefile = System.AppDomain.CurrentDomain.BaseDirectory + @"output.txt"; // write to output.txt strings it cannot convert
        private Logger logger = new Logger();
        private SQLInsertStatements si = new SQLInsertStatements();
        private SQLGetStatements ss = new SQLGetStatements();
        private SQLUpdateStatements su = new SQLUpdateStatements();
        private string FilePath = null;
        private Char comma = ',';
        private Char space = ' ';

        public void TextToDb()
        {
            // The files used in this example are created in the topic
            // How to: Write to a Text File. You can change the path and
            // file name to substitute text files of your own.

            // Example #1
            // Read the file as one string.
            string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");

            // Display the file contents to the console. Variable text is a string.
            System.Console.WriteLine("Contents of WriteText.txt = {0}", text);

            // Example #2
            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");

            // Display the file contents by using a foreach loop.
            System.Console.WriteLine("Contents of WriteLines2.txt = ");
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        public void InsertTicketsEoD(string text)  // Import Tickets from End Of Day files
        {
            int ticketres = 0;
            string str1 = "";
            str1 = Regex.Match(text, @"\(([^)]*)\)").Groups[1].Value; // '12/05/2015','57690-2',71.34,'Cash','VALUE CUSTOMER ','Anita W'
            text = str1.Replace("'", "");
            string[] subs = text.Split(comma);

            if (subs.Length == 6)
            {
                string cmdtxt = "SELECT TOP 1 * FROM tickets WHERE invoice_number = '" + subs[1] + "'";
                if (ss.IfExists(cmdtxt))
                {
                    DataTable dt = new DataTable();
                    dt = ss.GetDataTableFromQuery(cmdtxt);
                    int employeeid = 1;
                    string custName = subs[4].ToString();
                    string[] namesplit = custName.Split(space);
                    string custFirstName = namesplit[0];
                    string custLastName = namesplit[1];
                    string starttime = "";
                    string completetime = "";
                    int customerid = ss.GetCustomerIDByName(custFirstName, custLastName);
                    string emplName = subs[5].ToString();
                    if (emplName != "")
                    {
                        string[] empNameSplit = new string[2];
                        empNameSplit = emplName.Split(space);
                        employeeid = ss.GetEmployeeIDByName(empNameSplit[0], empNameSplit[1]);
                    }
                    int ticketid = Convert.ToInt32(dt.Rows[0]);
                    if (subs[0] == null || subs[0] == "")
                    {
                        starttime = dt.Rows[1].ToString();
                        completetime = dt.Rows[2].ToString();
                    }
                    else
                    {
                        starttime = subs[0];
                        completetime = subs[0];
                    }
                    string comment = dt.Rows[5].ToString();
                    string paymenttype = dt.Rows[6].ToString();
                    string invoicenumber = dt.Rows[7].ToString();
                    su.UpdateTickets(ticketid, starttime, completetime, customerid, employeeid, "Updated from end of day file", subs[3], invoicenumber);
                }
                else
                {
                    int employeeid = 1;
                    string custName = subs[4].ToString();
                    string[] namesplit = custName.Split(space);
                    string custFirstName = namesplit[0];
                    string custLastName = namesplit[1];
                    int customerid = ss.GetCustomerIDByName(custFirstName, custLastName);
                    string emplName = subs[5].ToString();
                    if (emplName != "")
                    {
                        string[] empNameSplit = new string[2];
                        empNameSplit = emplName.Split(space);
                        employeeid = ss.GetEmployeeIDByName(empNameSplit[0], empNameSplit[1]);
                    }
                    ticketres = si.InsertTicket(subs[0], subs[0], customerid, employeeid, "Entered from end of day file.", subs[3], subs[1]);
                }
            }
            else
            {
                logger.writeToImports(text);
            }

        }

        public void InsertInventoryEoD(string text)  // Imports Inventory from End of Day files
        {
            DataTable dtQuantity = new DataTable();
            int itemid = 0;
            int departmentid = 0;

            string[] commasplit = text.Split(comma);
            dtQuantity = ss.GetItemQuantity(itemid, departmentid);
            if (dtQuantity.Rows.Count > 0)
            {

            }
            // TODO: IfExists item
            // TODO: Else create item
            // TODO: Add or Subtract Item
        }

        public void InsertTicketItemsEoD(string text)
        {
            // TODO: Insert Ticket Items for End of Day
        }

        public void UpdateInventoryEoD(string text)
        {
            // Update Inventory Set Units=(Units-1), Sales=(Sales+1), DateSold=Datevalue('12/05/2015'), Sold=(Sold+1) Where Barcode='0009' and Branch='2'
            //                  0                           1                   2                           3           
            int itemid = 0;
            int departmentid = 0;

            string[] commasplit = text.Split(comma);
            // 0 = Update Inventory Set Units=(Units-1)
            // 1 = Sales=(Sales+1)
            // 2 = DateSold=Datevalue('12/05/2015')
            // 3 = Sold=(Sold+1) Where Barcode='0009' and Branch='2'
            string[] spacesplit0 = commasplit[0].Split(space);
            // 0 = Update Inventory Set Units=(Units-1)
            // 1 = Inventory
            // 2 = Set
            // 3 = Units=(Units-1)
            string[] spacesplit3 = commasplit[3].Split(space);
            // 0 = Sold=(Sold+1)
            // 1 = Where
            // 2 = Barcode='0009'
            // 3 = and
            // 4 = Branch='2'
            string itemnumber = Regex.Match(spacesplit3[2], @"'([^']*)").Groups[1].Value;
            string deptnum = Regex.Match(spacesplit3[4], @"'([^']*)").Groups[1].Value;
            string status = "Updated: " + DateTime.Now.ToString();
            itemid = ss.GetItemIDByBarcode(itemnumber);
            departmentid = ss.GetDepartmentID(deptnum);


            if (itemid != 1 && departmentid != 1)
            {
                DataTable dd = new DataTable();
                dd = ss.GetItemQuantity(itemid, departmentid);
                if (su.SQLItemQuantityUpdateQty(itemid, departmentid, dd.Rows[0].Field<int>("quantity"), status))
                {
                    // Worked 
                }
            }
            else
            {
                // create new item in item quantities
                // si.InsertItemQuantities(itemid, 0, 0, "Active");
            }
        }
    }


}

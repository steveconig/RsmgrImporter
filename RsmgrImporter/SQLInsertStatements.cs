using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace RsmgrImporter
{
    class SQLInsertStatements
    {
        private string DBConLocal = ConfigurationManager.ConnectionStrings["RemoteConnection"].ConnectionString.ToString();
        private Logger logging = new Logger();


        public string InsertAppConfig(string ukey, string value)
        {
            string returnval = "";
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAppConfigNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@ukey", SqlDbType.NVarChar).Value = ukey;
                    cmd.Parameters.Add("@val", SqlDbType.NVarChar).Value = value;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        returnval = "Success";
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        returnval = "Error";
                        logging.writeToLog("Issue with " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnval;
        }

        public int InsertAddress(string line1, string line2, string line3, string city, string state, string zipcode, string country, string details)
        {
            int returnid = 0;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAddressNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@line1", SqlDbType.NVarChar).Value = line1;
                    cmd.Parameters.Add("@line2", SqlDbType.NVarChar).Value = line2;
                    cmd.Parameters.Add("@line3", SqlDbType.NVarChar).Value = line3;
                    cmd.Parameters.Add("@city", SqlDbType.NVarChar).Value = city;
                    cmd.Parameters.Add("@state", SqlDbType.NVarChar).Value = state;
                    cmd.Parameters.Add("@ziporpostcode", SqlDbType.NVarChar).Value = zipcode;
                    cmd.Parameters.Add("@country", SqlDbType.NVarChar).Value = country;
                    cmd.Parameters.Add("@otherdetails", SqlDbType.NVarChar).Value = details;

                    try // Try to execute sql command
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e) // Exceptions
                    {
                        logging.writeToLog("Error: Insert Address : " + e.Message);
                        returnid = 1;
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertPerson(string firstname, string lastname, int addressid, string gender, string homephone, string cellphone, string workphone, string faxnumber, string birthday, string email, string comments, string ssn, string status)
        {
            int returnid = 0;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    DateTime bday = Convert.ToDateTime("01/01/1900");
                    if (gender != "M" || gender != "F") { gender = "U"; }  // gender check
                    if (birthday != null && birthday != "" && birthday.Length > 7) // birthday check
                    {
                        try { bday = Convert.ToDateTime(birthday); }
                        catch (Exception e)
                        {
                            logging.writeToLog("Error: Convert Birthday string to date : " + e.Message + "; Value: " + birthday);
                            bday = Convert.ToDateTime("01/01/1900");
                        }
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spPersonNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
                    cmd.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;
                    cmd.Parameters.Add("@addressid", SqlDbType.Int).Value = addressid;
                    cmd.Parameters.Add("@gender", SqlDbType.NVarChar).Value = gender;
                    cmd.Parameters.Add("@homephone", SqlDbType.NVarChar).Value = homephone;
                    cmd.Parameters.Add("@cellphone", SqlDbType.NVarChar).Value = cellphone;
                    cmd.Parameters.Add("@workphone", SqlDbType.NVarChar).Value = workphone;
                    cmd.Parameters.Add("@faxnumber", SqlDbType.NVarChar).Value = faxnumber;
                    cmd.Parameters.Add("@birthday", SqlDbType.DateTime).Value = bday;
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;
                    cmd.Parameters.Add("@SSN", SqlDbType.NVarChar).Value = ssn;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Insert spPersonNew : " + e.Message);
                        returnid = 1;
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertEmployee(string username, string password, string title, int personid, string startdate, string termdate)
        {
            #region Decelerations

            
            int returnid = 0;
            DateTime sdate = Convert.ToDateTime("01/01/1900");
            DateTime tdate = Convert.ToDateTime("01/01/2100");

            if (startdate != null && startdate != "" && startdate.Length > 7)
            {
                try { sdate = Convert.ToDateTime(startdate); }
                catch (Exception e)
                {
                    logging.writeToLog("Error: Convert Employee.StartDate string to date : " + e.Message + "; Value = " + startdate);
                    sdate = Convert.ToDateTime("01/01/1900");
                }                
            }

            if (termdate != null && termdate != "" && termdate.Length > 7)
            {
                try { tdate = Convert.ToDateTime(termdate); }
                catch (Exception e)
                {
                    logging.writeToLog("Error: Convert Employee.Termdate string to date : " + e.Message + "; Value = " + termdate);
                    tdate = Convert.ToDateTime("01/01/1900");
                }
            }
            #endregion
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.CommandText = "spEmployeeNew";
                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = title;
                    cmd.Parameters.Add("@personid", SqlDbType.NVarChar).Value = personid;
                    cmd.Parameters.Add("@startdate", SqlDbType.DateTime).Value = sdate;
                    cmd.Parameters.Add("@termdate", SqlDbType.DateTime).Value = tdate;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Insert spEmployeeNew : " + e.Message);
                        returnid = 1;
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertCustomer(string accountnumber, int personid, string joindate, int cancharge, decimal chargelimit, int totalvisits, int defaultstoreid, decimal balance, int taxable)
        {
            #region Declerations
            int returnid = 0;
            int storeid = 1;
            DateTime jdate = Convert.ToDateTime("01/01/1900");

            if (accountnumber == null || accountnumber == "") { accountnumber = "unknown"; }
            if (defaultstoreid < 1 || defaultstoreid > 8) { storeid = 1; }
            else { storeid = defaultstoreid; }

            try { jdate = Convert.ToDateTime(joindate); } // join date
            catch (Exception e)
            {
                logging.writeToLog("Error: Convert Customer.Joindate to jdate : " + e.Message + "; Value: " + joindate);
                jdate = Convert.ToDateTime(DateTime.Now);
            }

            if (personid < 1) { personid = 1; }
            if (cancharge != 1) { cancharge = 0; }
            if (taxable != 0) { taxable = 1; }
            #endregion
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCustomerNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@accountnumber", SqlDbType.NVarChar).Value = accountnumber;
                    cmd.Parameters.Add("@personid", SqlDbType.Int).Value = personid;
                    cmd.Parameters.Add("@joindate", SqlDbType.DateTime).Value = jdate;
                    cmd.Parameters.Add("@cancharge", SqlDbType.Int).Value = cancharge;
                    cmd.Parameters.Add("@chargelimit", SqlDbType.Decimal).Value = chargelimit;
                    cmd.Parameters.Add("@totalvisits", SqlDbType.Int).Value = totalvisits;
                    cmd.Parameters.Add("@defaultstoreid", SqlDbType.Int).Value = storeid;
                    cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = balance;
                    cmd.Parameters.Add("@taxable", SqlDbType.Int).Value = taxable;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Insert spCustomerNew : " + e.Message);
                        returnid = 1;
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertCategory(string name, string description, int inuse)
        {
            int returnid = 0;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCategoryNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@inuse", SqlDbType.Int).Value = inuse;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: InsertCategory failed. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertDepartment(string name, string number, int addressid, string comments, string status)
        {
            #region Declerations
            int returnid = 0;
            if (status.Length > 25)
            { 
                status = status.Trim();
                status = status.Substring(0, 25);
            }
            #endregion
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDepartmentNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@departmentname", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@departmentnumber", SqlDbType.NVarChar).Value = number;
                    cmd.Parameters.Add("@addressid", SqlDbType.Int).Value = addressid;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Executing statement spDepartmentNew in InsertDepartment. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertGiftcard(string giftcardnumber, string recordtime, decimal value, string status, int customerid, int employeeid)
        {
            #region Declerations
            int returnid = 0;
            DateTime rtime = DateTime.Now;

            if (recordtime != null && recordtime != "")
            {
                try { rtime = Convert.ToDateTime(recordtime); }
                catch (Exception e)
                {
                    logging.writeToLog("Error: Unable to convert recordtime: " + recordtime + " to DateTime. " + e.Message);
                }
            }

            if (status.Length > 25)
            {
                status = status.Trim();
                status = status.Substring(0, 25);
            }
            #endregion
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGiftcardNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@giftcardnumber", SqlDbType.NVarChar).Value = giftcardnumber;
                    cmd.Parameters.Add("@recordtime", SqlDbType.DateTime).Value = rtime;
                    cmd.Parameters.Add("@value", SqlDbType.Decimal).Value = value;
                    cmd.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.Int).Value = employeeid;
                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute SqlInsertStatements.InsertGiftCard.spGiftcardNew. " + e.Message); 
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertInventoryLog(int itemid, string actiontime, int departmentid, int employeeid)
        {
            int returnid = 0;

            DateTime atime = DateTime.Now;

            if (actiontime != null && actiontime != "")
            {
                atime = Convert.ToDateTime(actiontime);
            }

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInventoryLogsNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@itemid", SqlDbType.NVarChar).Value = itemid;
                    cmd.Parameters.Add("@actiontime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@departmentid", SqlDbType.Int).Value = departmentid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.Int).Value = employeeid;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to insert into SqlInsertStatements.InsertInventoryLog. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertInvoiceItems(int invoiceid, int itemid, string description, string serialnumber, int line, decimal quantitypurchased, decimal itemcostprice, decimal itemunitprice, decimal discountpercent, int itemlocation, int invoicequantity)
        {
            int returnid = 0;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInvoiceItemsNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@invoiceid", SqlDbType.Int).Value = invoiceid;
                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@serialnumber", SqlDbType.NVarChar).Value = serialnumber;
                    cmd.Parameters.Add("@line", SqlDbType.Int).Value = line;
                    cmd.Parameters.Add("@quantitypurchased", SqlDbType.Decimal).Value = quantitypurchased;
                    cmd.Parameters.Add("@itemcostprice", SqlDbType.Decimal).Value = itemcostprice;
                    cmd.Parameters.Add("@itemunitprice", SqlDbType.Decimal).Value = itemunitprice;
                    cmd.Parameters.Add("@discountpercent", SqlDbType.Decimal).Value = discountpercent;
                    cmd.Parameters.Add("@itemlocation", SqlDbType.Int).Value = itemlocation;
                    cmd.Parameters.Add("@invoicequantity", SqlDbType.Int).Value = invoicequantity;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute SqlInsertStatements.InsertInvoiceItems. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertInvoices(string invoicenumber, string ordertime, string receivetime, int vendorid, int employeeid, string comment, string paymenttype)
        {
            int returnid = 0;

            DateTime atime = DateTime.Now;
            DateTime btime = DateTime.Now;

            if (ordertime != null && ordertime != "") {
                try { atime = Convert.ToDateTime(ordertime); }
                catch (Exception e) { logging.writeToLog("Warning: Can not convert receivetime: " + ordertime + " to datetime. " + e.Message); }
            }
            if (receivetime != null && receivetime != "")
            {
                try { btime = Convert.ToDateTime(receivetime); }
                catch (Exception e) { logging.writeToLog("Warning: Can not convert receivetime: " + receivetime + " to datetime. " + e.Message); }
            }

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInvoiceNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@invoicenumber", SqlDbType.NVarChar).Value = invoicenumber;
                    cmd.Parameters.Add("@ordertime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@receivetime", SqlDbType.DateTime).Value = btime;
                    cmd.Parameters.Add("@vendorid", SqlDbType.Int).Value = vendorid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.Int).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Can not execute SqlInsertStatements.InsertInvoices.spInvoiceNew. " + e.Message);
                        returnid = 1;
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertItemQuantities(int itemid, int departmentid, int quantity, string status)
        {
            int returnid = 0;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@departmentid", SqlDbType.Int).Value = departmentid;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertItemQuantities.spItemQuantitiesNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertItems(string name, int categoryid, string barcode, string description, decimal costprice, decimal unitprice, decimal reorderlevel, decimal defaulttax, int receivingquantity, int picid, string status, string custom1, string custom2, string custom3, string custom4, string custom5, string custom6, string custom7, string custom8, string custom9, string custom10)
        {
            int returnid = 0;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemsNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@categoryid", SqlDbType.Int).Value = categoryid;
                    cmd.Parameters.Add("@barcode", SqlDbType.NVarChar).Value = barcode;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@costprice", SqlDbType.Decimal).Value = costprice;
                    cmd.Parameters.Add("@unitprice", SqlDbType.Decimal).Value = unitprice;
                    cmd.Parameters.Add("@reorderlevel", SqlDbType.Decimal).Value = reorderlevel;
                    cmd.Parameters.Add("@defaulttax", SqlDbType.Decimal).Value = defaulttax;
                    cmd.Parameters.Add("@receivingquantity", SqlDbType.Int).Value = receivingquantity;
                    cmd.Parameters.Add("@picid", SqlDbType.Int).Value = picid;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    cmd.Parameters.Add("@custom1", SqlDbType.NVarChar).Value = custom1;
                    cmd.Parameters.Add("@custom2", SqlDbType.NVarChar).Value = custom2;
                    cmd.Parameters.Add("@custom3", SqlDbType.NVarChar).Value = custom3;
                    cmd.Parameters.Add("@custom4", SqlDbType.NVarChar).Value = custom4;
                    cmd.Parameters.Add("@custom5", SqlDbType.NVarChar).Value = custom5;
                    cmd.Parameters.Add("@custom6", SqlDbType.NVarChar).Value = custom6;
                    cmd.Parameters.Add("@custom7", SqlDbType.NVarChar).Value = custom7;
                    cmd.Parameters.Add("@custom8", SqlDbType.NVarChar).Value = custom8;
                    cmd.Parameters.Add("@custom9", SqlDbType.NVarChar).Value = custom9;
                    cmd.Parameters.Add("@custom10", SqlDbType.NVarChar).Value = custom10;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertItems.spItemNew. " + e.Message);
                    }
                    con.Close();
                }
            }                   
            return returnid;
        }

        public int InsertItemTaxes(int itemid, string name, decimal percent)
        {
            int returnid = 0;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemTaxesNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@percent", SqlDbType.Decimal).Value = percent;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertItemTaxes.spItemTaxesNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;            
        }

        public int InsertTicketItems(int ticketid, int itemid, string description, string serialnumber, int line, decimal quantitypurchased, decimal itemcostprice, decimal itemunitprice, string status)
        {
            int returnid = 0;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketItemsNew";
                    cmd.Connection = con;

                    cmd.Parameters.Add("@ticketid", SqlDbType.Int).Value = ticketid;
                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@serialnumber", SqlDbType.NVarChar).Value = serialnumber;
                    cmd.Parameters.Add("@line", SqlDbType.Int).Value = line;
                    cmd.Parameters.Add("@quantitypurchased", SqlDbType.Decimal).Value = quantitypurchased;
                    cmd.Parameters.Add("@itemcostprice", SqlDbType.Decimal).Value = itemcostprice;
                    cmd.Parameters.Add("@itemunitprice", SqlDbType.Decimal).Value = itemunitprice;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketItems.spTicketItemsNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertTicket(string starttime, string completetime, int customerid, int employeeid, string comment, string paymenttype, string invoicenumber)
        {
            int returnid = 0;
            DateTime atime = DateTime.Now;
            DateTime btime = DateTime.Now;

            if (starttime != null && starttime != "")
            {
                atime = Convert.ToDateTime(starttime);
            }

            if (completetime != null && completetime != "")
            {
                btime = Convert.ToDateTime(completetime);
            }

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@starttime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@completetime", SqlDbType.DateTime).Value = btime;
                    cmd.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.Int).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    cmd.Parameters.Add("@invoicenumber", SqlDbType.NVarChar).Value = invoicenumber;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicket.spTicketNew. " + e.Message);
                    }
                    con.Close();
                }
            }            
            return returnid;
        }

        public int InsertTicketItemTaxes(int ticketid, int itemid, int line, string name, decimal percent)
        {
            int returnid = 0;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketItemTaxesNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ticketid", SqlDbType.Int).Value = ticketid;
                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@line", SqlDbType.Int).Value = line;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@percent", SqlDbType.Decimal).Value = percent;
                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketItemTaxes.spTicketItemTaxesNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertTicketPayments(int ticketid, string paymenttype, decimal paymentamount)
        {
            #region Declerations
            int returnid = 0;
            #endregion
            using(SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketPaymentsNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ticketid", SqlDbType.Int).Value = ticketid;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    cmd.Parameters.Add("@paymentamount", SqlDbType.Decimal).Value = paymentamount;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketPayments.spTicketPaymentsNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertVendors(string name, string vendornumber, string phone, int addressid, int personid, string comments)
        {
            #region Declerations
            int returnid = 0;
            #endregion
            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spVendorsNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@vendornumber", SqlDbType.NVarChar).Value = vendornumber;
                    cmd.Parameters.Add("@phone", SqlDbType.NVarChar).Value = phone;
                    cmd.Parameters.Add("@addressid", SqlDbType.Int).Value = addressid;
                    cmd.Parameters.Add("@personid", SqlDbType.Int).Value = personid;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertVendors.spVendorsNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public int InsertTransfer(int itemid, int employeeid, string transdate, string comment, int departmentfromid, int departmenttoid, int completed)
        {
            #region Declerations
            int returnid = 0;
            DateTime adate = DateTime.Now;

            if (transdate != null && transdate != "")
            {
                adate = Convert.ToDateTime(transdate);
            }
            #endregion

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTransferNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.Int).Value = employeeid;
                    cmd.Parameters.Add("@transdate", SqlDbType.NVarChar).Value = adate;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@departmentfromid", SqlDbType.Int).Value = departmentfromid;
                    cmd.Parameters.Add("@departmenttoid", SqlDbType.Int).Value = departmenttoid;
                    cmd.Parameters.Add("@completed", SqlDbType.Int).Value = completed;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTransfer.spTransferNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }


        /// <summary>
        /// This works with stored procedures in order to produce deleted results
        /// </summary>
        /// <param name="param">@parm from stored procedure</param>
        /// <param name="id">the value of the @param</param>
        /// <param name="cmdtxt">name of the stored procedure</param>
        /// <returns>Either 1 for success or 0 for unsuccessful</returns>
        public int DeleteCommand(string param, int id, string cmdtxt)

        {
            int returnid = 0;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = cmdtxt;
                    cmd.Connection = con;
                    cmd.Parameters.Add(param, SqlDbType.Int).Value = id;

                    try
                    {
                        con.Open();
                        returnid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute DeleteCommand. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace RsmgrImporter
{
    class SQLUpdates
    {

        private string DBConLocal = ConfigurationManager.ConnectionStrings["RemoteConnection"].ConnectionString;
        private Logger logging = new Logger();

        public bool UpdateAddress(string id, string line1, string line2, string line3, string city, string state, string zipcode, string country, string details)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAddressChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@line1", SqlDbType.NVarChar).Value = line1;
                    cmd.Parameters.Add("@line2", SqlDbType.NVarChar).Value = line2;
                    cmd.Parameters.Add("@line3", SqlDbType.NVarChar).Value = line3;
                    cmd.Parameters.Add("@city", SqlDbType.NVarChar).Value = city;
                    cmd.Parameters.Add("@state", SqlDbType.NVarChar).Value = state;
                    cmd.Parameters.Add("@ziporpostcode", SqlDbType.NVarChar).Value = zipcode;
                    cmd.Parameters.Add("@country", SqlDbType.NVarChar).Value = country;
                    cmd.Parameters.Add("@otherdetails", SqlDbType.NVarChar).Value = details;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateAddress | " + id + ", " + line1 + ", " + line2 + ", " + line3 + ", " + city + ", " + state + ", " + zipcode + ", " + country + ", " + details);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spAddressChange failed to execute : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateAppConfig(string ukey, string value)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAppConfigChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ukey", SqlDbType.NVarChar).Value = ukey;
                    cmd.Parameters.Add("@val", SqlDbType.NVarChar).Value = value;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateAppConfig | " + ukey + ", " + value);

                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spAppConfigChange failed to execute : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }
        
        public bool UpdateCategory(string id, string name, string description, int inuse)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCategoryChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@inuse", SqlDbType.Int).Value = inuse;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateCategory | " + id + ", " + name + ", " + description + ", " + inuse);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spCategoryChange failed to execute : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateCustomer(string id, string accountnumber, string personid, string joindate, int cancharge, decimal chargelimit, int totalvisits, string defaultstoreid, decimal balance, int taxable)
        {

            bool retvalue = false;
            DateTime jdate = dtconverter(joindate, 0);

            if (accountnumber == null || accountnumber == "") { accountnumber = "unknown"; }
            
            if (cancharge != 1) { cancharge = 0; }
            if (taxable != 0) { taxable = 1; }
            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCustomerChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@accountnumber", SqlDbType.NVarChar).Value = accountnumber;
                    cmd.Parameters.Add("@personid", SqlDbType.UniqueIdentifier).Value = personid;
                    cmd.Parameters.Add("@joindate", SqlDbType.DateTime).Value = jdate;
                    cmd.Parameters.Add("@cancharge", SqlDbType.Int).Value = cancharge;
                    cmd.Parameters.Add("@chargelimit", SqlDbType.Decimal).Value = chargelimit;
                    cmd.Parameters.Add("@totalvisits", SqlDbType.Int).Value = totalvisits;
                    cmd.Parameters.Add("@defaultstoreid", SqlDbType.UniqueIdentifier).Value = defaultstoreid;
                    cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = balance;
                    cmd.Parameters.Add("@taxable", SqlDbType.Int).Value = taxable;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateCustomer | " + id + ", " + accountnumber + ", " + personid + ", " + jdate + ", " + cancharge + ", " + chargelimit + ", " + totalvisits + ", " + defaultstoreid + ", " + balance + ", " + taxable);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spCategoryChange failed to execute : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateDepartment(string id, string name, string number, string addressid, string comments, string status)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDepartmentChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@departmentname", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@departmentnumber", SqlDbType.NVarChar).Value = number;
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateDepartment | " + id + ", " + name + ", " + number + ", " + addressid + ", " + comments + ", " + status);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spDepartmentUpdate failed to execute : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateEmployee(string id, string username, string password, string title, string personid, string startdate, string termdate)
        {
            bool retvalue = false;
            DateTime sdate = dtconverter(startdate, 0);
            DateTime tdate = dtconverter(termdate, 0);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDepartmentChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = title;
                    cmd.Parameters.Add("@personid", SqlDbType.UniqueIdentifier).Value = personid;
                    cmd.Parameters.Add("@startdate", SqlDbType.DateTime).Value = sdate;
                    cmd.Parameters.Add("@termdate", SqlDbType.DateTime).Value = tdate;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateDepartment | " + id + ", " + username + ", " + password + ", " + title + ", " + personid + ", " + sdate + ", " + tdate);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spDepartmentUpdate failed to execute : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateGiftCards(string giftcardid, string giftcardnumber, string recordtime, decimal value, decimal balance, string status, string customerid, string employeeid)
        {
            bool retvalue = false;
            DateTime adate = dtconverter(recordtime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGiftcardNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = giftcardid;
                    cmd.Parameters.Add("@giftcardnumber", SqlDbType.NVarChar).Value = giftcardnumber;
                    cmd.Parameters.Add("@recordtime", SqlDbType.NVarChar).Value = adate;
                    cmd.Parameters.Add("@value", SqlDbType.Decimal).Value = value;
                    cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = balance;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateGiftCards | " + giftcardid + ", " + giftcardnumber + ", " + adate + ", " + value + ", " + balance + ", " + status + ", " + customerid + ", " + employeeid);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Unable to execute InsertTransfer.spTransferNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateGiftCardBalance(string giftcardid, decimal balance, string status)
        {
            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGiftcardNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = giftcardid;
                    cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = balance;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateGiftCardBalance | " + giftcardid + ", " + balance + ", " + status);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTransfer. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        // InventoryLog

        // InvoiceItems

        // Invoices

        public bool UpdateItem(int itemid, string name, int categoryid, string barcode, string description, decimal costprice, decimal unitprice, decimal reorderlevel, decimal defaulttax, int receivingquantity, int picid, string status, string custom1, string custom2, string custom3, string custom4, string custom5, string custom6, string custom7, string custom8, string custom9, string custom10)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = itemid;
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
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateItem | " + itemid + ", " + name + ", " + categoryid + ", " + barcode + ", " + description + ", " + costprice + ", " + unitprice + ", " + reorderlevel + ", " + defaulttax + ", " + receivingquantity + ", " + picid + ", " + status + ", " + custom1 + ", " + custom2 + ", " + custom3 + ", " + custom4 + ", " + custom5 + ", " + custom6 + ", " + custom7 + ", " + custom8 + ", " + custom9 + ", " + custom10);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Unable to execute SQLUpdates.UpdateItemQuantities: " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateItemQuantities(string itemid, string departmentid, int quantity, string status)
        {
            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@departmentid", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateItemQuantities | " + itemid + ", " + departmentid + ", " + quantity + ", " + status);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute SQLUpdates.UpdateItemQuantities: " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        // ItemTaxes

        // Layaway

        public bool UpdateLayaway(string id, string ticketid, string actionitem, string departmentid, string employeeid, string transactiontext)
        {
            bool retvalue = false;
            DateTime atime = dtconverter(actionitem, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spLayawayChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@ticketid", SqlDbType.UniqueIdentifier).Value = ticketid;
                    cmd.Parameters.Add("@actiontime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@departmentid", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@transactiontext", SqlDbType.NVarChar).Value = transactiontext;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateGiftCardBalance | " + id + ", " + ticketid + ", " + atime + ", " + departmentid + ", " + employeeid + ", " + transactiontext);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTransfer. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        // TicketItems

        // TicketItemTaxes

        // TicketPayments
        
        // Transfers

        // Vendors

        public bool UpdatePerson(string id, string firstname, string lastname, string addressid, string gender, string homephone, string cellphone, string workphone, string faxnumber, string birthday, string email, string comments, string ssn, string status)
        {
            bool retvalue = false;
            DateTime bday = dtconverter(birthday, 0);
            if (gender != "M" || gender != "F") { gender = "U"; }  // gender check

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spPersonChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
                    cmd.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
                    cmd.Parameters.Add("@gender", SqlDbType.NVarChar).Value = gender; // M or F
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
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdatePerson | " + id + ", " + firstname + ", " + lastname + ", " + addressid + ", " + gender + ", " + homephone + ", " + cellphone + ", " + workphone + ", " + faxnumber + ", " + bday + ", " + email + ", " + comments + ", " + ssn + ", " + status);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spPersonChange failed to execute : " + e.Message);
                        retvalue = false;
                    }

                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateTicket(string id, string completetime, string customerid, string employeeid, string comment, string paymenttype)
        {
            bool retvalue = false;
            DateTime atime = dtconverter(completetime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@completetime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateTicketComplete | " + id + ", " + atime + ", " + customerid + ", " + employeeid + ", " + comment + ", " + paymenttype);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute SQLUpdates.SQLTicketUpdate: " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public bool UpdateTicketComplete(string id, string completetime, string customerid, string employeeid, string comment, string paymenttype)
        {
            bool retvalue = false;
            DateTime atime = dtconverter(completetime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketChangeComplete";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                    cmd.Parameters.Add("@completetime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("UpdateTicket | " + id + ", " + atime + ", " + customerid + ", " + employeeid + ", " + comment + ", " + paymenttype);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute SQLUpdates.SQLTicketUpdate: " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

       

        /// <summary>
        /// Converts a string to a DateTime or produces a default value if conversion fails.
        /// </summary>
        /// <param name="dtime">The string to convert to a datetime value.</param>
        /// <param name="now">If 1 will return default datetime.now if failure of conversion.</param>
        /// <returns>The converted DateTime value or default.</returns>
        private DateTime dtconverter(string dtime, int now)
        {
            DateTime retvalue = DateTime.Now;
            if (now != 1) { retvalue = Convert.ToDateTime("01/01/1900"); }

            if (DateTime.TryParse(dtime, out retvalue))
            {
                retvalue = Convert.ToDateTime(dtime);
            }
            else
            {
                logging.writeToLog("Error: Unable to convert string to date; Value = " + dtime);
            }
            return retvalue;
        }
    }
}

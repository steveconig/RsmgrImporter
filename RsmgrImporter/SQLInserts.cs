using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace RsmgrImporter
{
    class SQLInserts
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
                        // AppConfig is location dependent, may need to review how this is handled.
                        // Not sending as an import for now.
                        //logging.writeToImports("InsertAppConfig | " + ukey + ", " + value);
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

        public string InsertAddress(string line1, string line2, string line3, string city, string state, string zipcode, string country, string details)
        {
            string returnid = "";
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
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertAddress | " + returnid + ", " + line1 + ", " + line2 + ", " + line3 + ", " + city + ", " + state + ", " + zipcode + ", " + country + ", " + details);
                    }
                    catch (Exception e) // Exceptions
                    {
                        logging.writeToLog("Error: Insert Address : " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertAddressWithID(string addressid, string line1, string line2, string line3, string city, string state, string zipcode, string country, string details)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAddressNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = addressid;
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
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToExports("InsertAddressWithID | " + addressid + ", " + line1 + ", " + line2 + ", " + line3 + ", " + city + ", " + state + ", " + zipcode + ", " + country + ", " + details);
                    }
                    catch (Exception e) // Exceptions
                    {
                        logging.writeToLog("Error: Insert Address : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertPerson(string firstname, string lastname, string addressid, string gender, string homephone, string cellphone, string workphone, string faxnumber, string birthday, string email, string comments, string ssn, string status)
        {
            string returnid = "";
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    DateTime bday = dtconverter(birthday, 0);
                    
                    // gender check
                    if (gender != "M" || gender != "F")
                    {
                        gender = "U";
                    }  
                                            
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spPersonNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
                    cmd.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
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
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertPersonWithID | " + returnid + ", " + firstname + ", " + lastname + ", " + addressid + ", " + gender + ", " + homephone + ", " + cellphone + ", " + workphone + ", " + faxnumber + ", " + bday + ", " + email + ", " + comments + ", " + ssn + ", " + status);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Insert spPersonNew : " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertPersonWithID(string personid, string firstname, string lastname, int addressid, string gender, string homephone, string cellphone, string workphone, string faxnumber, string birthday, string email, string comments, string ssn, string status)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    DateTime bday = dtconverter(birthday, 0);

                    // gender check
                    if (gender != "M" || gender != "F")
                    {
                        gender = "U";
                    }
                    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spPersonNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = personid;
                    cmd.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
                    cmd.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
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
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertPersonWithID | " + personid + ", " + firstname + ", " + lastname + ", " + addressid + ", " + gender + ", " + homephone + ", " + cellphone + ", " + workphone + ", " + faxnumber + ", " + bday + ", " + email + ", " + comments + ", " + ssn + ", " + status);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Insert spPersonNew : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertEmployee(string username, string password, string title, string personid, string startdate, string termdate)
        {
            string returnid = "";
            DateTime sdate = dtconverter(startdate, 0);
            DateTime tdate = dtconverter(termdate, 0);

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
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertEmployee | " + returnid + ", "+  username + ", " + password + ", " + title + ", " + personid + ", " + sdate + ", " + tdate);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Insert spEmployeeNew : " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertEmployeeWithID(string employeeid, string username, string password, string title, string personid, string startdate, string termdate)
        {
            bool returnid = false;
            DateTime sdate = dtconverter(startdate, 0);
            DateTime tdate = dtconverter(termdate, 0);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.CommandText = "spEmployeeNewWithID";
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = employeeid;
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
                        returnid = true;
                        logging.writeToImports("InsertEmployeeWithID | " + employeeid + ", " + username + ", " + password + ", " + title + ", " + personid + ", " + sdate + ", " + tdate);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Insert spEmployeeNew : " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public string InsertCustomer(string accountnumber, string personid, string joindate, int cancharge, decimal chargelimit, int totalvisits, string defaultstoreid, decimal balance, int taxable)
        {
            
            string returnid = "";
            DateTime jdate = dtconverter(joindate, 0);

            if (accountnumber == null || accountnumber == "") { accountnumber = "unknown"; }
            
            if (cancharge != 1) { cancharge = 0; }
            if (taxable != 0) { taxable = 1; }
            
            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCustomerNew";
                    cmd.Connection = con;
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
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertCustomer | " + returnid + ", " + accountnumber + ", " + personid + ", " + jdate + ", " + cancharge + ", " + chargelimit + ", " + totalvisits + ", " + defaultstoreid + ", " + balance + ", " + taxable);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Insert spCustomerNew : " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertCustomerWithID(string customerid, string accountnumber, string personid, string joindate, int cancharge, decimal chargelimit, int totalvisits, string defaultstoreid, decimal balance, int taxable)
        {
            bool retvalue = false;
            DateTime jdate = dtconverter(joindate, 0);

            if (accountnumber == null || accountnumber == "") { accountnumber = "unknown"; }

            if (cancharge != 1) { cancharge = 0; }
            if (taxable != 0) { taxable = 1; }

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCustomerNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = customerid;
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
                        logging.writeToImports("InsertCustomer | " + customerid + ", " + accountnumber + ", " + personid + ", " + jdate + ", " + cancharge + ", " + chargelimit + ", " + totalvisits + ", " + defaultstoreid + ", " + balance + ", " + taxable);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Insert spCustomerNew : " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertCategory(string name, string description, int inuse)
        {
            string returnid = "";

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
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertCategory | " + returnid  + ", " + name + ", " + description + ", " + inuse);
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

        public bool InsertCategoryWithID(string categoryid, string name, string description, int inuse)
        {
            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCategoryNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = categoryid;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@inuse", SqlDbType.Int).Value = inuse;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertCategory | " + categoryid + ", " + name + ", " + description + ", " + inuse);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: InsertCategory failed. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertDepartment(string name, string number, string addressid, string comments, string status)
        {
            string returnid = "";
            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDepartmentNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@departmentname", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@departmentnumber", SqlDbType.NVarChar).Value = number;
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertDepartment | " + returnid + ", " + name + ", " + number + ", " + addressid + ", " + comments + ", " + status);
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

        public string InsertDepartmentWithID(string departmentid, string name, string number, string addressid, string comments, string status)
        {
            #region Declerations
            string returnid = "";

            #endregion
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDepartmentNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@departmentname", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@departmentnumber", SqlDbType.NVarChar).Value = number;
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertDepartment | " + departmentid + ", " + name + ", " + number + ", " + addressid + ", " + comments + ", " + status);
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

        public string InsertGiftcard(string giftcardnumber, string recordtime, decimal value, string status, string customerid, string employeeid)
        {
            string returnid = "";
            DateTime rtime = dtconverter(recordtime, 1);

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
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertGiftcard | " + returnid + ", " + giftcardnumber + ", " + rtime + ", " + value + ", " + customerid + ", " + employeeid);
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

        public bool InsertGiftcardWithID(string giftcardid, string giftcardnumber, string recordtime, decimal value, string status, string customerid, string employeeid)
        {
            bool retvalue = false;
            DateTime rtime = dtconverter(recordtime, 1);
            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGiftcardNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = giftcardid;
                    cmd.Parameters.Add("@giftcardnumber", SqlDbType.NVarChar).Value = giftcardnumber;
                    cmd.Parameters.Add("@recordtime", SqlDbType.DateTime).Value = rtime;
                    cmd.Parameters.Add("@value", SqlDbType.Decimal).Value = value;
                    cmd.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.Int).Value = employeeid;
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertGiftcard | " + giftcardid + ", " + giftcardnumber + ", " + rtime + ", " + value + ", " + customerid + ", " + employeeid);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute SqlInsertStatements.InsertGiftCard.spGiftcardNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertInventoryLog(string itemid, string actiontime, string departmentid, string employeeid, string transactiontext)
        {
            string returnid = "";
            DateTime atime = dtconverter(actiontime, 1);
            
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
                    cmd.Parameters.Add("@transactiontext", SqlDbType.NVarChar).Value = transactiontext;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertInventoryLog | " + itemid + ", " + atime + ", " + departmentid + ", " + employeeid + ", " + transactiontext);
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

        public bool InsertInventoryLogWithID(string inventoryid, string itemid, string actiontime, string departmentid, string employeeid, string transactiontext)
        {
            bool retvalue = false;
            DateTime atime = dtconverter(actiontime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInventoryLogsNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = inventoryid;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@actiontime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@departmentid", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@transactiontext", SqlDbType.NVarChar).Value = transactiontext;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertInventoryLog | " + inventoryid + ", " + itemid + ", " + atime + ", " + departmentid + ", " + employeeid + ", " + transactiontext);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to insert into SqlInsertStatements.InsertInventoryLog. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertInvoiceItems(string invoiceid, string itemid, string description, string serialnumber, int line, decimal quantitypurchased, decimal itemcostprice, decimal itemunitprice, decimal discountpercent, string departmentid, int invoicequantity)
        {
            string returnid = "";
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInvoiceItemsNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@invoiceid", SqlDbType.UniqueIdentifier).Value = invoiceid;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@serialnumber", SqlDbType.NVarChar).Value = serialnumber;
                    cmd.Parameters.Add("@line", SqlDbType.Int).Value = line;
                    cmd.Parameters.Add("@quantitypurchased", SqlDbType.Decimal).Value = quantitypurchased;
                    cmd.Parameters.Add("@itemcostprice", SqlDbType.Decimal).Value = itemcostprice;
                    cmd.Parameters.Add("@itemunitprice", SqlDbType.Decimal).Value = itemunitprice;
                    cmd.Parameters.Add("@discountpercent", SqlDbType.Decimal).Value = discountpercent;
                    cmd.Parameters.Add("@itemlocation", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@invoicequantity", SqlDbType.Int).Value = invoicequantity;
                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertInvoiceItems | " + invoiceid + ", " + itemid + ", " + description + ", " + serialnumber + ", " + line + ", " + quantitypurchased + ", " + itemcostprice + ", " + itemunitprice + ", " + discountpercent + ", " + departmentid + ", " + invoicequantity);
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

        public bool InsertInvoiceItemsWithID(string invoiceitemid, string invoiceid, string itemid, string description, string serialnumber, int line, decimal quantitypurchased, decimal itemcostprice, decimal itemunitprice, decimal discountpercent, int departmentid, int invoicequantity)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInvoiceItemsNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = invoiceitemid;
                    cmd.Parameters.Add("@invoiceid", SqlDbType.UniqueIdentifier).Value = invoiceid;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@serialnumber", SqlDbType.NVarChar).Value = serialnumber;
                    cmd.Parameters.Add("@line", SqlDbType.Int).Value = line;
                    cmd.Parameters.Add("@quantitypurchased", SqlDbType.Decimal).Value = quantitypurchased;
                    cmd.Parameters.Add("@itemcostprice", SqlDbType.Decimal).Value = itemcostprice;
                    cmd.Parameters.Add("@itemunitprice", SqlDbType.Decimal).Value = itemunitprice;
                    cmd.Parameters.Add("@discountpercent", SqlDbType.Decimal).Value = discountpercent;
                    cmd.Parameters.Add("@itemlocation", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@invoicequantity", SqlDbType.Int).Value = invoicequantity;
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertInvoiceItems | " + invoiceitemid + ", "+ invoiceid + ", " + itemid + ", " + description + ", " + serialnumber + ", " + line + ", " + quantitypurchased + ", " + itemcostprice + ", " + itemunitprice + ", " + discountpercent + ", " + departmentid + ", " + invoicequantity);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute SqlInsertStatements.InsertInvoiceItems. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertInvoices(string invoicenumber, string ordertime, string receivetime, string vendorid, string employeeid, string comment, string paymenttype)
        {
            string returnid = "";
            DateTime atime = dtconverter(ordertime, 1);
            DateTime btime = dtconverter(receivetime, 1);

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
                    cmd.Parameters.Add("@vendorid", SqlDbType.UniqueIdentifier).Value = vendorid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertInvoices | " + returnid + ", " + invoicenumber + ", " + atime + ", " + btime + ", " + vendorid + ", " + employeeid + ", " + comment + ", " + paymenttype);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Can not execute SqlInsertStatements.InsertInvoices.spInvoiceNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertInvoicesWithID(string invoiceid, string invoicenumber, string ordertime, string receivetime, string vendorid, string employeeid, string comment, string paymenttype)
        {
            bool retvalue = false;
            DateTime atime = dtconverter(ordertime, 1);
            DateTime btime = dtconverter(receivetime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInvoiceNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = invoiceid;
                    cmd.Parameters.Add("@invoicenumber", SqlDbType.NVarChar).Value = invoicenumber;
                    cmd.Parameters.Add("@ordertime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@receivetime", SqlDbType.DateTime).Value = btime;
                    cmd.Parameters.Add("@vendorid", SqlDbType.UniqueIdentifier).Value = vendorid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    try
                    {
                        con.Open();

                        retvalue = true;
                        logging.writeToImports("InsertInvoicesWithID | " + invoiceid + ", " + invoicenumber + ", " + atime + ", " + btime + ", " + vendorid + ", " + employeeid + ", " + comment + ", " + paymenttype);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Can not execute SqlInsertStatements.InsertInvoices.spInvoiceNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertItemQuantities(string itemid, string departmentid, int quantity, string status)
        {
            string returnid = "";

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@departmentid", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertItemQuantities | " + returnid + ", " + itemid + ", " + departmentid + ", " + quantity + ", " + status);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Unable to execute InsertItemQuantities.spItemQuantitiesNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertItemQuantitiesWithID(string itemquantityid, string itemid, string departmentid, int quantity, string status)
        {
            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = itemquantityid;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@departmentid", SqlDbType.UniqueIdentifier).Value = departmentid;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertItemQuantitiesWithID | " + itemquantityid + ", " + itemid + ", " + departmentid + ", " + quantity + ", " + status);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Unable to execute InsertItemQuantities.spItemQuantitiesNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertItems(string name, string categoryid, string barcode, string description, decimal costprice, decimal unitprice, decimal reorderlevel, decimal defaulttax, int receivingquantity, string picid, string status, string custom1, string custom2, string custom3, string custom4, string custom5, string custom6, string custom7, string custom8, string custom9, string custom10)
        {
            string returnid = "";
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemsNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@categoryid", SqlDbType.UniqueIdentifier).Value = categoryid;
                    cmd.Parameters.Add("@barcode", SqlDbType.NVarChar).Value = barcode;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@costprice", SqlDbType.Decimal).Value = costprice;
                    cmd.Parameters.Add("@unitprice", SqlDbType.Decimal).Value = unitprice;
                    cmd.Parameters.Add("@reorderlevel", SqlDbType.Decimal).Value = reorderlevel;
                    cmd.Parameters.Add("@defaulttax", SqlDbType.Decimal).Value = defaulttax;
                    cmd.Parameters.Add("@receivingquantity", SqlDbType.Int).Value = receivingquantity;
                    cmd.Parameters.Add("@picid", SqlDbType.UniqueIdentifier).Value = picid;
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
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertItems | " + returnid + ", " + name + ", " + categoryid + ", " + barcode + ", " + description + ", " + costprice + ", " + unitprice + ", " + reorderlevel + ", " + defaulttax + ", " + receivingquantity + ", " + defaulttax + ", " + receivingquantity + ", " + picid + ", " + status + ", " + custom1 + ", " + custom2 + ", " + custom3 + ", " + custom4 + ", " + custom5 + ", " + custom6 + ", " + custom7 + ", " + custom8 + ", " + custom9 + ", " + custom10);
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

        public bool InsertItemsWithID(string itemid, string name, string categoryid, string barcode, string description, decimal costprice, decimal unitprice, decimal reorderlevel, decimal defaulttax, int receivingquantity, string picid, string status, string custom1, string custom2, string custom3, string custom4, string custom5, string custom6, string custom7, string custom8, string custom9, string custom10)
        {
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemsNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@categoryid", SqlDbType.UniqueIdentifier).Value = categoryid;
                    cmd.Parameters.Add("@barcode", SqlDbType.NVarChar).Value = barcode;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@costprice", SqlDbType.Decimal).Value = costprice;
                    cmd.Parameters.Add("@unitprice", SqlDbType.Decimal).Value = unitprice;
                    cmd.Parameters.Add("@reorderlevel", SqlDbType.Decimal).Value = reorderlevel;
                    cmd.Parameters.Add("@defaulttax", SqlDbType.Decimal).Value = defaulttax;
                    cmd.Parameters.Add("@receivingquantity", SqlDbType.Int).Value = receivingquantity;
                    cmd.Parameters.Add("@picid", SqlDbType.UniqueIdentifier).Value = picid;
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
                        logging.writeToImports("InsertItems | " + itemid + ", " + name + ", " + categoryid + ", " + barcode + ", " + description + ", " + costprice + ", " + unitprice + ", " + reorderlevel + ", " + defaulttax + ", " + receivingquantity + ", " + defaulttax + ", " + receivingquantity + ", " + picid + ", " + status + ", " + custom1 + ", " + custom2 + ", " + custom3 + ", " + custom4 + ", " + custom5 + ", " + custom6 + ", " + custom7 + ", " + custom8 + ", " + custom9 + ", " + custom10);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertItems.spItemNewWithID. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertItemTaxes(string itemid, string name, decimal percent)
        {
            string returnid = "";

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemTaxesNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@percent", SqlDbType.Decimal).Value = percent;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertItemTaxes | " + returnid + ", " + itemid + ", " + name + ", " + percent);
                    }
                    catch (Exception e) // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertItemTaxes. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertItemTaxesWithID(string itemtaxid, string itemid, string name, decimal percent)
        {
            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemTaxesNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = itemtaxid;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@percent", SqlDbType.Decimal).Value = percent;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertItemTaxesWithID | " + itemtaxid + ", " + itemid + ", " + name + ", " + percent);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertItemTaxes.spItemTaxesNew. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertTicketItems(string ticketid, string itemid, string description, string serialnumber, int line, decimal quantitypurchased, decimal itemcostprice, decimal itemunitprice, decimal discount, decimal discount2, string status)
        {
            string returnid = "";

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketItemsNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ticketid", SqlDbType.UniqueIdentifier).Value = ticketid;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@serialnumber", SqlDbType.NVarChar).Value = serialnumber;
                    cmd.Parameters.Add("@line", SqlDbType.Int).Value = line;
                    cmd.Parameters.Add("@quantitypurchased", SqlDbType.Decimal).Value = quantitypurchased;
                    cmd.Parameters.Add("@itemcostprice", SqlDbType.Decimal).Value = itemcostprice;
                    cmd.Parameters.Add("@itemunitprice", SqlDbType.Decimal).Value = itemunitprice;
                    cmd.Parameters.Add("@discount", SqlDbType.Decimal).Value = discount;
                    cmd.Parameters.Add("@discount2", SqlDbType.Decimal).Value = discount2;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertTicketItems | " + ticketid + ", " + itemid + ", " + description + ", " + serialnumber + ", " + line + ", " + quantitypurchased + ", " + itemcostprice + ", " + itemunitprice + ", " + discount + ", " + discount2 + ", " + status);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketItems. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertTicketItemsWithID(string ticketitemid, string ticketid, string itemid, string description, string serialnumber, int line, decimal quantitypurchased, decimal itemcostprice, decimal itemunitprice, decimal discount, decimal discount2, string status)
        {
            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketItemsNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = ticketitemid;
                    cmd.Parameters.Add("@ticketid", SqlDbType.UniqueIdentifier).Value = ticketid;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                    cmd.Parameters.Add("@serialnumber", SqlDbType.NVarChar).Value = serialnumber;
                    cmd.Parameters.Add("@line", SqlDbType.Int).Value = line;
                    cmd.Parameters.Add("@quantitypurchased", SqlDbType.Decimal).Value = quantitypurchased;
                    cmd.Parameters.Add("@itemcostprice", SqlDbType.Decimal).Value = itemcostprice;
                    cmd.Parameters.Add("@itemunitprice", SqlDbType.Decimal).Value = itemunitprice;
                    cmd.Parameters.Add("@discount", SqlDbType.Decimal).Value = discount;
                    cmd.Parameters.Add("@discount2", SqlDbType.Decimal).Value = discount2;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertTicketItemsWithID | " + ticketitemid + ", " + ticketid + ", " + itemid + ", " + description + ", " + serialnumber + ", " + line + ", " + quantitypurchased + ", " + itemcostprice + ", " + itemunitprice + ", " + discount + ", " + discount2 + ", " + status);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketItems. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertTicket(string starttime, string completetime, string customerid, string employeeid, string comment, string paymenttype, string invoicenumber)
        {
            string returnid = "";
            DateTime atime = dtconverter(starttime, 1);
            DateTime btime = dtconverter(completetime, 1);
            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@starttime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@completetime", SqlDbType.DateTime).Value = btime;
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    cmd.Parameters.Add("@invoicenumber", SqlDbType.NVarChar).Value = invoicenumber;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertTicket | " + returnid + "," + atime + ", " + btime + ", " + customerid + ", " + employeeid + ", " + comment + ", " + paymenttype + ", " + invoicenumber );
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicket. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertTicketWithID(string ticketid, string starttime, string completetime, string customerid, string employeeid, string comment, string paymenttype, string invoicenumber)
        {
            bool retvalue = false;
            DateTime atime = dtconverter(starttime, 1);
            DateTime btime = dtconverter(completetime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = ticketid;
                    cmd.Parameters.Add("@starttime", SqlDbType.DateTime).Value = atime;
                    cmd.Parameters.Add("@completetime", SqlDbType.DateTime).Value = btime;
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    cmd.Parameters.Add("@invoicenumber", SqlDbType.NVarChar).Value = invoicenumber;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertTicketWithID | " + ticketid + "," + atime + ", " + btime + ", " + customerid + ", " + employeeid + ", " + comment + ", " + paymenttype + ", " + invoicenumber);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketWithID. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertTicketPayments(string ticketid, string paymenttype, decimal paymentamount)
        {
            string returnid = "";

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketPaymentsNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ticketid", SqlDbType.UniqueIdentifier).Value = ticketid;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    cmd.Parameters.Add("@paymentamount", SqlDbType.Decimal).Value = paymentamount;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertTicketPayments | " + returnid + "," + ticketid + ", " + paymenttype + ", " + paymentamount);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketPayments. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertTicketPaymentsWithID(string ticketpaymentid, string ticketid, string paymenttype, decimal paymentamount)
        {
            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketPaymentsNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = ticketpaymentid;
                    cmd.Parameters.Add("@ticketid", SqlDbType.UniqueIdentifier).Value = ticketid;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    cmd.Parameters.Add("@paymentamount", SqlDbType.Decimal).Value = paymentamount;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertTicketPayments | " + ticketpaymentid + "," + ticketid + ", " + paymenttype + ", " + paymentamount);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertTicketPayments. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertVendors(string name, string vendornumber, string phone, string addressid, string personid, string comments)
        {

            string returnid = "";

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
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
                    cmd.Parameters.Add("@personid", SqlDbType.UniqueIdentifier).Value = personid;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertVendors | " + returnid +", " + name + ", " + vendornumber + ", " + phone + ", " + addressid + ", " + personid + ", " + comments);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertVendors. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertVendorsWithID(string vendorid, string name, string vendornumber, string phone, string addressid, string personid, string comments)
        {

            bool retvalue = false;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spVendorsNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = vendorid;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@vendornumber", SqlDbType.NVarChar).Value = vendornumber;
                    cmd.Parameters.Add("@phone", SqlDbType.NVarChar).Value = phone;
                    cmd.Parameters.Add("@addressid", SqlDbType.UniqueIdentifier).Value = addressid;
                    cmd.Parameters.Add("@personid", SqlDbType.UniqueIdentifier).Value = personid;
                    cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertVendorsWithID | " + vendorid + ", " + name + ", " + vendornumber + ", " + phone + ", " + addressid + ", " + personid + ", " + comments);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Unable to execute InsertVendorsWithID. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertTransfer(string itemid, string employeeid, string transdate, string comment, string departmentfromid, string departmenttoid, int completed)
        {
            
            string returnid = "";
            DateTime adate = dtconverter(transdate, 1);
            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTransferNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@transdate", SqlDbType.NVarChar).Value = adate;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@departmentfromid", SqlDbType.UniqueIdentifier).Value = departmentfromid;
                    cmd.Parameters.Add("@departmenttoid", SqlDbType.UniqueIdentifier).Value = departmenttoid;
                    cmd.Parameters.Add("@completed", SqlDbType.Int).Value = completed;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertTransfer | " + returnid + ", " + itemid + ", " + employeeid + ", " + adate + ", " + comment + ", " + departmentfromid + ", " + departmenttoid + ", " + completed);
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

        public bool InsertTransferWithID(string transferid, string itemid, string employeeid, string transdate, string comment, string departmentfromid, string departmenttoid, int completed)
        {

            bool retvalue = false;
            DateTime adate = dtconverter(transdate, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTransferNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = transferid;
                    cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;
                    cmd.Parameters.Add("@transdate", SqlDbType.NVarChar).Value = adate;
                    cmd.Parameters.Add("@comment", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@departmentfromid", SqlDbType.UniqueIdentifier).Value = departmentfromid;
                    cmd.Parameters.Add("@departmenttoid", SqlDbType.UniqueIdentifier).Value = departmenttoid;
                    cmd.Parameters.Add("@completed", SqlDbType.Int).Value = completed;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertTransferWithID | " + transferid + ", " + itemid + ", " + employeeid + ", " + adate + ", " + comment + ", " + departmentfromid + ", " + departmenttoid + ", " + completed);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: Unable to execute InsertTransferWithID. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public string InsertGiftCards(string giftcardnumber, string recordtime, decimal value, string status, string customerid, string employeeid)
        {
            string returnid = "";
            DateTime adate = dtconverter(recordtime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGiftcardNew";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@giftcardnumber", SqlDbType.NVarChar).Value = giftcardnumber;
                    cmd.Parameters.Add("@recordtime", SqlDbType.NVarChar).Value = adate;
                    cmd.Parameters.Add("@value", SqlDbType.Decimal).Value = value;
                    cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = value;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;

                    try
                    {
                        con.Open();
                        returnid = (string)cmd.ExecuteScalar();
                        logging.writeToImports("InsertGiftCards | " + returnid + ", " + giftcardnumber + ", " + adate + ", " + value + ", " + status + ", " + customerid + ", " + employeeid);
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute InsertGiftCards. " + e.Message);
                    }
                    con.Close();
                }
            }
            return returnid;
        }

        public bool InsertGiftCardsWithID(string giftcardid, string giftcardnumber, string recordtime, decimal value, string status, string customerid, string employeeid)
        {
            bool retvalue = false;
            DateTime adate = dtconverter(recordtime, 1);

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGiftcardNewWithID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = giftcardid;
                    cmd.Parameters.Add("@giftcardnumber", SqlDbType.NVarChar).Value = giftcardnumber;
                    cmd.Parameters.Add("@recordtime", SqlDbType.NVarChar).Value = adate;
                    cmd.Parameters.Add("@value", SqlDbType.Decimal).Value = value;
                    cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = value;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    cmd.Parameters.Add("@customerid", SqlDbType.UniqueIdentifier).Value = customerid;
                    cmd.Parameters.Add("@employeeid", SqlDbType.UniqueIdentifier).Value = employeeid;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                        logging.writeToImports("InsertGiftCardsWithID | " + giftcardid + ", " + giftcardnumber + ", " + adate + ", " + value + ", " + status + ", " + customerid + ", " + employeeid);
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

        /// <summary>
        /// Converts a string to a DateTime or produces a default value if conversion fails.
        /// </summary>
        /// <param name="dtime">The string to convert to a datetime value.</param>
        /// <param name="now">If 1 will return default datetime.now if failure of conversion.</param>
        /// <returns>The converted DateTime value or default.</returns>
        private DateTime dtconverter(string dtime,int now)
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

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace RsmgrImporter
{
    class SQLUpdateStatements
    {
        private string DBConLocal = ConfigurationManager.ConnectionStrings["RemoteConnection"].ConnectionString;
        private Logger logging = new Logger();

        public bool SQLUpdateAppConfig(string ukey, string value)
        {
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE app_config SET value = @value WHERE ukey = @ukey";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ukey", SqlDbType.NVarChar).Value = ukey;
                    cmd.Parameters.Add("@val", SqlDbType.NVarChar).Value = value;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return true;
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Failed to update app_config; " + e.Message);
                        return false;
                    }
                }
            }
        }

        public bool SQLPersonUpdate(int id, string firstname, string lastname, int addressid, string gender, string homephone, string cellphone, string workphone, string faxnumber, string birthday, string email, string comments, string ssn, string status)
        {
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    bool retvalue = false;
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
                    cmd.CommandText = "spPersonChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
                    cmd.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;
                    cmd.Parameters.Add("@addressid", SqlDbType.Int).Value = addressid;
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
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spPersonChange failed to execute : " + e.Message);
                        retvalue = false;
                    }

                    con.Close();
                    return retvalue;
                }
            }
        }

        public bool SQLItemQuantityUpdateQty(int itemid, int departmentid, int quantity, string status)
        {
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    bool retvalue = false;
                    // status = Active / InActive / SoldOut

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesChangeQty";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@departmentid", SqlDbType.Int).Value = departmentid;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spItemQuantitiesChangeQty failed to execute : " + e.Message);
                        retvalue = false;
                    }

                    con.Close();
                    return retvalue;
                }
            }
        }

        public bool SQLItemQuantityUpdateQtyByID(int itemquantityid, int quantity, string status)
        {
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    bool retvalue = false;
                    // status = Active / InActive / SoldOut

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesChangeQtyByID";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@itemquantityid", SqlDbType.Int).Value = itemquantityid;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: spItemQuantitiesChangeQtyByID failed to execute : " + e.Message);
                        retvalue = false;
                    }

                    con.Close();
                    return retvalue;
                }
            }
        }

        public bool SQLInventoryLogsUpdate(int inventorylogid, int itemid, int departmentid, int employeeid, string transactiontext)
        {
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    bool retvalue = false;

                    DateTime actiontime = DateTime.Now;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemQuantitiesChangeQty";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@inventory_log_id", SqlDbType.Int).Value = inventorylogid;
                    cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                    cmd.Parameters.Add("@action_time", SqlDbType.DateTime).Value = actiontime;
                    cmd.Parameters.Add("@departmentid", SqlDbType.Int).Value = departmentid;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = employeeid;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = transactiontext;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: SQLInventoryLogsUpdate failed to execute : " + e.Message);
                        retvalue = false;
                    }

                    con.Close();
                    return retvalue;
                }
            }
        }

        public bool UpdateTickets(int ticketid, string starttime, string completetime, int customerid, int employeeid, string comment, string paymenttype, string invoicenumber)
        {
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    bool retvalue = false;

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

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketchange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = ticketid;
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
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: SQLUpdateStatements.cs/UpdateTickets failed to execute : " + e.Message);
                        retvalue = false;
                    }

                    con.Close();
                    return retvalue;
                }
            }
        }

        public bool UpdateTicketPayments(int ticketid, string paymenttype, decimal paymentamount)
        {
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    bool retvalue = false;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketPaymentsChange";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ticketid", SqlDbType.Int).Value = ticketid;
                    cmd.Parameters.Add("@paymenttype", SqlDbType.NVarChar).Value = paymenttype;
                    cmd.Parameters.Add("@paymentamount", SqlDbType.Decimal).Value = paymentamount;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        retvalue = true;
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Error: SQLUpdateStatements.cs/UpdateTicketPayments failed to execute : " + e.Message);
                        retvalue = false;
                    }

                    con.Close();
                    return retvalue;
                }
            }
        }

    }
}

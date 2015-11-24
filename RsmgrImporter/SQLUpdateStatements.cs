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

        bool SQLUpdateAppConfig(string ukey, string value)
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

        bool SQLPersonUpdate(int id, string firstname, string lastname, int addressid, string gender, string homephone, string cellphone, string workphone, string faxnumber, string birthday, string email, string comments, string ssn, string status)
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
    }
}

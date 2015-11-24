using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace RsmgrImporter
{
    class SQLGetStatements
    {
        private string DBConLocal = ConfigurationManager.ConnectionStrings["RemoteConnection"].ConnectionString;
        private Logger logging = new Logger();

        public string GetAppConfig(string ukey)
        {
            string retvalue = "0";
            SqlConnection con = new SqlConnection(DBConLocal);
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAppConfigGet";
            cmd.Connection = con;
            
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.Add("@ukey", SqlDbType.NVarChar).Value = ukey;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                sda.Fill(dt);
                con.Close();
                if (dt.Rows.Count == 1)
                {
                    foreach (DataRow row in dt.Rows) { retvalue = row[1].ToString(); }
                }
                else { retvalue = "0"; }

                dt.Clear();
                logging.writeToLog("Info: Retrieved the value for ukey;  ukey = " + ukey + " and value = " + retvalue);
                return retvalue;
            }
            catch (Exception)  // find out how to add to dt and return value
            {
                dt.Clear(); con.Close(); return retvalue;
            }
        }

        bool IfExists(string cmdtxt, string param, string pvalue)
        {
            SqlConnection con = new SqlConnection(DBConLocal);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = cmdtxt;
            cmd.Connection = con;
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.Add(param, SqlDbType.NVarChar).Value = pvalue;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                sda.Fill(dt);
                con.Close();

                int retvalue = dt.Rows.Count;
                dt.Clear();
                if (retvalue > 0) { return true; } else { return false; }
                
            }
            catch (Exception e)  // find out how to add to dt and return value
            {
                logging.writeToLog("Error: Insert IfExists" + e.Message);
                return false;
            }            
        }

        public int GetVendorID(string vendornumber)
        {
            int returnval = 1;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT [vendor_id] FROM [vendors] WHERE [vendor_number] = @vendornumber";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@vendornumber", SqlDbType.NVarChar).Value = vendornumber;

                        using (DataTable dt = new DataTable())
                        {
                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                                sda.Fill(dt);
                            }
                            catch (Exception e)  // find out how to add to dt and return value
                            {
                                logging.writeToLog("Warning: Failed to GetVendorID : " + e.Message);
                                returnval = 1;
                            }
                            con.Close();

                            if (dt.Rows.Count == 1) {
                                try
                                {
                                    returnval = dt.Rows[0].Field<int>("vendor_id");
                                }
                                catch (Exception e) { logging.writeToLog("Error: Failure to convert Row[0] " + dt.Rows[0].Field<int>("vendor_id").ToString() + " to returnval GetVendorID. " + 
                                    e.Message); }
                            }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Warning: Account number " + vendornumber + " returned NO results."); }
                            else { logging.writeToLog("Warning: Account number" + vendornumber + " returned MULTIPLE results."); }
                        }
                    }
                }
                return returnval;
        }    }

        public int GetInvoiceIDforImports(int vendorid)
        {
            int returnval = 0;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT invoice_id FROM invoices WHERE vendor_id = @vendorid ORDER BY invoice_id";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@vendorid", SqlDbType.Int).Value = vendorid;

                        using (DataTable dt = new DataTable())
                        {
                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                                sda.Fill(dt);
                            }
                            catch (Exception e)  // find out how to add to dt and return value
                            {
                                logging.writeToLog("Warning: Failed to InvoiceID from GetInvoiceIDforImports : " + e.Message);
                                returnval = 0;
                            }

                            con.Close();

                            if (dt.Rows.Count == 1) { returnval = dt.Rows[0].Field<int>("invoice_id"); }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Info: Vendor_ID: " + vendorid.ToString() + " returned NO results in GetInvoiceIDforImports."); }
                            else
                            {
                                returnval = dt.Rows[0].Field<int>("invoice_id");
                                logging.writeToLog("Info: Vendor_ID: " + vendorid.ToString() + " returned MULTIPLE results in GetInvoiceIDforImports.");
                            }
                        }
                    }
                }
                return returnval;
            }
        }



    }
}

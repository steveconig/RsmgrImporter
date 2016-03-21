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
        private Conversions conv = new Conversions();

        public string GetAppConfig(string ukey)
        {
            string retvalue = "0";
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection con = new SqlConnection(DBConLocal))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spAppConfigGet";
                            cmd.Connection = con;
                            cmd.Parameters.Add("@ukey", SqlDbType.NVarChar).Value = ukey;

                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                                sda.Fill(dt);
                                con.Close();
                                if (dt.Rows.Count == 1)
                                {
                                    foreach (DataRow row in dt.Rows) { retvalue = row[0].ToString(); }
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
                    }
                }
            }
        }

        public bool IfExists(string cmdtxt)
        {
            DataTable dt = new DataTable();
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = cmdtxt;
                    cmd.Connection = con;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(dt);

                        int rows = dt.Rows.Count;
                        dt.Clear();
                        if (rows > 0) { retvalue = true; }
                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Error: Unable to execute IfExists. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public DataTable GetDataTableFromQuery(string cmdtxt)
        {
            DataTable retvalue = new DataTable("Main");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = cmdtxt;
                    cmd.Connection = con;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(retvalue);
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Unable to execute SQLGetStatements.cs/GetDataTableFromQuery. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
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

        public int GetDepartmentID(string departmentnumber)
        {
            int returnval = 1;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT department_id FROM departments WHERE department_number = @departmentnumber ORDER BY department_id";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@departmentnumber", SqlDbType.NVarChar).Value = departmentnumber;

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
                                logging.writeToLog("Warning: Failed to department_id from GetDepartmentID : " + e.Message);
                                returnval = 1;
                            }

                            con.Close();

                            if (dt.Rows.Count == 1) { returnval = dt.Rows[0].Field<int>("department_id"); }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Info: department_number: " + departmentnumber + " returned NO results in GetDepartmentID."); }
                            else
                            {
                                returnval = dt.Rows[0].Field<int>("department_id");
                                logging.writeToLog("Info: department_number: " + departmentnumber + " returned MULTIPLE results in GetDepartmentID.");
                            }
                        }
                    }
                }
                return returnval;
            }
        }

        public int GetCustomerIDByName(string firstname, string lastname)
        {
            int returnval = 1;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spCustomerGetByName";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
                        cmd.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;

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

                            if (dt.Rows.Count == 1)
                            {
                                try
                                {
                                    returnval = dt.Rows[0].Field<int>("customer_id");
                                }
                                catch (Exception e)
                                {
                                    logging.writeToLog("Error: Failure to convert Row[0] " + dt.Rows[0].Field<int>("customer_id").ToString() + " to returnval GetVendorID. " +
                  e.Message);
                                }
                            }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Warning: Account number " + firstname + " " + lastname + " returned NO results."); }
                            else
                            {
                                logging.writeToLog("Warning: Account number " + firstname + " " + lastname + " returned MULTIPLE results.");
                                returnval = dt.Rows[0].Field<int>("customer_id");
                            }
                        }
                    }
                }
                return returnval;
            }
        }

        public int GetEmployeeIDByName(string firstname, string lastname)
        {
            int returnval = 1;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spEmployeeGetByName";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
                        cmd.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;

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
                                logging.writeToLog("Warning: Failed to GetEmployeeIDByName : " + e.Message);
                                returnval = 1;
                            }
                            con.Close();

                            if (dt.Rows.Count == 1)
                            {
                                try
                                {
                                    returnval = dt.Rows[0].Field<int>("employee_id");
                                }
                                catch (Exception e)
                                {
                                    logging.writeToLog("Error: Failure to convert Row[0] " + dt.Rows[0].Field<int>("employee_id").ToString() + " to returnval GetEmployeeIDByName. " +
                  e.Message);
                                }
                            }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Warning: Name " + firstname + " " + lastname + " returned NO results."); }
                            else
                            {
                                logging.writeToLog("Warning: Name " + firstname + " " + lastname + " returned MULTIPLE results.");
                                returnval = dt.Rows[0].Field<int>("employee_id");
                            }
                        }
                    }
                }
                return returnval;
            }
        }

        /// <summary>
        /// i.name | i.barcode | i.description | q.quantity | q.status | d.department_name | d.department_number | i.cost_price | i.unit_price | i.reorder_level | i.default_tax | i.receiving_quantity | i.pic_id | i.status | i.custom1 | i.custom2 | i.custom3 | i.custom4 | i.custom5 | i.custom6 | i.custom7 | i.custom8 | i.custom9 | i.custom10 | q.item_quantity_id | i.item_id | d.department_id
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="departmentid"></param>
        /// <returns></returns>
        public DataTable GetItemQuantity(int itemid, int departmentid)
        {
            DataTable returnval = new DataTable();

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spItemQuantitiesGet";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@itemid", SqlDbType.Int).Value = itemid;
                        cmd.Parameters.Add("@departmentid", SqlDbType.Int).Value = departmentid;
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(returnval);
                        }
                        catch (Exception e)  // find out how to add to dt and return value
                        {
                            logging.writeToLog("Warning: Failed to GetItemIDByBarcode : " + e.Message);
                        }
                        con.Close();
                    }
                }
                return returnval;
            }
        }
        
        /// <summary>
        /// name | barcode | quantity | status | item_quantity_id | item_id | department_id
        /// </summary>
        /// <param name="departmentid">The department id returned from another SQL query.</param>
        /// <returns>DataTable</returns>
        public DataTable GetItemQuantityForDepartment(int departmentid)
        {
            using (DataTable returnval = new DataTable())
            {
                using (SqlConnection con = new SqlConnection(DBConLocal))
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spItemQuantitiesGetFromDept";
                            cmd.Connection = con;
                            cmd.Parameters.Add("@departmentid", SqlDbType.Int).Value = departmentid;
                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                                sda.Fill(returnval);
                            }
                            catch (Exception e)  // find out how to add to dt and return value
                            {
                                logging.writeToLog("Warning: Failed to GetItemIDByBarcode : " + e.Message);
                            }
                            con.Close();
                        }
                    }
                    return returnval;
                }
            }
        }

        public int GetItemIDByBarcode(string barcode)
        {
            int returnval = 1;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spItemsGetByBarcode";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@barcode", SqlDbType.NVarChar).Value = barcode;

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
                                logging.writeToLog("Warning: Failed to GetItemIDByBarcode : " + e.Message);
                                returnval = 1;
                            }
                            con.Close();

                            if (dt.Rows.Count == 1)
                            {
                                try
                                {
                                    returnval = dt.Rows[0].Field<int>("item_id");
                                }
                                catch (Exception e)
                                {
                                    logging.writeToLog("Error: Failure to convert Row[0] " + dt.Rows[0].Field<int>("item_id").ToString() + " to returnval GetItemIDByBarcode. " +
                  e.Message);
                                }
                            }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Warning: Barcode " + barcode + " returned NO results."); }

                        }
                    }
                }
                return returnval;
            }
        }

    }
}

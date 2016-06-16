using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace RsmgrImporter
{
    class SQLSelects
    {
        private string DBConLocal = ConfigurationManager.ConnectionStrings["RemoteConnection"].ConnectionString;
        private Logger logging = new Logger();


        /// <summary>
        /// Checks to see if a value exists.
        /// </summary>
        /// <param name="cmdtxt">The sql Select query that you want to check.</param>
        /// <returns>A bool result that determines if the query contains a value or not.</returns>
        public bool IfExists(string cmdtxt)
        {
            DataTable dt = new DataTable();
            bool retvalue = false;
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using(SqlCommand cmd = new SqlCommand())
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
                        logging.writeToLog("Unable to execute GetData. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        /// <summary>
        /// Id | Line1 | Line2 | Line3 | City | State | Zip | Country | Details
        /// </summary>
        /// <param name="id">Int</param>
        /// <returns>DataTable</returns>
        public DataTable GetAddress(string id)
        {
            DataTable retvalue = new DataTable();
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAddressGet";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(retvalue);
                            retvalue.Columns[0].ColumnName = "Id";
                            retvalue.Columns[1].ColumnName = "Line1";
                            retvalue.Columns[2].ColumnName = "Line2"; // remove
                            retvalue.Columns[3].ColumnName = "Line3"; // remove
                            retvalue.Columns[4].ColumnName = "City";
                            retvalue.Columns[5].ColumnName = "State";
                            retvalue.Columns[6].ColumnName = "Zip";
                            retvalue.Columns[7].ColumnName = "Country"; // remove
                            retvalue.Columns[8].ColumnName = "Details"; // remove
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Unable to execute GetAddress. " + e.Message);
                        }
                        con.Close();
                    }
                }
            }
            return retvalue;
        }

        /// <summary>
        /// Returns the ukey and value from App_Config table.
        /// </summary>
        /// <param name="ukey">The ukey to lookup the value.</param>
        /// <returns></returns>
        public string GetAppConfig(string ukey)
        {
            string retvalue = "0";
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAppConfigGet";
                    cmd.Parameters.Add("@ukey", SqlDbType.NVarChar).Value = ukey;
                    cmd.Connection = con;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(dt);

                        if (dt.Rows.Count == 1)
                        {
                            retvalue = dt.Rows[0][0].ToString();
                        }
                        else { retvalue = "0"; }

                        logging.writeToLog("Info: Retrieved the value for ukey;  ukey = " + ukey + " and value = " + retvalue);

                    }
                    catch (Exception e)  // find out how to add to dt and return value
                    {
                        logging.writeToLog("Unable to execute GetAppConfig. " + e.Message);
                    }
                    con.Close();
                    dt.Clear();
                }
            }
            return retvalue;
        }
        
        public DataTable GetAppConfigAll()
        {
            DataTable retvalue = new DataTable("AppConfigs");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAppConfigGetAll";
                    cmd.Connection = con;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(retvalue);
                        retvalue.Columns[0].ColumnName = "Key";
                        retvalue.Columns[1].ColumnName = "Value";
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Unable to execute GetDepartment. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public DataTable GetCategory(string id)
        {
            DataTable retvalue = new DataTable();
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCategoryGet";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(retvalue);
                            retvalue.Columns[0].ColumnName = "ID";
                            retvalue.Columns[1].ColumnName = "Name";
                            retvalue.Columns[2].ColumnName = "Description";
                            retvalue.Columns[3].ColumnName = "In Use";
                            retvalue.Columns[4].ColumnName = "Parent ID";
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Unable to execute GetCategory. " + e.Message);
                        }
                        con.Close();
                    }
                }
            }
            return retvalue;
        }

        // public DataTable GetCategoryAll()

        public DataTable GetCustomerAll()
        {
            DataTable retvalue = new DataTable("Customers");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spCustomerGetAll";
                    cmd.Connection = con;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(retvalue);
                        retvalue.Columns[0].ColumnName = "First Name";
                        retvalue.Columns[1].ColumnName = "Last Name";
                        retvalue.Columns[2].ColumnName = "Account Number";
                        retvalue.Columns[3].ColumnName = "Join Date";
                        retvalue.Columns[4].ColumnName = "can Charge?";
                        retvalue.Columns[5].ColumnName = "Charge Limit";
                        retvalue.Columns[6].ColumnName = "Total Visits";
                        retvalue.Columns[7].ColumnName = "Default Store";
                        retvalue.Columns[8].ColumnName = "Balance";
                        retvalue.Columns[9].ColumnName = "Taxable";
                        retvalue.Columns[10].ColumnName = "Gender";
                        retvalue.Columns[11].ColumnName = "Home Phone";
                        retvalue.Columns[12].ColumnName = "Cell Phone";
                        retvalue.Columns[13].ColumnName = "Work Phone";
                        retvalue.Columns[14].ColumnName = "Fax Number";
                        retvalue.Columns[15].ColumnName = "Birthday";
                        retvalue.Columns[16].ColumnName = "Email";
                        retvalue.Columns[17].ColumnName = "Comments";
                        retvalue.Columns[18].ColumnName = "Status";
                        retvalue.Columns[19].ColumnName = "Line 1";
                        retvalue.Columns[20].ColumnName = "Line 2";
                        retvalue.Columns[21].ColumnName = "Line 3";
                        retvalue.Columns[22].ColumnName = "City";
                        retvalue.Columns[23].ColumnName = "State";
                        retvalue.Columns[24].ColumnName = "Zip";
                        retvalue.Columns[25].ColumnName = "Country";
                        retvalue.Columns[26].ColumnName = "Details";
                        retvalue.Columns[27].ColumnName = "PersonID";
                        retvalue.Columns[28].ColumnName = "CustomerID";
                        retvalue.Columns[29].ColumnName = "AddressID";
                        retvalue.Columns.Remove("Line 2");
                        retvalue.Columns.Remove("Line 3");
                        retvalue.Columns.Remove("Country");
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Unable to execute GetCustomerAll. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public DataTable GetCustomerByID(string id)
        {
            DataTable retvalue = new DataTable();
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spCustomerGet";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(retvalue);
                            retvalue.Columns[0].ColumnName = "First Name";
                            retvalue.Columns[1].ColumnName = "Last Name";
                            retvalue.Columns[2].ColumnName = "Account Number";
                            retvalue.Columns[3].ColumnName = "Join Date";
                            retvalue.Columns[4].ColumnName = "Can Charge?";
                            retvalue.Columns[5].ColumnName = "Charge Limit";
                            retvalue.Columns[6].ColumnName = "Total Visits";
                            retvalue.Columns[7].ColumnName = "Default Store";
                            retvalue.Columns[8].ColumnName = "Balance";
                            retvalue.Columns[9].ColumnName = "Taxable";
                            retvalue.Columns[10].ColumnName = "Gender";
                            retvalue.Columns[11].ColumnName = "Home Phone";
                            retvalue.Columns[12].ColumnName = "Cell Phone";
                            retvalue.Columns[13].ColumnName = "Work Phone";
                            retvalue.Columns[14].ColumnName = "Fax Number";
                            retvalue.Columns[15].ColumnName = "Birthday";
                            retvalue.Columns[16].ColumnName = "Email";
                            retvalue.Columns[17].ColumnName = "Comments";
                            retvalue.Columns[18].ColumnName = "Status";
                            retvalue.Columns[19].ColumnName = "Line 1";
                            retvalue.Columns[20].ColumnName = "Line 2";
                            retvalue.Columns[21].ColumnName = "Line 3";
                            retvalue.Columns[22].ColumnName = "City";
                            retvalue.Columns[23].ColumnName = "State";
                            retvalue.Columns[24].ColumnName = "Zip";
                            retvalue.Columns[25].ColumnName = "Country";
                            retvalue.Columns[26].ColumnName = "Details";
                            retvalue.Columns[27].ColumnName = "PersonID";
                            retvalue.Columns[28].ColumnName = "CustomerID";
                            retvalue.Columns[29].ColumnName = "AddressID";
                            retvalue.Columns.Remove("Line 2");
                            retvalue.Columns.Remove("Line 3");
                            retvalue.Columns.Remove("Country");
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Unable to execute GetCustomerByID. " + e.Message);
                        }
                        con.Close();
                    }
                }
            }
            return retvalue;
        }

        public string GetInvoiceIDforImports(string vendorid)
        {
            string returnval = "";

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT invoice_id FROM invoices WHERE vendor_id = @vendorid ORDER BY invoice_id";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@vendorid", SqlDbType.UniqueIdentifier).Value = vendorid;

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
                            }
                            con.Close();

                            if (dt.Rows.Count == 1) { returnval = dt.Rows[0].Field<string>("invoice_id"); }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Info: Vendor_ID: " + vendorid + " returned NO results in GetInvoiceIDforImports."); }
                            else
                            {
                                returnval = dt.Rows[0].Field<string>("invoice_id");
                                logging.writeToLog("Info: Vendor_ID: " + vendorid.ToString() + " returned MULTIPLE results in GetInvoiceIDforImports.");
                            }
                        }
                    }
                }
                return returnval;
            }
        }

        public DataTable GetDepartmentAll()
        {
            DataTable retvalue = new DataTable("Departments");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDepartmentGetAll";
                    cmd.Connection = con;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(retvalue);
                        retvalue.Columns[0].ColumnName = "ID";
                        retvalue.Columns[1].ColumnName = "Name";
                        retvalue.Columns[2].ColumnName = "StoreNumber";
                        retvalue.Columns[3].ColumnName = "Comments";
                        retvalue.Columns[4].ColumnName = "Status";
                        retvalue.Columns[5].ColumnName = "AddressId";
                        retvalue.Columns[6].ColumnName = "Line1";
                        retvalue.Columns[7].ColumnName = "Line2"; // remove
                        retvalue.Columns[8].ColumnName = "Line3"; // remove
                        retvalue.Columns[9].ColumnName = "City";
                        retvalue.Columns[10].ColumnName = "State";
                        retvalue.Columns[11].ColumnName = "Zip";
                        retvalue.Columns[12].ColumnName = "Country"; // remove
                        retvalue.Columns[13].ColumnName = "Details"; // remove
                        retvalue.Columns.Remove("Line2");
                        retvalue.Columns.Remove("Line3");
                        retvalue.Columns.Remove("Country");
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Unable to execute GetDepartment. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public DataTable GetDepartmentByDeptNum(string departmentnumber)
        {
            DataTable retvalue = new DataTable("Departments");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spDepartmentGetByDeptNum";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@departmentnumber", SqlDbType.NVarChar).Value = departmentnumber;

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(retvalue);
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Unable to execute GetDepartmentByDeptNum. " + e.Message);
                        }
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        // public DataTable GetEmployee(string id)

        // public DataTable GetEmployeesAll()

        public DataTable Authentication(string username, string password)
        {
            // employee_id, username, title, person_id, first_name, last_name
            DataTable dt = new DataTable();            
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spLogin";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@u", SqlDbType.NVarChar).Value = username;
                        cmd.Parameters.Add("@p", SqlDbType.NVarChar).Value = password;
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(dt);
                        }
                        catch (Exception e)
                        { // find out how to add to dt and return value
                            logging.writeToLog("Unable to execute SQLSelects.cs/Authentication. " + e.Message);
                        }
                    }
                }
                con.Close();
            }
            return dt;
        }

        // public DataTable GetGiftcard(string id)

        // public DataTable GetGiftCardsAll()

        // public DataTable GetInventoryLog(string id)

        // public DataTable GetInventoryLogsAll()

        // public DataTable GetInvoice(string id)

        // public DataTable GetInvoicesAll()

        // public DataTable GetInvoiceItem(string id)

        // public DataTable GetInvoiceItemsAll()

        // public DataTable GetItem(string id)

        public DataTable GetItemsAll()
        {
            DataTable retvalue = new DataTable("Items");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemsGetAll";
                    cmd.Connection = con;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(retvalue);
                        retvalue.Columns[0].ColumnName = "Name";
                        retvalue.Columns[1].ColumnName = "Barcode";
                        retvalue.Columns[2].ColumnName = "Description";
                        retvalue.Columns[3].ColumnName = "Category";
                        retvalue.Columns[4].ColumnName = "Cost";
                        retvalue.Columns[5].ColumnName = "Price";
                        retvalue.Columns[6].ColumnName = "Reorder Level";
                        retvalue.Columns[7].ColumnName = "Default Tax";
                        retvalue.Columns[8].ColumnName = "Default Quantity";
                        retvalue.Columns[9].ColumnName = "PicID";
                        retvalue.Columns[10].ColumnName = "Status";
                        retvalue.Columns[11].ColumnName = "Style";
                        retvalue.Columns[12].ColumnName = "Color";
                        retvalue.Columns[13].ColumnName = "Season";
                        retvalue.Columns[14].ColumnName = "Sizes";
                        retvalue.Columns[15].ColumnName = "iSizes";
                        retvalue.Columns[16].ColumnName = "Unused1"; // unuse
                        retvalue.Columns[17].ColumnName = "Unused2"; // unuse
                        retvalue.Columns[18].ColumnName = "Division";
                        retvalue.Columns[19].ColumnName = "Department";
                        retvalue.Columns[20].ColumnName = "Class";
                        retvalue.Columns[21].ColumnName = "Item ID";
                        retvalue.Columns[22].ColumnName = "Category ID";
                        retvalue.Columns.Remove("Unused1");
                        retvalue.Columns.Remove("Unused2");
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Unable to execute GetDepartment. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public DataTable GetItemsAllByBarcode(string barcode)
        {
            DataTable retvalue = new DataTable("Items");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spItemsGetByBarcode";
                    cmd.Connection = con;
                    cmd.Parameters.Add("@barcode", SqlDbType.NVarChar).Value = barcode;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        sda.Fill(retvalue);
                        retvalue.Columns[0].ColumnName = "Name";
                        retvalue.Columns[1].ColumnName = "Barcode";
                        retvalue.Columns[2].ColumnName = "Description";
                        retvalue.Columns[3].ColumnName = "Category";
                        retvalue.Columns[4].ColumnName = "Cost";
                        retvalue.Columns[5].ColumnName = "Price";
                        retvalue.Columns[6].ColumnName = "Reorder Level";
                        retvalue.Columns[7].ColumnName = "Default Tax";
                        retvalue.Columns[8].ColumnName = "Default Quantity";
                        retvalue.Columns[9].ColumnName = "PicID";
                        retvalue.Columns[10].ColumnName = "Status";
                        retvalue.Columns[11].ColumnName = "Style";
                        retvalue.Columns[12].ColumnName = "Color";
                        retvalue.Columns[13].ColumnName = "Season";
                        retvalue.Columns[14].ColumnName = "Sizes";
                        retvalue.Columns[15].ColumnName = "iSizes";
                        retvalue.Columns[16].ColumnName = "Unused1"; // unuse
                        retvalue.Columns[17].ColumnName = "Unused2"; // unuse
                        retvalue.Columns[18].ColumnName = "Division";
                        retvalue.Columns[19].ColumnName = "Department";
                        retvalue.Columns[20].ColumnName = "Class";
                        retvalue.Columns[21].ColumnName = "Item ID";
                        retvalue.Columns[22].ColumnName = "Category ID";
                        retvalue.Columns.Remove("Unused1");
                        retvalue.Columns.Remove("Unused2");
                    }
                    catch (Exception e)
                    {
                        logging.writeToLog("Unable to execute GetDepartment. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public DataTable GetItemQty(string id)
        {
            DataTable retvalue = new DataTable("Inventory");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spItemQuantitiesGetFromItem";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(retvalue);
                            retvalue.Columns[0].ColumnName = "Name";
                            retvalue.Columns[1].ColumnName = "Barcode";
                            retvalue.Columns[2].ColumnName = "Description";
                            retvalue.Columns[3].ColumnName = "Quantity";
                            retvalue.Columns[4].ColumnName = "Status";
                            retvalue.Columns[5].ColumnName = "Department Name";
                            retvalue.Columns[6].ColumnName = "Department Number";
                            retvalue.Columns[7].ColumnName = "Cost Price";
                            retvalue.Columns[8].ColumnName = "Unit Price";
                            retvalue.Columns[9].ColumnName = "Reorder Level";
                            retvalue.Columns[10].ColumnName = "Default Tax";
                            retvalue.Columns[11].ColumnName = "Receiving Quantity";
                            retvalue.Columns[12].ColumnName = "PicID";
                            retvalue.Columns[13].ColumnName = "Item Status";
                            retvalue.Columns[14].ColumnName = "Style";
                            retvalue.Columns[15].ColumnName = "Color";
                            retvalue.Columns[16].ColumnName = "Season";
                            retvalue.Columns[17].ColumnName = "Sizes";
                            retvalue.Columns[18].ColumnName = "iSizes";
                            retvalue.Columns[19].ColumnName = "Unused1"; // unuse
                            retvalue.Columns[20].ColumnName = "Unused2"; // unuse
                            retvalue.Columns[21].ColumnName = "Division";
                            retvalue.Columns[22].ColumnName = "Department";
                            retvalue.Columns[23].ColumnName = "Class";
                            retvalue.Columns[24].ColumnName = "Item Qty ID";
                            retvalue.Columns[25].ColumnName = "Item ID";
                            retvalue.Columns[26].ColumnName = "Department ID";
                            retvalue.Columns.Remove("Unused1");
                            retvalue.Columns.Remove("Unused2");
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Unable to execute GetItemQty. " + e.Message);
                        }
                        con.Close();
                    }
                }
            }
            return retvalue;
        }

        public DataTable GetItemQtyByItemDepartment(string itemid, string departmentid)
        {
            DataTable retvalue = new DataTable("Inventory");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spItemQuantitiesGet";
                        cmd.Connection = con;
                        cmd.Parameters.Add("@itemid", SqlDbType.UniqueIdentifier).Value = itemid;
                        cmd.Parameters.Add("@departmentid", SqlDbType.UniqueIdentifier).Value = departmentid;
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            sda.Fill(retvalue);
                            retvalue.Columns[0].ColumnName = "Name";
                            retvalue.Columns[1].ColumnName = "Barcode";
                            retvalue.Columns[2].ColumnName = "Description";
                            retvalue.Columns[3].ColumnName = "Quantity";
                            retvalue.Columns[4].ColumnName = "Status";
                            retvalue.Columns[5].ColumnName = "Department Name";
                            retvalue.Columns[6].ColumnName = "Department Number";
                            retvalue.Columns[7].ColumnName = "Cost Price";
                            retvalue.Columns[8].ColumnName = "Unit Price";
                            retvalue.Columns[9].ColumnName = "Reorder Level";
                            retvalue.Columns[10].ColumnName = "Default Tax";
                            retvalue.Columns[11].ColumnName = "Receiving Quantity";
                            retvalue.Columns[12].ColumnName = "PicID";
                            retvalue.Columns[13].ColumnName = "Item Status";
                            retvalue.Columns[14].ColumnName = "Style";
                            retvalue.Columns[15].ColumnName = "Color";
                            retvalue.Columns[16].ColumnName = "Season";
                            retvalue.Columns[17].ColumnName = "Sizes";
                            retvalue.Columns[18].ColumnName = "iSizes";
                            retvalue.Columns[19].ColumnName = "Unused1"; // unuse
                            retvalue.Columns[20].ColumnName = "Unused2"; // unuse
                            retvalue.Columns[21].ColumnName = "Division";
                            retvalue.Columns[22].ColumnName = "Department";
                            retvalue.Columns[23].ColumnName = "Class";
                            retvalue.Columns[24].ColumnName = "Item Qty ID";
                            retvalue.Columns[25].ColumnName = "Item ID";
                            retvalue.Columns[26].ColumnName = "Department ID";
                            retvalue.Columns.Remove("Unused1");
                            retvalue.Columns.Remove("Unused2");
                        }
                        catch (Exception e)
                        {
                            logging.writeToLog("Unable to execute GetItemQty. " + e.Message);
                        }
                        con.Close();
                    }
                }
            }
            return retvalue;
        }

        // public DataTable GetLayaway(string id)

        // public DataTable GetLayawaysAll()

        // public DataTable GetModule(string id)

        // public DataTable GetModulesAll()

        // public DataTable GetPermission(string id)

        // public DataTable GetPermissionsAll()

        // public DataTable GetPerson(string id)

        // public DataTable GetPersonsAll()

        // public DataTable GetTicket(string id)

        public DataTable GetTicketsAll()
        {
            DataTable retvalue = new DataTable("Tickets");
            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTicketGetAll";
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
                        logging.writeToLog("Unable to execute GetTicketAll. " + e.Message);
                    }
                    con.Close();
                }
            }
            return retvalue;
        }

        public int GetTicketCountToday()
        {
            int returnval = 0;

            using (SqlConnection con = new SqlConnection(DBConLocal))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spTicketGetAllToday";
                        cmd.Connection = con;

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
                                logging.writeToLog("Error: Failed to Execute GetTicketCountToday : " + e.Message);
                            }
                            returnval = dt.Rows.Count;
                        }
                    }
                    con.Close();
                }
            }
            return returnval;
        }

        // public DataTable GeTicketItem(string id)

        // public DataTable GetTicketItemsAll()

        // public DataTable GetTicketItemsByTicket(string ticketid)

        public string GetVendorID(string vendornumber)
        {
            string returnval = "";

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
                            }
                            con.Close();

                            if (dt.Rows.Count == 1)
                            {
                                try
                                {
                                    returnval = dt.Rows[0].Field<string>("vendor_id");
                                }
                                catch (Exception e)
                                {
                                    logging.writeToLog("Error: Failure to convert Row[0] " + dt.Rows[0].Field<string>("vendor_id") + " to returnval GetVendorID. " + e.Message);
                                }
                            }
                            else if (dt.Rows.Count == 0) { logging.writeToLog("Warning: Account number " + vendornumber + " returned NO results."); }
                            else { logging.writeToLog("Warning: Account number" + vendornumber + " returned MULTIPLE results."); }
                        }
                    }
                }
                return returnval;
            }
        }
    }
}

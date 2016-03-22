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
                foreach (DataRow row in TicketTable.Rows)
                {
                    // TODO: Execute imports into SQL.
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

                foreach (DataRow row in TicketItemTable.Rows)
                {
                    // TODO: Execute imports into SQL.
                }
            }
        }
    }
}

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
        public DataTable TicketTable = new DataTable();

        public void SetTicketTable(DataTable dt)
        {
            TicketTable = dt;

        }



    }
}

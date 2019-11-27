using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.OleDb;

namespace Game
{
    class database
    {
        public static database dbcon = new database();
        OleDbConnection conn = null;
        public database()
        {
            createConnection();
        }
        private void createConnection()
        {
            string connection = "Provider = OraOleDb.Oracle" +
                ".1 ;Data Source = ORCL;user id= systemuser; " +
                "password = williamwood01";
            conn = new OleDbConnection(connection);
            conn.Open();
        }
        public OleDbConnection getConnection()
        { return conn; }
    }
}

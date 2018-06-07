using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCDatabase 
{
    public class EcoDBConnection : AmcDataConnection
    {
        const string CONNECTIONSTRING = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=prodmansql;Password=pr0dman5ux;Initial Catalog=ECO_Tracker;Data Source=BUSHMASTER";
        const string CONNECTIONTYPE = "SQL";

        public EcoDBConnection()
        {
            _nDatabaseConnectionType = (int)dbType.sqlEcoTracker;
        }

        public bool connect(string sTableName)
        {
            return ConnectToDatabase(CONNECTIONSTRING);
        }
    }
}

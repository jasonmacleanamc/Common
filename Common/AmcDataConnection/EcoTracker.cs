using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMCDatabase;

namespace AMCDatabase
{
    public class EcoTracker : DBWorks
    {
        public string CONNECTION_STRING = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=prodmansql;Password=pr0dman5ux;Initial Catalog=ECO_Tracker;Data Source=BUSHMASTER";
        public int nDATABASE_TYPE = (int)dbType.sqlEcoTracker;

        public EcoTracker()
        {
            ConnectionString = CONNECTION_STRING;
            DataBaseType = nDATABASE_TYPE;
        }

        override public void UpdateTable()
        {

        }
    }
}

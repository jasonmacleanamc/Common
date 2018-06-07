using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCDatabase
{
    public class Amplifs : PMTables
    {
        public Amplifs(string sKeyValue) : base("Amplifs.dbf", sKeyValue, "Baseplate")
        {             
        }
        
        public override void AddColums()
        {
            // TODO: ja - add all columns
            
            _ColumnNames.Add(KeyIdentifier);
        }        
    }
}

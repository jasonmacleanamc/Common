using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCDatabase
{
    public class Workcode : PMTables
    {
        public string MODELLCODE = "Modellcode";
        public string AMPVERSION = "Ampver";
        
        public Workcode(string sKeyValue, bool bIsTest = false) //: base("Workcode.dbf", sKeyValue, "Workcode", bIsTest)
        {
            KeyValue = sKeyValue;
            KeyIdentifier = "Workcode";
            TableName = "Workcode";
            ConnectionString = GetPMConnString();
        }
        
        public override void AddColums()
        {
            // TODO: ja - add all columns
            
            _ColumnNames.Add(KeyIdentifier);
            _ColumnNames.Add(MODELLCODE);
            _ColumnNames.Add("Quantity");
            _ColumnNames.Add(AMPVERSION);
            _ColumnNames.Add("Ems_wover");
        }        
    }
}

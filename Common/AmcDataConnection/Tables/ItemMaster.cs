using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCDatabase
{
    public class ItemMaster : PMTables
    {
        const string KEY_IDENTIFIER = "Partno";
        const string TABLE_NAME = "Itemmast";

        public ItemMaster(string sKeyValue, bool bIsTest = false) //: base(, sKeyValue, "Partno", bIsTest)
        {
            KeyValue = sKeyValue;
            KeyIdentifier = KEY_IDENTIFIER;
            TableName = TABLE_NAME;
            ConnectionString = GetPMConnString();
        }

        public override void AddColums()
        {
            _ColumnNames.Add(KeyIdentifier);
            _ColumnNames.Add("Oper_code");
            _ColumnNames.Add("Description");
            _ColumnNames.Add("Type_code");
            _ColumnNames.Add("Lead_code");
            _ColumnNames.Add("Vend_desc");
            _ColumnNames.Add("Vend_name");
            _ColumnNames.Add("Qty_on_hand");
            _ColumnNames.Add("Cost_avg");
            _ColumnNames.Add("Status");
            _ColumnNames.Add("Con_code");
            _ColumnNames.Add("Version");
            _ColumnNames.Add("Cbpb_rec");
            _ColumnNames.Add("Engattrib");
        }       
    }
}

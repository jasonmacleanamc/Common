﻿/* ITable Class
 * 
 * This class must inherit from the Database Type (derived from DBWorks) class and the ITable Interface
 * 
 * The Constructor must assign the KeyValue Property and call InitTable() 
 * 
 * InitTable() must assign the KeyIdentifier and TableName Property and call InitializeDatabase()
 * 
 * AddColums() must populate the global _ColumnNames field
 * 
 */

namespace AMCDatabase
{
    public class ItemMaster : PMTables, ITable
    {
        const string KEY_IDENTIFIER = "Partno";
        const string TABLE_NAME = "Itemmast";

        public ItemMaster(string sKeyValue, bool bTest = false)
        {
            KeyValue = sKeyValue;
            TestData = bTest;

            InitTable();
        }

        public bool InitTable()
        {
            KeyIdentifier = KEY_IDENTIFIER;
            TableName = TABLE_NAME;

            // ja - for vfp the connection string has to be set after the table name is known
            ConnectionString = GetPMConnString();

            return InitializeDatabase();
        }
#if no
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
#endif
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TestResultsLib.SQLBuilder
{
    public partial class SQLBuilder
    {
        public List<string> ArgumentList { get; private set; }
        public SqlConnection TheConnection { get; private set; }
        protected SqlDataReader TheReader { get; private set; }
        protected SqlCommand TheCommand { get; private set; }


        protected string TheTableName { get; private set; }
        public string TheKey { get; private set; }
        public string TheKeyValue { get; private set; }

        public SQLBuilder(string sTableName)
        {
            TheTableName = sTableName;
        }

        public SQLBuilder(string sTableName, string sKey, string sKeyValue)
        {
            TheTableName = sTableName;
            TheKey = sKey;
            TheKeyValue = sKeyValue;
        }

        public void AddArgument(string sArgument)
        {
            ArgumentList.Add(sArgument);            
        }

        private bool OpenConnection()
        {
            TheConnection = new SqlConnection(ConnectonStrings.GetMfgDataConnectionString());

            try
            {
                TheConnection.Open();
            }
            catch (Exception)
            {
                throw;
            }

            return true;

        }

       

        
    }

    
}

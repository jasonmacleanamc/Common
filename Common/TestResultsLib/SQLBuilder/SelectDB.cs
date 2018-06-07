using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TestResultsLib.SQLBuilder
{
    partial class SQLBuilder
    {
        private string CreateSelectString()
        {
            string sSelect = "Select";
            string sArgs = string.Join(",", ArgumentList.ToArray());
            string sFrom = "From " + TheTableName;
            string sWhere = "Where " + TheKey + " = '" + TheKeyValue + "'";

            string sSql = string.Format("{0} {1} {2} {3}", sSelect, sArgs, sFrom, sWhere);

            return sSql;
        }

        public bool ReadData()
        {
            bool bRet = true;

            try
            {
                if (!OpenConnection())
                    throw new System.ArgumentException("Cannot open Database", "MfgData");               

                TheReader = null;
                TheCommand = new SqlCommand(CreateSelectString(), TheConnection);

                TheReader = TheCommand.ExecuteReader();

                if (TheReader.HasRows)
                {
                    while (TheReader.Read())
                    {
                        //TheReader.FieldCount
                        var Result = TheReader[0];

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return bRet;
        }

       

        private static IEnumerable<T> GetSqlData<T>(string connectionstring, string sql) where T : new()
        {
            var properties = typeof(T).GetProperties();

            using (var conn = new SqlConnection(connectionstring))
            {
                using (var comm = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var element = new T();

                            foreach (var f in properties)
                            {
                                var o = reader[f.Name];
                                if (o.GetType() != typeof(DBNull)) f.SetValue(element, o, null);
                            }
                            yield return element;
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}

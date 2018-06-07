using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Globalization;

namespace AMCDatabase
{
    public class UpdateTables : AMCDatabase
    {
        private string UpdateString { get; set; }
        private string AndWhere { get; set; }

        private AmcDataConnection _dbConn;

        private Dictionary<string, string> SetPairs { get; set; }

        public UpdateTables(AmcDataConnection dbConn, Dictionary<string, string> setParams)
        {
            _dbConn = dbConn;
            SetPairs = setParams;
        }

        private void GenerateUpdateSQLString()
        {
            string sUpdate = "Update " + _dbConn.TableName;
            string sSet = " set " + GetSetString();
            string sWhere = " where " + _dbConn.KeyIdentifier + " = " + FormatQuote(_dbConn.KeyValue);

            if (!string.IsNullOrEmpty(AndWhere))
                sWhere += " " + AndWhere;

            // TODO: ja - (UpdateTables) Add Version!!!!

            UpdateString = sUpdate + sSet + sWhere;
        }

        private string GetSetString()
        {
            string sSetString = "";
            int nTotal = SetPairs.Count;
            int nCounter = 0;

            foreach (var setPair in SetPairs)
            {
                if (!(setPair.Key.ToLower() == _dbConn.KeyIdentifier.ToLower()))
                {
                    string sValue = setPair.Value;// = __FormatQuote(setPair.Value);

                    // ja - special case for VFP
                    if (_dbConn.DataBaseType == (int)dbType.vfp)
                    {
                        // ja - this will test for boolean, date time and int and format accordingly for VFP
                        if (!FormatForBool(ref sValue)) // ja - boolean
                            if (!FormatForDateTime(ref sValue)) // ja - date time
                                if (!FormatForInt(ref sValue)) // ja - int
                                    sValue = FormatQuote(setPair.Value); // ja - string
                    }

                    sSetString += setPair.Key + " = " + sValue.Trim();

                    if (++nCounter != nTotal)
                        sSetString += ", ";
                }
                else
                    ++nCounter;
            }
            
            return sSetString;
        }

        private bool FormatForInt(ref string sValue)
        {
            bool bRet = false;
            
            string sTestValue = sValue.TrimStart('"');
            if (sTestValue.StartsWith(INT))
            {
                sValue = sValue.TrimStart('"');
                sValue = sValue.TrimEnd('"');
                sValue = sValue.Substring(4);

                bRet = true;
            }

            return bRet;
        }

        private bool FormatForBool(ref string sFormatValue)
        {
            bool bRet = true;

            if (sFormatValue == "True")
                sFormatValue = ".T.";
            else if (sFormatValue == "False")
                sFormatValue = ".F.";
            else
                bRet = false;

            return bRet;
        }

        private bool FormatForDateTime(ref string sValue)
        {
            bool bRet = false;

	        DateTime dt;
            if (DateTime.TryParse(sValue, out dt)) 
            {
                string sHour = dt.Hour.ToString();
                string sAMPM = "AM";
                if (dt.Hour > 12)
                {
                    sAMPM = "PM";
                    int nHour = dt.Hour - 12;
                    sHour = nHour.ToString();
                }
                                    
                sValue = "{^" + dt.Year + "/" + dt.Month + "/" + dt.Day + " " + sHour + ":" + dt.Minute + ":" + dt.Second + sAMPM + "}";

                bRet = true;
            }	
            
            return bRet;
}

        public bool PerformUpdate()
        {
            Console.WriteLine("PerformUpdate");
            bool bRet = true;

            GenerateUpdateSQLString();

            OleDbCommand SQlUpdate = new OleDbCommand(UpdateString, _dbConn.GetOleConn());

            // ja - don't catch the exception here, let the UI do it
            try
            {
                if (SQlUpdate.ExecuteNonQuery() == -1)
                {
                    bRet = false;
                    throw new System.ArgumentException("Update Failed", "AMCDatabase");
                }
            }
            catch (System.Exception ex)
            {
                 Console.WriteLine(ex.Message);
                 throw new System.ArgumentException(ex.Message, "AMCDatabase");
            }
            
            return bRet;
        }

        public void AddAndWhere(string sAndWhere)
        {
            AndWhere = sAndWhere;
        }
               

        public bool UpdateBlob()
        {
            Console.WriteLine("Update Blob");
            OleDbDataAdapter dataAdapter = _dbConn.GetAdapter();
            OleDbCommand command;
            
            string sField = "";
            string sValue = "";
            foreach (var setPair in SetPairs)
            {
                sField = setPair.Key;
                sValue = setPair.Value;

                break;
            }

            string sWhere = " where " + _dbConn.KeyIdentifier + " = " + FormatQuote(_dbConn.KeyValue);

            // Create the SelectCommand.
            //command = new OleDbCommand("SELECT " + sField + " FROM " + _dbConn.TableName + sWhere, conn);
            //dataAdapter.SelectCommand = command;

            // Create the UpdateCommand.
            command = new OleDbCommand("UPDATE " + _dbConn.TableName + " SET " + sField + "= ? " + sWhere, _dbConn.GetOleConn());
            command.Parameters.Add(sField, OleDbType.VarChar, 2000, sField);
            
            //parameter.SourceVersion = DataRowVersion.Original;

            dataAdapter.UpdateCommand = command;
           
            try
            {
                _dbConn.TheDataTable.Rows[0][sField] = sValue;

                dataAdapter.Update(_dbConn.TheDataTable);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return true;
        }
    }
}

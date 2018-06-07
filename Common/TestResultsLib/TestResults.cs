using System;
using System.Collections.Generic;
using wsEpicorAccess;
using System.Data.SqlClient;

namespace TestResultsLib
{
    public class TestResults
    {
        const string AuTestConnString = "User Id=autest;password=autest1234;server=amc-sql01;database=AMC_TestData;connection timeout=30";
        //const string MfgConnString = "User Id=autest;password=autest1234;server=amc-sql01;database=AMC_MfgData;connection timeout=30";

        private EpicorAccess TheEpicorAcc = new EpicorAccess(true);
      
        private string SerialNumber { get; set; }
        private string PartNumber { get; set; }
        private string Version { get; set; }

        private enum TestType
        {
            Test = 0,
            Burn,
            TestBurn,
            ManualTest
        }

        // ja - public methods
        public bool bTestRequired { get; set; }
        public bool bBurnRequired { get; set; }
        public bool bManualTestRequired { get; set; }
        public bool bTestPassed { get; set; }
        public bool bBurnPassed { get; set; }
        public bool bTestBurnPassed { get; set; }
        public bool bManualTestPassed { get; set; }
        private bool bAbortResults { get; set; }
        private bool bFoundResults { get; set; }

        public TestResults()
        {
        }

        public TestResults(string sPartNumber, string sVersion)
        {
            PartNumber = sPartNumber;
            Version = sVersion;
        }

        public TestResults(string sWorkCode)
        {
            GetKeysFromEpicor(sWorkCode);
        }

        // TODO: ja - move...
        private bool GetKeysFromEpicor(string sWorkCode)
        {
            try
            {
                // ja - get the part number and version from Epicor
                string sPartNumber = TheEpicorAcc.GetJobPart(sWorkCode);
                Version = TheEpicorAcc.GetJobRevision(sWorkCode);

                // ja - remove the "=xx" because Epicor adds the version to part number
                if (sPartNumber.Contains("="))
                {
                    int nPos = sPartNumber.IndexOf("=");
                    sPartNumber = sPartNumber.Substring(0, nPos);
                }

                // ja - assign part number
                PartNumber = sPartNumber;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public void SetSerialNumber(string sNewSerial, bool bReadRequirementsOnly = false)
        {
            SerialNumber = sNewSerial;

            bTestPassed = false;
            bBurnPassed = false;            
            bTestBurnPassed = false;
            bManualTestPassed = false;

            bTestRequired = false;
            bBurnRequired = false;
            bManualTestRequired = false;

            // TODO: ja - not used for now.. but there was a reason if no test result to skip other results
            bAbortResults = false;
            bFoundResults = false;

            if (bReadRequirementsOnly)
            {
                // ja - read the requirements because there is no entry in the test db yet
                ReadRequirementsDatabase();
            }
            else
            {
                // TODO: ja - this needs to change to one call
                ReadResultsDatabase(TestType.Test);

                if (!bAbortResults)
                    ReadResultsDatabase(TestType.Burn);
                if (!bAbortResults)
                    ReadResultsDatabase(TestType.TestBurn);
                if (!bAbortResults)
                    ReadResultsDatabase(TestType.ManualTest);
            }

        }

        private bool ReadResultsDatabase(TestType type)
        {
            bool bRet = true;

            SqlConnection myConnection = new SqlConnection(AuTestConnString);

            try
            {
                myConnection.Open();

                string sSql = GetTestErrorCodeSqlString2(type);

                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(sSql, myConnection);

                myReader = myCommand.ExecuteReader();

                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        bFoundResults = true;

                        //Console.WriteLine(myReader["Operation"].ToString());
                        Console.WriteLine("TestErrorCode = " + myReader["TestErrorCode"].ToString());
                        var Result = myReader["TestErrorCode"];
                        var operation = myReader["operation"];
                        var bTestReq = myReader["regulartest"];
                        var bBurnReq = myReader["regularburn"];
                        var bManualtestReq = myReader["manualtest"];

                        bool bTheResult = (int)Result == 0;

                        if (type == TestType.Test)
                            bTestPassed = bTheResult;
                        else if (type == TestType.Burn)
                            bBurnPassed = bTheResult;
                        else if (type == TestType.TestBurn)
                            bTestBurnPassed = bTheResult;
                        else if (type == TestType.ManualTest)
                            bManualTestPassed = bTheResult;

                        // TODO: ja - Only assign once...
                        bTestRequired = (bool)bTestReq;
                        bBurnRequired = (bool)bBurnReq;
                        bManualTestRequired = (bool)bManualtestReq;
                    }
                }
                else
                {
                    if (!bFoundResults)
                    {
                        if (!bAbortResults)
                        {
                            ReadRequirementsDatabase();
                            //bAbortResults = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return bRet;
        }

        private string GetTestErrorCodeSqlString2(TestType type)
        {

            string sSql = @"select top 1 operation, testerrorcode, regulartest, regularburn, manualtest from BarCodeView where Baseplate like '" 
                            + SerialNumber + "' and Operation = ";

            if (type == TestType.Test)
                sSql += "'Computer Test'";
            else if (type == TestType.Burn)
                sSql += "'Computer Burn'";
            else if (type == TestType.TestBurn)
                sSql += "'Test/Burn/Test'";
            else if (type == TestType.ManualTest)
                sSql += "'Manual Test'";

            sSql += " Order by TestDate desc";

            return sSql;

        }

        private bool ReadRequirementsDatabase()
        {
            bool bRet = true;

            SqlConnection myConnection = new SqlConnection(AuTestConnString);

            try
            {
                myConnection.Open();

                string sSql = GetTestRequirementsSqlString();

                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(sSql, myConnection);

                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                   
                    var bTestReq = myReader["RegTest"];
                    var bBurnReq = myReader["RegBurn"];
                    var manualtest = myReader["ManTest"];                  

                    bTestRequired = (bool)bTestReq;
                    bBurnRequired = (bool)bBurnReq;
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return bRet;
        }

        private string GetTestRequirementsSqlString()
        {
            string sWorkCode = "";
            GetWorkcode(ref sWorkCode);

            GetKeysFromEpicor(sWorkCode);

            string sSql = @"select top 1 RegTest, RegBurn, ManTest from TestOps where Model = '"
                          + PartNumber + "' and Version = '" + Version + "'";          

            return sSql;

        }

        private bool GetWorkcode(ref string sWorkCode)
        {
            if (SerialNumber.Length < 5)
                return false;

            // ja - extract workcode
            sWorkCode = SerialNumber.Substring(0, 5);

            if (String.IsNullOrWhiteSpace(sWorkCode))
                return false;

            int n;
            if (!int.TryParse(sWorkCode.Substring(0, 5), out n))
                return false;

            return true;
        }

        public string GetPartNumber()
        {
            return PartNumber;
        }

        public bool TestRequired()
        {
            return bTestRequired;
        }

        public bool BurnRequired()
        {
            return bBurnRequired;
        }

        public bool ManualTestRequired()
        {
            return bManualTestRequired;
        }        

        public bool PassedTest()
        {
            return bTestPassed;
        }

        public bool PassedBurn()
        {           
            return bBurnPassed;
        }

        public bool PassedTestBurn()
        {
            return bTestBurnPassed;
        }

        public bool PassedManualTest()
        {
            return bManualTestPassed;
        }

        public List<string> GetFailedTypes()
        {
            // ja - returns a list of strings that did not pass
            List<string> retString = new List<string>();

            // ja - test is required
            if (TestRequired() && !PassedTest())
                retString.Add("Test");

            // ja - test is required
            if (ManualTestRequired() && !PassedManualTest())
                retString.Add("Manual Test");

            // ja - burn is required (test for test)
            if (TestRequired() && BurnRequired() && !PassedBurn())
                retString.Add("Burn");

            // ja - test is not required , burn is required and return the or of burn and test/burn
            if (!TestRequired() && BurnRequired())
            {
                bool bPassed = (PassedBurn() || PassedTestBurn());

                if (!bPassed)
                    retString.Add("TestBurn");
            }

            return retString;
        }

        static public void LogLables(List<string> serialsList, bool bIsRma, string sLabelStation, string sLabelType, bool BurstedLabels)
        {
            int nInsertedKey = -1;  
            var WorkCode = serialsList[0].Substring(0, 5);
            var UserName = Environment.UserName;
            var ComputerName = Environment.MachineName;
            //string SerialData = "[" + string.Join("], [", serialsList) + "]";
            var PrintedLabelsCnt = serialsList.Count;

            // ja - open the mfg database and insert the printed label information          
            using (SqlConnection conn = new SqlConnection(ConnectonStrings.GetMfgDataConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;

                    // ja - create insert command with scope identity so we can get the key after the insert
                    string sqlInsert = @"Insert Into LabelsLog (WorkCode, IsRMA, BurstedLabels, LabelStation, LabelType, ComputerName, UserName, PrintedLabelsCnt) 
                                        Values (@WorkCode, @IsRMA, @BurstedLabels, @LabelStation, @LabelType, @ComputerName, @UserName, @PrintedLabelsCnt);
                                        SELECT SCOPE_IDENTITY()";
                    cmd.CommandText = sqlInsert;

                    // ja - populate the parmaerters 
                    cmd.Parameters.AddWithValue("@WorkCode", WorkCode);
                    cmd.Parameters.AddWithValue("@IsRMA", bIsRma);
                    cmd.Parameters.AddWithValue("@BurstedLabels", BurstedLabels);                    
                    cmd.Parameters.AddWithValue("@LabelStation", sLabelStation);
                    cmd.Parameters.AddWithValue("@LabelType", sLabelType);
                    cmd.Parameters.AddWithValue("@ComputerName", ComputerName);
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@PrintedLabelsCnt", PrintedLabelsCnt);
                    //cmd.Parameters.AddWithValue("@Serials", SerialData);

                    try
                    {
                        conn.Open();
                        
                        // ja - get the last key
                        nInsertedKey = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Parameters.Clear();

                        foreach (string sSerial in serialsList)
                        {
                            string sqlSerialInsert = @"Insert Into LabelsLogSerials (SerialNumber, JobId) 
                                        Values (@SerialNumber, @JobId)";

                            cmd.CommandText = sqlSerialInsert;

                            // ja - populate the permaerters 
                            cmd.Parameters.AddWithValue("@SerialNumber", sSerial);
                            cmd.Parameters.AddWithValue("@JobId", nInsertedKey);

                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                        }                  
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }           
        }
    }
}
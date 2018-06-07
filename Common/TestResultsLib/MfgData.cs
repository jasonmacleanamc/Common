using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


/* TODO:
 * 
 --Change to index(Top level ID) instead of part number
  
 --Set and unset the accocated flag in top level 
 
 Make sure timestamp is updating in sub database
 
 
 */
  
namespace TestResultsLib
{
    public class MfgData
    {
        public string TopLevelSerial { get; private set; }
        public int TopLevelId { get; private set; }
        private string WorkCode
        {
            get
            {
                return TopLevelSerial.Substring(0, 5);
            }

        }

        public struct SubAssembliyInfo
        {
            public int Index;
            public string SerialNumber;
            public string PartNumber;
            public string Version;
            public bool Associated;
            public bool InDatabase;            
        }

        private List<SubAssembliyInfo> EpicorInfo = new List<SubAssembliyInfo>();
        private List<SubAssembliyInfo> DatabaseInfo = new List<SubAssembliyInfo>();

        public List<SubAssembliyInfo> MergedInfo = new List<SubAssembliyInfo>();

        protected SqlConnection TheConnection { get; private set; }

        public MfgData()
        {
            
        }       

        public void Go(string serialNum)
        {
            // ja - read in top level serial number to see if it exists
            //      if does not exist add to Top Level Serial Info (Insert)
            //
            //      if it does exist then read in sub assemblies and disable them in list

            // ja - allow user to select 1 or more of the enabled subs and add to sub data

            try
            {
                SetTopLevelSerial(serialNum);

                // ja - read in list of subs from Epicor that can be associated
                ReadSubsFromEpicor();

                // ja - does this exist in database
                VerifyTopLevel();

                // ja - populate from database
                ReadSubs();

                // ja - merge the Epicor and database lists
                MergeLists();

            }
            catch (Exception)
            {
                throw;
            }           
        }
       
        private void MergeLists()
        {
            try
            {
                // ja - create a temporary list
                List<SubAssembliyInfo> mergedList = new List<SubAssembliyInfo>();

                int i = 0;
                bool found = false;
                foreach (var epicorItem in EpicorInfo)
                {
                    // ja - once you have found it in the db skip it next time
                    found = false;
                    foreach (var dbItem in DatabaseInfo.Skip(i))
                    {
                        // ja - check part number and version (does not matter for dupes)
                        if (epicorItem.PartNumber == dbItem.PartNumber && epicorItem.Version == dbItem.Version)
                        {
                            i++;
                            // ja - add the item from the database
                            mergedList.Add(dbItem);
                            found = true;
                            break;
                        }
                    }

                    // ja - not found in database add the item from epicor
                    if (!found)
                        mergedList.Add(epicorItem);

                }

                MergedInfo.Clear();
                MergedInfo = mergedList;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

       }

        public void SetTopLevelSerial(string sTopLevelSerial)
        {
            TopLevelSerial = sTopLevelSerial;
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

        public void VerifyTopLevel()
        {
            if (!DoesTopLevelExistInDB())
                CreateTopLevel();

        }

        public int FindPartNumberMatch(string serialNumber)
        {
            string partNumber = "";
            string partVersion = "";
            string tempWorkCode = serialNumber.Substring(0, 5);

            try
            {
                EpicorData ep = new EpicorData();
                ep.GetPartInfo(tempWorkCode, ref partNumber, ref partVersion);

                foreach (var item in MergedInfo)
                {
                    if ((partNumber.Trim() == item.PartNumber) && (partVersion.Trim() == item.Version))
                    {
                        if (!item.Associated)
                            return item.Index;
                    }
                }
            }
            catch (Exception)
            {
                string sMsg = string.Format("WorkCode {0} Not Found in Epicor", tempWorkCode);
                throw new System.InvalidOperationException(sMsg);
                //throw;
            }          

            return -1;
        }

        public void CreateTopLevel()
        {            
            string partNumber = "";
            string partVersion = "";

            EpicorData ep = new EpicorData();
            ep.GetPartInfo(WorkCode, ref partNumber, ref partVersion);

            // ja - open the mfg database and insert the printed label information          
            using (SqlConnection conn = new SqlConnection(ConnectonStrings.GetMfgDataConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;

                    // ja - create insert command with scope identity so we can get the key after the insert
                    string sqlInsert = @"Insert Into AssociatedTopLevelSerialInfo (TopLevelSerialNumber, PartNumber, PartVersion) 
                                        Values (@TopLevelSerialNumber, @PartNumber, @PartVersion);
                                        SELECT SCOPE_IDENTITY()";
                    cmd.CommandText = sqlInsert;

                    // ja - populate the parameters 
                    cmd.Parameters.AddWithValue("@TopLevelSerialNumber", TopLevelSerial);
                    cmd.Parameters.AddWithValue("@PartNumber", partNumber);
                    cmd.Parameters.AddWithValue("@PartVersion", partVersion);

                    try
                    {
                        conn.Open();

                        // ja - get the top level key
                        TopLevelId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        string sMsg = string.Format("Insert into Database Failed... {0}", ex.Message);
                        throw new System.InvalidOperationException(sMsg);
                    }
                }
            }
        }

        public void ReadSubsFromEpicor()
        {
            EpicorInfo.Clear();           

            // ja - local class to wrap Epicor functionality
            EpicorData epicorData = new EpicorData();

            List<string> subs = epicorData.GetSubs(WorkCode);

            foreach (var item in subs)
            {
                string partNumber = null;
                string version = null;
                epicorData.ExtractPartInfo(item, ref partNumber, ref version);

                SubAssembliyInfo info = new SubAssembliyInfo();

                info.PartNumber = partNumber.Trim();
                info.Version = version.Trim();
                info.InDatabase = false;
                info.Associated = false;

                EpicorInfo.Add(info);
            }            
        }


        public bool DoesTopLevelExistInDB()
        {
            bool bRet = false;

            try
            {
                if (!OpenConnection())
                    throw new System.ArgumentException("Cannot open Database", "MfgData");


                string sSql = @"select * from AssociatedTopLevelSerialInfo where TopLevelSerialNumber = '"
                          + TopLevelSerial + "'";
          
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(sSql, TheConnection);

                myReader = myCommand.ExecuteReader();

                if (myReader.HasRows)
                {
                    myReader.Read();

                    TopLevelId = (int)myReader["TopLevelID"];

                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return bRet;
        }

        public bool ReadSubs()
        {
            bool bRet = false;

            DatabaseInfo.Clear();

            try
            {
                if (!OpenConnection())
                    throw new System.ArgumentException("Cannot open Database", "MfgData");


                string sSql = @"select * from AssociatedSubAssemblies where TopLevelID = '"
                          + TopLevelId + "'";
             
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(sSql, TheConnection);

                myReader = myCommand.ExecuteReader();

                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        int Index = (int)myReader["SubId"];
                        string SerialNumber = (string)myReader["SubSerialNumber"];
                        string PartNumber = (string)myReader["SubPartNumber"];
                        bool Associated = (bool)myReader["SubModded"];
                        string version = (string)myReader["SubPartVersion"];
                        PartNumber = PartNumber.Trim();

                        SubAssembliyInfo info = new SubAssembliyInfo
                        {
                            Index = Index,
                            SerialNumber = SerialNumber.Trim(),
                            PartNumber = PartNumber.Trim(),
                            Version = version.Trim(),
                            InDatabase = true,
                            Associated = Associated
                        };

                        DatabaseInfo.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                string sMsg = string.Format("Reading Subs from Database Failed.. {0}", ex.Message);
                throw new System.InvalidOperationException(sMsg);
            }

            return bRet;
        }


        private bool GetAssocatedFlag()
        {
            bool bRet = false;           

            try
            {
                if (!OpenConnection())
                    throw new System.ArgumentException("Cannot open Database", "MfgData");


                string sSql = @"select SubPartNumber, SubPartVersion, Associated from AssociatedTopLevelSerialInfo where TopLevelId = '"
                          + TopLevelId + "'";
               

                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(sSql, TheConnection);

                myReader = myCommand.ExecuteReader();

                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {

                        bool Associated = (bool)myReader["Associated"];
                        bRet = true;                      
                       
                    }
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return bRet;
        }       

        public void UpdateAssocatedFlag()
        {
            bool bAssociated = false;
            foreach (var item in MergedInfo)
            {
                if (item.Associated)
                {
                    bAssociated = true;
                    break;
                }
            }

            // ja - open the mfg database and insert the printed label information          
            using (SqlConnection conn = new SqlConnection(ConnectonStrings.GetMfgDataConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;

                    // ja - create insert command with scope identity so we can get the key after the insert
                    string sqlUpdate = @"Update AssociatedTopLevelSerialInfo Set Associated = @Associated
                                         where TopLevelId = " + @"'" + TopLevelId + @"'";
                    cmd.CommandText = sqlUpdate;

                    // ja - populate the parameters 
                    cmd.Parameters.AddWithValue("@Associated", bAssociated);                  

                    try
                    {
                        conn.Open();

                        // ja - get the top level key
                        cmd.ExecuteScalar();
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }

        public void AddSubSerialNumber(int nIndex, string newSerialNumber)
        {
            SubAssembliyInfo info = MergedInfo[nIndex];
            info.SerialNumber = newSerialNumber;
            info.Associated = !string.IsNullOrEmpty(newSerialNumber);

            MergedInfo[nIndex] = info;

            if (MergedInfo[nIndex].InDatabase == true)
                UpdateModdedFlag(nIndex);
            else
                AssociateSubAssembly(nIndex);

            UpdateAssocatedFlag();

        }

        public void UpdateModdedFlag(int index)
        {
            string PartNumber = MergedInfo[index].PartNumber;
            string SerialNumber = MergedInfo[index].SerialNumber;
            int SubId = MergedInfo[index].Index;

            // ja - open the mfg database and insert the printed label information          
            using (SqlConnection conn = new SqlConnection(ConnectonStrings.GetMfgDataConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;

                    // TODO: ja - add version check

                    // ja - create insert command with scope identity so we can get the key after the insert
                    //string sqlUpdate = @"Update AssociatedSubAssemblies Set SubSerialNumber = @SubSerialNumber, SubModded = @SubModded
                    //                     where SubPartNumber = " + @"'" + PartNumber + @"'";

                    string sqlUpdate = @"Update AssociatedSubAssemblies Set SubSerialNumber = @SubSerialNumber, SubModded = @SubModded
                                         where SubId = " + @"'" + SubId + @"'";

                    cmd.CommandText = sqlUpdate;

                    bool modded = true;
                    if (string.IsNullOrEmpty(SerialNumber))
                        modded = false;

                    // ja - populate the parameters 
                    cmd.Parameters.AddWithValue("@SubModded", modded);
                    cmd.Parameters.AddWithValue("@SubSerialNumber", SerialNumber);

                    try
                    {
                        conn.Open();

                        // ja - get the top level key
                        cmd.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }

        public void AssociateSubAssembly(int index)
        {
            int nInsertedKey = -1;

            SubAssembliyInfo newItem = MergedInfo[index];

            string SubSerialNumber = newItem.SerialNumber.Trim();
            string SubPartNumber = newItem.PartNumber.Trim();
            string SubPartVersion = newItem.Version.Trim();

            // ja - open the mfg database and insert the printed label information          
            using (SqlConnection conn = new SqlConnection(ConnectonStrings.GetMfgDataConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;

                    // ja - create insert command with scope identity so we can get the key after the insert
                    string sqlInsert = @"Insert Into AssociatedSubAssemblies (TopLevelID, SubSerialNumber, SubPartNumber, SubPartVersion, SubModded) 
                                        Values (@TopLevelID, @SubSerialNumber, @SubPartNumber, @SubPartVersion, @SubModded);
                                        SELECT SCOPE_IDENTITY()";
                    cmd.CommandText = sqlInsert;                    

                    // ja - populate the permaerters 
                    cmd.Parameters.AddWithValue("@TopLevelID", TopLevelId);
                    cmd.Parameters.AddWithValue("@SubSerialNumber", SubSerialNumber);
                    cmd.Parameters.AddWithValue("@SubPartNumber", SubPartNumber);
                    cmd.Parameters.AddWithValue("@SubPartVersion", SubPartVersion);
                    cmd.Parameters.AddWithValue("@SubModded", true);

                    try
                    {
                        conn.Open();

                        // ja - get the last key
                        nInsertedKey = Convert.ToInt32(cmd.ExecuteScalar());                  
                        
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


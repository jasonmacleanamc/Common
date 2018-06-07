using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wsEpicorAccess;

namespace TestResultsLib
{
    public class EpicorData
    {

        private EpicorAccess TheEpicorAcc = new EpicorAccess(true);

        public List<string> GetSubs(string workCode)
        {
            List<string> subs = new List<string>();

            try
            {
                // ja - ask Epicor for the list of subs                
                subs = TheEpicorAcc.GetSerializedSubs(workCode);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string sMsg = string.Format("Workcode {0} not found in Epicor", workCode);
                throw new System.InvalidOperationException(sMsg);
            }
          
            return subs;
        }

        public void GetPartInfo(string workCode, ref string partNumber, ref string version)
        {
            try
            {
                string temppartNumber = TheEpicorAcc.GetJobPart(workCode);
                version = TheEpicorAcc.GetJobRevision(workCode);

                partNumber = CleanPartNumber(temppartNumber);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CleanPartNumber(string epPartNumber)
        {
            string partNumber = "";
            if (epPartNumber.Contains("="))
            {
                int nPos = epPartNumber.IndexOf("=");
                partNumber = epPartNumber.Substring(0, nPos);                
            }

            return partNumber;
        }

        public void ExtractPartInfo(string fullPartNum, ref string partNumber, ref string version)
        {
           
            if (fullPartNum.Contains("="))
            {
                int nPos = fullPartNum.IndexOf("=");
                partNumber = fullPartNum.Substring(0, nPos);
                version = fullPartNum.Substring(nPos + 1, 2);

                decimal temp = Convert.ToDecimal(version);
                temp = (temp * .01m);

                version = temp.ToString();
            }
        }

        private bool GetKeysFromEpicor(string sWorkCode)
        {
            try
            {
                // ja - get the part number and version from Epicor
                string sPartNumber = TheEpicorAcc.GetJobPart(sWorkCode);
                var Version = TheEpicorAcc.GetJobRevision(sWorkCode);

                string partNumber = null;
                string version = null;

                // ja - remove the "=xx" because Epicor adds the version to part number
                ExtractPartInfo(sPartNumber, ref partNumber, ref version);
               
                // ja - assign part number
                var PartNumber = partNumber;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}

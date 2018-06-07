using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestResultsLib
{
    static class ConnectonStrings
    {
        const string MfgConnString = "User Id=autest;password=autest1234;server=amc-sql01;database=AMC_MfgData;connection timeout=30";
        const string AuTestConnString = "User Id=autest;password=autest1234;server=amc-sql01;database=AMC_TestData;connection timeout=30";

        public static string GetMfgDataConnectionString()
        {
            return MfgConnString;
        }

        public static string GetAuTestConnectionString()
        {
            return AuTestConnString;
        }
    }
}

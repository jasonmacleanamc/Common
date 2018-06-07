using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCDatabase
{
    public enum dbType
    {
        vfp,
        sql
    }

    public class AMCDatabase
    {
        public static string QUOTE = "\"";
        public static string SINGLE_QUOTE = "\'";
        public static string INT = "^INT";

        public int DataBaseType { get; set; }
        public string TableName { get; set; }
        public string KeyIdentifier { get; set; }
        public string KeyValue { get; set; }

        public string FormatQuote(string sValue)
        {
            if (DataBaseType == (int)dbType.vfp)
            {
                return AmcDataConnection.QUOTE + sValue + AmcDataConnection.QUOTE;
            }
            else
            {
                return AmcDataConnection.SINGLE_QUOTE + sValue + AmcDataConnection.SINGLE_QUOTE;
            }
        }

    }
}

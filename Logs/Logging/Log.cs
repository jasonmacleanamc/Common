using System;
using System.IO;

namespace AMCCommon
{
    public class Log
    {
        public StreamWriter TheStream;
        public string _sLogFileName = "";

        public bool LogUserName { get; set; }

        public Log(string sPath, bool bUserName = false, bool bDefaultName = false)
        {
            if (bDefaultName)
                _sLogFileName = "Test_File.txt";
            else
                GetLogFileName(sPath);

            TheStream = new StreamWriter(_sLogFileName, append: true);
            LogUserName = bUserName;
        }

        private void GetLogFileName(string sPath)
        {
            // ja - create a valid log name with todays date and time in the current directory 
            string sDir = sPath;

            if (!Directory.Exists(sDir))
            {
                Directory.CreateDirectory(sDir);
            }

            if (LogUserName)
                _sLogFileName = sDir + @"\" + GetUserName() + ":" + DateTime.Now.ToString("yyyyMMdd-HHmm") + @".log";
            else
                _sLogFileName = sDir + @"\" + DateTime.Now.ToString("yyyyMMdd-HHmm") + @".log";
        }

        public void WriteInfo(string sInfo)
        {
            TheStream.WriteLine(sInfo);
            Console.WriteLine(sInfo);
            TheStream.Flush();
        }

        public string GetUserName()
        {
            return Environment.UserName;
        }

        public void CloseLogging()
        {
            TheStream.Close();
        }
    }
}

using System;
using System.IO;

namespace FWR.Engine
{
    public class Log
    {

        private string _logFilePath;

        public Log(string logFilePath)
        {
            _logFilePath = logFilePath;
        }
      

        public void Error(Object message, Exception exception)
        {

        }

        public void Debug(Object message)
        {

        }

        public void Info(String message)
        {
            DateTime time = DateTime.Now;
            string timeStamp = time.ToString(@"dd/MM hh\:mm\:ss");

            String strng = timeStamp + "|Info|" + message + Environment.NewLine;
            File.AppendAllText(_logFilePath, strng);
        }
    }
}

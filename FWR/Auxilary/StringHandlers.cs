using System;

namespace FWR.UI_Aux
{
    public static class StringHandlers
    {
        public static string Unescape(string originalString)
        {
            return originalString.Replace(@"\\", @"\").Replace("\"", "");
        }

        public static string CleanName(string originalString)
        {
            return NoLineFeed(originalString.Replace(" ", "_"));
        }

        public static string NoLineFeed(string originalString)
        {
            return originalString.Replace("\r", "").Replace("\n", "");
        }


        public static string IntSecondsToHhMmSsString(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString(@"hh\:mm\:ss");
        }

        public static string ShortDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString(@"dd_MM-hh_mm");
        }

        public static string CompactDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString(@"ddMMhhmm");
        }
    }
}

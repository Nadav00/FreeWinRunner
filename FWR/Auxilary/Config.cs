using FWR.Engine;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows;

namespace FWR.UI_Aux
{
    public static class Config
    {

        public static string MAIN_DIR_DEFAULT = @"""C:\FreeWinRunner\FWR""";
        public static string cfgFilePath = ".\\Config.txt";
        

        public static void Load_Config()
        {
            if (!Config_Exists())
                Create_Config();

            Load_Config_Perform();
        }

        public static bool Config_Exists()
        {
            return (File.Exists(cfgFilePath));
        }

        public static void Create_Config()
        {
            Runtime.Config newConfig = new Runtime.Config()
            {
                MAIN_DIR = MAIN_DIR_DEFAULT,
                MAX_CUNCURRENT_RUNNING_CYCLES = 10,
            };

            var myObj = new JavaScriptSerializer().Serialize(newConfig);
            File.WriteAllText(cfgFilePath, myObj);
        }

        public static void Edit_Config()
        {
            Process.Start("notepad.exe", cfgFilePath);
            Load_Config_Perform();
        }

        public static void Load_Config_Perform()
        {
            string lines = System.String.Concat(File.ReadAllLines(cfgFilePath));
            Runtime.config = new JavaScriptSerializer().Deserialize<Runtime.Config>(lines);
            string main_dir = StringHandlers.Unescape(Runtime.config.MAIN_DIR);
        }
    }
}

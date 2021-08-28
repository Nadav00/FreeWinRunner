using FWR.Engine;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System;

namespace FWR.UI_Aux
{
    public static class Config
    {

        public static string MAIN_DIR_DEFAULT = @"""C:\FreeWinRunner\FWR\FWR""";
        public static string cfgFilePath = ".\\Config.txt";
        public static string RELATIVE_DB_PATH_DEFAULT = @".\\FWR.db";

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
                DB_FILE_START_WITH_DOT_TO_MAKE_RELATIVE_TO_MAIN_DIR = "FWR.db",
            };

            string json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
            File.WriteAllText(cfgFilePath, json);
        }

        public static void Edit_Config()
        {
            Process.Start("notepad.exe", cfgFilePath);
            Load_Config_Perform();
        }

        public static void Load_Config_Perform()
        {
            string lines = String.Concat(File.ReadAllLines(cfgFilePath));
            Runtime.config = JsonConvert.DeserializeObject<Runtime.Config>(lines);

            string main_dir = StringHandlers.Unescape(Runtime.config.MAIN_DIR);
        }
    }
}

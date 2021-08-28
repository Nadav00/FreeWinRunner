using System.Collections.Generic;

namespace FWR.Engine
{
    public static class Runtime
    {
        public static Queue queue = new Queue();
        public static Config config { get; set; } = new Config();
        public static List<Resource> resources;
        public static MainEngine engine = new MainEngine();
        public static Const.Status status = Const.Status.New;

        private static MainWindow _mainwindow;

        public class Config
        {
            public string MAIN_DIR { get; set; } = @"C:\\FreeWinRunner\\FWR\\FWR";
            public int MAX_CUNCURRENT_RUNNING_CYCLES { get; set; } = 1;
            public string DB_FILE_START_WITH_DOT_TO_MAKE_RELATIVE_TO_MAIN_DIR { get; set; } = "FWR.db";
        }

        public static void SetMainWindow(MainWindow window)
        {
            _mainwindow = window;
        }

        public static MainWindow GetMainWindow()
        {
            return _mainwindow;
        }
    }
}

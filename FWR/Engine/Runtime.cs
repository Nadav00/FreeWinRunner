using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWR.Engine
{
    public static class Runtime
    {
        public static Queue queue = new Queue();
        public static Config config;
        public static List<Resource> resources;
        public static MainEngine engine = new MainEngine();
        public static Const.Status status = Const.Status.New;

        private static MainWindow _mainwindow;

        public class Config
        {
            public string MAIN_DIR;
            public int MAX_CUNCURRENT_RUNNING_CYCLES;
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

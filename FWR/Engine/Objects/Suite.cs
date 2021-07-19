using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWR.Engine
{
    public class Suite
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string SuiteFilePath { get; set; }
        public Environments Environments { get; set; }
        public List<Test> Tests { get; set; }
        public Const.Status Status { get; set; } = Const.Status.New;
        public int TotalSecondsRunning { get; set; }
        public string logFilePath { get; set; }

    }
}

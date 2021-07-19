using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FWR.Engine
{

    public class Cycle
    {
        public string Name { get; set; }
        public List<Suite> Suites { get; set; }
        public int ID { get; set; }
        public bool Active { get; set; }
        public Const.Status Status { get; set; } = Const.Status.New;
        public Thread CycleWorker { get; set; }
        public int TotalSecondsRunning { get; set; }
        public string logFilePath { get; set; }

        public Cycle(string name)
        {
            Name = name;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FWR.Engine
{

    public class Cycle
    {
        public string Name { get; set; }
        public List<Suite> Suites { get; set; }
        public int ID { get; set; }
        public bool Active { get; set; }
        public Const.Status Status { get; set; } = Const.Status.New;
        public Const.Result Result { get; set; } = Const.Result.New;
        public Thread CycleWorker { get; set; }
        public int TotalSecondsRunning { get; set; }
        public string LogFilePath { get; set; }

        public ListView CycleUiObject { get; set; }

        public Cycle(string name)
        {
            Name = name;
        }

        public bool AllSuitesFinished()
        {
            foreach (var suite in Suites ?? new List<Suite>())
                if (suite.Status != Const.Status.Finished)
                    return false;

            return true;
        }

        public bool AllSuitesPass()
        {
            foreach (var suite in Suites ?? new List<Suite>())
                if (suite.Result != Const.Result.Pass)
                    return false;

            return true;
        }

    }
}

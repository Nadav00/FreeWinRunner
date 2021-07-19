using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWR.Engine
{
    public class Test
    {
        public Const.TestType Type { get; set; } = Const.TestType.Normal;
        public Const.Status Status { get; set; } = Const.Status.New;
        public Const.Result Result { get; set; } = Const.Result.New;

        public string Name { get; set; }
        public double ID { get; set; }
        public double MaxSeconds { get; set; } = 3600;

        public bool Selected { get; set; }
        public bool Activated { get; set; }

        public string ScriptPath { get; set; }
        public List<Resource> Resources { get; set; }

        public int TotalSecondsRunning { get; set; }
        public string logFilePath { get; set; }
    }
}

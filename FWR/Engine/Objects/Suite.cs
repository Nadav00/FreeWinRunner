using FWR.UI_Controls;
using Newtonsoft.Json;
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
        public Const.Result Result { get; set; } = Const.Result.New;
        public int TotalSecondsRunning { get; set; }
        public string logFilePath { get; set; }

        [JsonIgnore]
        public bool needUiUpdate { get; set; } = false;

        [JsonIgnore]
        public Cycle cycle { get; set; }

        [JsonIgnore]
        public SuiteInCycleControl suiteInCycleControl { get; set; }

        public Test GetRunningTest()
        {
            foreach (var test in Tests ?? new List<Test>())
            {
                if (test.Status == Const.Status.Running)
                    return test;
            }
            return null;
        }

        public int TestsToRun()
        {
            int counter = 0;

            foreach (var test in Tests ?? new List<Test>())
            {
                if (test.Status == Const.Status.Running || test.Status == Const.Status.New)
                    counter++;
            }

            return counter;
        }

        public int TotalPassTests()
        {
            int counter = 0;

            foreach (var test in Tests ?? new List<Test>())
            {
                if (test.Result == Const.Result.Pass)
                    counter++;
            }

            return counter;
        }

        public Test FindNextTestToRun()
        {
            foreach (var test in Tests ?? new List<Test>())
            {
                if (test.Status == Const.Status.New)
                    return test;
            }
            return null;
        }

        public void SetNeedUiUpdate()
        {
            needUiUpdate = true;
            cycle.SetNeedUiUpdate();
        }
    }
}

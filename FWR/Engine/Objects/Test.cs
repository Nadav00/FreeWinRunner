using FWR.UI_Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms.Integration;

namespace FWR.Engine
{
    public class Test
    {
        public WindowsFormsHost WindowsFormsHostControl { get; set; }
        public Process ShellProcess { get; set; }
        public TestInSuiteInQueueControl testInSuiteInQueueControl { get; set; }
        public Suite suite { get; set; }


        public Const.TestType Type { get; set; } = Const.TestType.Normal;
        public Const.Status Status { get; set; } = Const.Status.New;
        public Const.Result Result { get; set; } = Const.Result.New;

        public string Name { get; set; }
        public double ID { get; set; }
        public double MaxSeconds { get; set; } = 3600;
        public string Error { get; set; }

        public bool Selected { get; set; }
        public bool Activated { get; set; }

        public string ScriptPath { get; set; }
        public List<Resource> Resources { get; set; }

        public int TotalSecondsRunning { get; set; }
        public string logFilePath { get; set; }

        public bool needUiUpdate { get; set; }

        public void SetUiNeedUpdate()
        {
            needUiUpdate = true;
            suite.SetNeedUiUpdate();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWR.Engine
{
    public static class Const
    {
        public enum TestType
        {
            Init,
            Prepare,
            Setup,
            Script,
            Normal,
        }

        public enum Status
        {
            Empty,
            New,
            Running,
            Paused,
            Stopped,
            Terminated,
            Finished
        }

        public enum Result
        {
            New,
            Pass,
            PassWithIssues,
            Fail,
            Error,
            Inconclusive,
            Skipped,
            Terminated
        }

        static public string cycleNamePrefix = "cycleName";
        static public string cycleObject = "cycleObject";
        static public string addSuiteButton = "add";
        static public string newSuiteButton = "newSuiteButton";
        static public string cycleExpander = "cycleExpander";
        static public string cycleBody = "cycleBody";
        static public string cycleBodySuite = "cycleBodySuiteControl";
        static public string cycleBodySuiteTest = "cycleBodySuiteTestControl";
        static public string cycleActiveCheckbox = "cycleActiveCheckbox";
        static public string cycleTimeLabelLabel = "timeLabelLabel";
        static public string cycleTimeCaptionLabel = "cycleTimeCaptionLabel";

        static public string projectSubfolder = "Projects";
        static public string EnvironmentSubfolder = "Environments";
        static public string TestInSuiteUiObj = "TestInSuiteUiObj_ID_";
    }
}

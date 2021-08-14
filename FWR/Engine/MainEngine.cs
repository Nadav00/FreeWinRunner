using FWR.UI_Aux;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;


namespace FWR.Engine
{
    public class MainEngine
    {
        private Thread MAINTHRD;
        private Log log;
        MainWindow FORM;

        private System.Windows.Threading.DispatcherPriority IDLE = System.Windows.Threading.DispatcherPriority.ApplicationIdle;
        private System.Windows.Threading.DispatcherPriority HIGH = System.Windows.Threading.DispatcherPriority.Send;


        SolidColorBrush RED = new SolidColorBrush(Colors.Red);
        SolidColorBrush GREEN = new SolidColorBrush(Colors.Green);
        SolidColorBrush LBLUE = new SolidColorBrush(Colors.CornflowerBlue);
        SolidColorBrush ORANGE = new SolidColorBrush(Colors.Orange);

        private List<Const.Status> Startable = new List<Const.Status>()
        {
            Const.Status.New,
            Const.Status.Stopped,
        };

        private List<Const.Status> Stoppable = new List<Const.Status>()
        {
            Const.Status.Running,
            Const.Status.Paused
        };

        private List<Const.Status> Ended = new List<Const.Status>()
        {
            Const.Status.Stopped,
            Const.Status.Terminated,
            Const.Status.Finished
        };


        public void StartRun()
        {
            FORM = Runtime.GetMainWindow();
            string logPath = StringHandlers.CleanName(FORM.NameTextBox.Text + "MainEngine.log");
            log = new Log(Path.Combine(Path.GetTempPath(), "FWR", logPath));

            if (Runtime.queue?.Cycles?.Count > 0)
                log.Info("Starting Main Engine");
            else
            { log.Info("Nothing to Start"); return; }

            if (Startable.Contains(Runtime.status))
            {
                Runtime.status = Const.Status.Running;
                MAINTHRD = new Thread(MainEngineRun);
                MAINTHRD.Name = "QUEUE_MAIN_THRD";
                MAINTHRD.Priority = ThreadPriority.AboveNormal;
                MAINTHRD.Start();
            }
        }

        public void StopRun()
        {
            if (Stoppable.Contains(Runtime.status))
            {
                log.Info("Stopping Main Engine");
                Runtime.status = Const.Status.Stopped;
                MAINTHRD.Abort();
                MAINTHRD = null;

                foreach (Cycle cycle in Runtime.queue.Cycles)
                {
                    if (Stoppable.Contains(cycle.Status))
                    {
                        cycle.Status = Const.Status.Stopped;
                        cycle.CycleWorker.Abort();
                        cycle.CycleWorker = null;
                    }
                }
            }
        }

        private void MainEngineRun()
        {
            while (Runtime.status == Const.Status.Running && Runtime.queue.Cycles != null)
            {
                bool markedFinished = true;

                foreach (Cycle cycle in Runtime.queue.Cycles)
                {
                    if (!Ended.Contains(cycle.Status))
                        markedFinished = false;

                    if (cycle.Active && Startable.Contains(cycle.Status))
                    {
                        cycle.Status = Const.Status.Running;

                        int ID = cycle.ID;
                        string controlName = Const.cycleObject + ID;

                        FORM.Dispatcher.Invoke(IDLE, new Action(delegate () { StyleControl(controlName, "Background", LBLUE); }));

                        foreach (Suite suite in cycle.Suites ?? new List<Suite>())
                            if (Startable.Contains(suite.Status))
                            {
                                suite.Status = Const.Status.Running;
                                FORM.Dispatcher.Invoke(HIGH, new Action(delegate () { StyleControl(suite.suiteInCycleControl.Name, "Background", LBLUE); }));
                            }
                        

                        cycle.CycleWorker = new Thread(() => CycleWorkerThread(cycle));
                        cycle.CycleWorker.Name = cycle.Name + "_RunThread";
                        cycle.CycleWorker.Start();
                    }
                }

                if (markedFinished)
                    Runtime.status = Const.Status.Finished;

                Thread.Sleep(1000);
            }
        }

        private void CycleWorkerThread(Cycle cycle)
        {
            while (cycle.Status == Const.Status.Running)
            {
                cycle.TotalSecondsRunning++;

                foreach (Suite suite in cycle.Suites ?? new List<Suite>())
                {
                    suite.TotalSecondsRunning++;
                    if (suite.Status == Const.Status.Running)
                    {

                        var runningTest = suite.GetRunningTest();

                        if (runningTest != null)
                        {
                            //if (OldOrDead(runningTest))
                            //  Kill_Test(cycle, suite, test);

                        }
                        else
                            Start_Test(cycle, suite, suite.FindNextTestToRun());
                    }

                    if (suite.TestsToRun() == 0)
                    {
                        suite.Status = Const.Status.Finished;

                        if (suite.TotalPassTests() == suite.Tests.Count)
                        {
                            suite.Result = Const.Result.Pass;
                            FORM.Dispatcher.Invoke(HIGH, new Action(delegate () { StyleControl(suite.suiteInCycleControl.Name, "Background", GREEN); }));
                        }
                        else
                        {
                            suite.Result = Const.Result.NotPass;
                            FORM.Dispatcher.Invoke(HIGH, new Action(delegate () { StyleControl(suite.suiteInCycleControl.Name, "Background", ORANGE); }));
                        }
                    }
                }

                if (cycle.AllSuitesFinished())
                {
                    cycle.Status = Const.Status.Finished;
                    if (cycle.AllSuitesPass())
                    {
                        cycle.Result = Const.Result.Pass;
                        FORM.Dispatcher.Invoke(HIGH, new Action(delegate () { StyleControl(cycle.CycleUiObject.Name, "Background", GREEN); }));
                    }
                    else
                    {
                        cycle.Result = Const.Result.NotPass;
                        FORM.Dispatcher.Invoke(HIGH, new Action(delegate () { StyleControl(cycle.CycleUiObject.Name, "Background", ORANGE); }));
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void Start_Test(Cycle cycle, Suite suite, Test test)
        {
            if (test != null)
            {
                FORM.Dispatcher.Invoke(HIGH, new Action(delegate () { StyleControl(test.testInSuiteInQueueControl.Name, "Background", LBLUE); }));

                test.Status = Const.Status.Running;
                string taskName = "CYCLE_" + cycle.ID + "_SUITE_" + StringHandlers.CleanName(suite.Name) + "_TEST_" + test.ID;
                test.logFilePath = Path.Combine(Path.GetTempPath(), "FWR", taskName + "-" + StringHandlers.ShortDateTimeString(DateTime.Now) + ".log");
                Log testLog = new Log(test.logFilePath);
                testLog.Info("starting test:" + taskName);

                string exeName = "powershell.exe";

                var procInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy unrestricted \"{test.ScriptPath}\" 2> {test.logFilePath + ".err"} -wait | Tee-Object -file {testLog.GetLogFilePath()}",
                };

                procInfo.WorkingDirectory = Path.GetDirectoryName(exeName);
                procInfo.WindowStyle = ProcessWindowStyle.Normal;

                Process _childp = Process.Start(procInfo);
                _childp.EnableRaisingEvents = true;
                _childp.Exited += (sender, e) => { End_Test(test); };
                test.ShellProcess = _childp;

                FORM.Dispatcher.Invoke(IDLE, new Action(delegate ()
                {
                    test.testInSuiteInQueueControl.exeWindowGrid.Children.Add(ChildWindowHandler.SetProcessAsChildOfPanelControl(_childp, 800, 400, test));
                }));
            }
        }

        private void End_Test(Test test)
        {
            if (test.ShellProcess.ExitCode == 0)
            {
                FORM.Dispatcher.Invoke(IDLE, new Action(delegate () { StyleControl(test.testInSuiteInQueueControl.Name, "Background", GREEN); }));

                test.Result = Const.Result.Pass;

                FORM.Dispatcher.Invoke(HIGH, new Action(delegate () { StyleControl(test.testInSuiteInQueueControl.Name, "Background", GREEN); }));
            }
            else
            {
                test.Result = Const.Result.Fail;

                FORM.Dispatcher.Invoke(IDLE, new Action(delegate () { StyleControl(test.testInSuiteInQueueControl.Name, "Background", RED); }));

                using (var reader = new StreamReader(test.logFilePath + ".err"))
                {
                    string error1 = reader.ReadLine();
                    try
                    {
                        test.Error = reader.ReadLine().Split(':')[1];
                    }
                    catch
                    {
                        test.Error = error1;
                    }
                }

                FORM.Dispatcher.Invoke(HIGH, new Action(delegate (){StyleControl(test.testInSuiteInQueueControl.Name, "Background", RED);}));
            }

            test.ShellProcess.Exited -= (sender, e) => { End_Test(test); };
            test.Status = Const.Status.Finished;
            test.ShellProcess.Close();
            test.ShellProcess = null;

            FORM.Dispatcher.Invoke(IDLE, new Action(delegate ()
            {
                test.testInSuiteInQueueControl.exeWindowGrid.Children.Remove(test.WindowsFormsHostControl);
                test.testInSuiteInQueueControl.expander.IsExpanded = false;
                test.testInSuiteInQueueControl.Height = 30;
            }));

            test.WindowsFormsHostControl = null;
        }

        private void StyleControl(string controlName, string controlPropertyName, Object controlPropertyToSet)
        {
            FORM.InvalidateVisual();

            FORM.Dispatcher.Invoke(IDLE, new Action(() =>
            {
                var obj = ObjectsHandlers.FindChildObjectInObject<Control>(FORM).First(x => x.Name == controlName);
                PropertyInfo pInfo = obj.GetType().GetProperty(controlPropertyName);
                TypeConverter tc = TypeDescriptor.GetConverter(pInfo.PropertyType);
                pInfo.SetValue(obj, controlPropertyToSet);
            }));
        }
    }
}



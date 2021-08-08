using FWR.UI_Aux;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FWR.Engine
{
    public class MainEngine
    {
        private Thread MAINTHRD;
        private System.Windows.Threading.DispatcherPriority NRML = System.Windows.Threading.DispatcherPriority.Normal;
        private Log log;
        MainWindow FORM;

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
                foreach (Cycle cycle in Runtime.queue.Cycles)
                {
                    if (cycle.Active && Startable.Contains(cycle.Status))
                    {
                        cycle.Status = Const.Status.Running;

                        int ID = cycle.ID;
                        string controlName = Const.cycleObject + ID;

                        FORM.Dispatcher.Invoke(NRML, new Action(delegate ()
                        {
                            FORM.AddToTodoList(controlName, "Background", new SolidColorBrush(Colors.CornflowerBlue));
                        }));

                        foreach (Suite suite in cycle.Suites ?? new List<Suite>())
                            if (Startable.Contains(suite.Status))
                                suite.Status = Const.Status.Running;

                        cycle.CycleWorker = new Thread(() => CycleWorkerThread(cycle));
                        cycle.CycleWorker.Name = cycle.Name + "_RunThread";
                        cycle.CycleWorker.Start();
                    }

                    Thread.Sleep(1000);
                }
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
                }
                Thread.Sleep(1000);
            }
        }

        private void Start_Test(Cycle cycle, Suite suite, Test test)
        {
            if (test != null)
            {
                test.Status = Const.Status.Running;
                string taskName = "CYCLE_" + cycle.ID + "_SUITE_" + StringHandlers.CleanName(suite.Name) + "_TEST_" + test.ID;
                string logFileName = taskName + "-" + StringHandlers.ShortDateTimeString(DateTime.Now) + ".log";
                Log testLog = new Log(Path.Combine(Path.GetTempPath(), "FWR", logFileName));
                testLog.Info("starting test:" + taskName);

                string exeName = "powershell.exe";

                var procInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy unrestricted \"{test.ScriptPath}\"  | Tee-Object -file {testLog.GetLogFilePath()}",
                };

                procInfo.WorkingDirectory = Path.GetDirectoryName(exeName);
                procInfo.WindowStyle = ProcessWindowStyle.Normal;
                Process _childp = Process.Start(procInfo);
                _childp.EnableRaisingEvents = true;
                _childp.Exited += (sender, e) => { End_Test(test); };
                test.ShellProcess = _childp;

                FORM.Dispatcher.Invoke(NRML, new Action(delegate (){
                     test.testInSuiteInQueueControl.exeWindowGrid.Children.Add(ChildWindowHandler.SetProcessAsChildOfPanelControl(_childp, 800, 400, test));
                }));

            }
        }

        private void End_Test(Test test)
        {
            test.ShellProcess.Exited -= (sender, e) => { End_Test(test); };
            test.Status = Const.Status.Finished;
            test.ShellProcess.Close();
            test.ShellProcess = null;

            FORM.Dispatcher.Invoke(NRML, new Action(delegate (){
                test.testInSuiteInQueueControl.exeWindowGrid.Children.Remove(test.WindowsFormsHostControl);
                test.testInSuiteInQueueControl.expander.IsExpanded = false;
                test.testInSuiteInQueueControl.Height = 30;
            }));

            test.WindowsFormsHostControl = null;
        }
    }
}



using FWR.Engine;
using FWR.UI_Aux;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FWR
{
    public partial class MainWindow : Window
    {
        Color color = new Color();

        void UiUpdateRunner(object sender, EventArgs e)
        {
            timesUpdated++;
            if (timesUpdated % 20 == 0)
                MarkAllNeedUpdate();

            StatusLabel.Content = Runtime.status.ToString();
            if (!StatusLabel.Content.ToString().ToUpper().Contains("RUNNING"))
                StatusLabel.Foreground = new SolidColorBrush(Colors.Black);

            if (Runtime.status == Const.Status.Running || needLastUiUpdate)
            {

                totalSecondsRunning++;
                TimeLabel.Content = StringHandlers.IntSecondsToHhMmSsString(totalSecondsRunning);

                foreach (Cycle cycle in Runtime.queue.Cycles)
                {
                    int ID = cycle.ID;
                    Label timeCaptionLabelObj = ObjectsHandlers.FindChildObjectInObject<Label>(this).First(x => x.Name == Const.cycleTimeCaptionLabel + ID);
                    timeCaptionLabelObj.Content = StringHandlers.IntSecondsToHhMmSsString(cycle.TotalSecondsRunning);

                    if (cycle.needUiUpdate)
                        UpdateCycleUi(cycle);

                    foreach (Suite suite in cycle.Suites)
                    {
                        if (suite.needUiUpdate)
                            UpdateSuiteUi(suite);

                        foreach (Test test in suite.Tests)
                            if (test.needUiUpdate)
                                UpdateTestUi(test);
                    }
                }

                if (Runtime.status != Const.Status.Running)
                    needLastUiUpdate = false;
            }
        }

        private void MarkAllNeedUpdate()
        {
            foreach (Cycle cycle in Runtime.queue.Cycles)
                foreach (Suite suite in cycle.Suites)
                    foreach (Test test in suite.Tests)
                        test.SetUiNeedUpdate();
        }

        public void UpdateCycleUi(Cycle cycle)
        {
            switch (cycle.Status)
            {
                case Const.Status.Running:
                    color = Colors.CornflowerBlue;
                    break;
                case Const.Status.Finished:
                    if (cycle.Result == Const.Result.Pass)
                        color = Colors.Green;
                    else
                        color = Colors.Red;
                    break;
                default:
                    return;
            }

            StyleControl(cycle.CycleUiObject, "Background", new SolidColorBrush(color));
        }

        public void UpdateSuiteUi(Suite suite)
        {
            switch (suite.Status)
            {
                case Const.Status.Running:
                    color = Colors.CornflowerBlue;
                    break;
                case Const.Status.Finished:
                    if (suite.Result == Const.Result.Pass)
                        color = Colors.Green;
                    else
                        color = Colors.Red;
                    break;
                default:
                    return;
            }

            StyleControl(suite.suiteInCycleControl, "Background", new SolidColorBrush(color));
        }

        public void UpdateTestUi(Test test)
        {
            switch (test.Status)
            {
                case Const.Status.Running:
                    color = Colors.CornflowerBlue;
                    break;
                case Const.Status.Finished:
                    if (test.Result == Const.Result.Pass)
                        color = Colors.Green;
                    else
                        color = Colors.Red;
                    break;
                default:
                    return;
            }

            StyleControl(test.testInSuiteInQueueControl.Name, "Background", new SolidColorBrush(color));
        }
    }
}

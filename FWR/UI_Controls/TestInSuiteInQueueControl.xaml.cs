using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System;
using System.Windows.Forms.Integration;
using FWR.Engine;

namespace FWR.UI_Controls
{
    /// <summary>
    /// Interaction logic for TestInSuiteInQueueControl.xaml
    /// </summary>
    public partial class TestInSuiteInQueueControl : UserControl
    {
        private Test _test;
        private Suite _suite;

        public TestInSuiteInQueueControl(Suite suite, Test test)
        {
            _test = test;
            _suite = suite;
            InitializeComponent();
        }

        public Suite GetSuite()
        {
            return _suite;
        }

        public Test GetTest()
        {
            return _test;
        }

        private void Expander_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (exeWindowGrid.IsVisible)
            {
                exeWindowGrid.Visibility = Visibility.Collapsed;
                this.Height = 30;
            }
            else
            {
                exeWindowGrid.Visibility = Visibility.Visible;
                this.Height = 60;
            }
        }

        private void Test_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Click_Log(sender, e);
        }

        private void Click_Log(object sender, RoutedEventArgs e)
        {
            string exeName = "powershell.exe";
            var procInfo = new System.Diagnostics.ProcessStartInfo(exeName);
            procInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(exeName);
            procInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process _childp = Process.Start(procInfo);

            exeWindowGrid.Children.Add(UI_Aux.ChildWindowHandler.SetProcessAsChildOfPanelControl(_childp, 800, 400));
        }

        private void Checkbox_Clicked(object sender, RoutedEventArgs e)
        {
            _test.Selected = (bool)selectedCheckbox.IsChecked ;
        }
    }
}

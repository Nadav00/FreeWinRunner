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
using FWR.UI_Aux;

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
            _test.ID = new Random().Next(0, int.MaxValue);
            Thread.Sleep(10);
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
                if (_test.ShellProcess != null)
                    ChildWindowHandler.MakeExternalWindowHiddenOrNot(_test.ShellProcess, true);

            }
            else
            {
                exeWindowGrid.Visibility = Visibility.Visible;
                if (_test.ShellProcess != null)
                {
                    this.Height = 430;
                    ChildWindowHandler.MakeExternalWindowHiddenOrNot(_test.ShellProcess, false);
                }
                else
                    this.Height = 60;
            }
        }

        private void Test_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Click_Log(sender, e);
        }

        private void Click_Log(object sender, RoutedEventArgs e)
        {

        }

        private void Checkbox_Clicked(object sender, RoutedEventArgs e)
        {
            _test.Selected = (bool)selectedCheckbox.IsChecked ;
        }
    }
}

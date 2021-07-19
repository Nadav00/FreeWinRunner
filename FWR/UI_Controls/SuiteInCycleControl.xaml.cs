using FWR.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FWR.UI_Controls
{
    /// <summary>
    /// Interaction logic for SuiteInCycleControl.xaml
    /// </summary>
    public partial class SuiteInCycleControl : UserControl
    {
        private bool _locked = false;
        private Suite _suite;
        private Cycle _cycle;

        public SuiteInCycleControl(Suite suite, Cycle cycle)
        {
            _suite = suite;
            _cycle = cycle;
            InitializeComponent();
        }

        public Suite GetSuite()
        {
            return _suite;
        }

        public Cycle GetCycle()
        {
            return _cycle;
        }

        private void Expander_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (testsPanel.IsVisible)
                testsPanel.Visibility = Visibility.Collapsed;
            else
                testsPanel.Visibility = Visibility.Visible;
        }

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
        }

        public bool IsLocked()
        {
            return _locked;
        }
    }
}

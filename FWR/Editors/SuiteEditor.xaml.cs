using FWR.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using FWR.UI_Aux;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using Newtonsoft.Json;

namespace FWR.Editors
{
    /// <summary>
    /// Interaction logic for NewSuite.xaml
    /// </summary>
    public partial class SuiteEdit : Window
    {
        public enum EditState 
        {
            New,
            Edit,
            Browse,
            Delete
        }

        private Cycle _cycle;
        private Suite _suite;
        public ObservableCollection<Test> testList { get; set; }
        private string _folder;
        private EditState _editstate;
   
        public SuiteEdit(ref Cycle cycle, ref Suite suite, EditState editState)
        {
            _cycle = cycle;
            _suite = suite;
            _editstate = editState;

            InitializeComponent();
            this.DataContext = this;
            InitalizeWindow();
            InitializeObjects();

            this.testsGrid.DataContext = testList;
            this.testsGrid.ItemsSource = testList;
        }

        private void InitializeObjects()
        {
            testList = new ObservableCollection<Test>();
        }

        void InitalizeWindow()
        {
            CycleName.Content = _cycle.Name;
            string unescapedFolder = StringHandlers.Unescape(Runtime.config.MAIN_DIR);
            folderTextbox.Text = System.IO.Path.Combine(unescapedFolder, Const.projectSubfolder);
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            _folder = FileSystemPickers.FolderPicker(Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.projectSubfolder));
            folderTextbox.Text = _folder.Replace(StringHandlers.Unescape(Runtime.config.MAIN_DIR), ""); ;
        }

        private void AddTestButton_Click(object sender, RoutedEventArgs e)
        {
            Test newTest = new Test();
            newTest.Status = Const.Status.Empty;
            AddTest myAddTest = new AddTest(ref newTest);

            myAddTest.ShowDialog();

            if (newTest?.Status == Const.Status.New)
                testList.Add(newTest);

            myAddTest = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _suite.Status = Const.Status.New;
            _suite.Name = nameTextBox.Text;

            string suiteFilePath = Path.Combine(folderTextbox.Text, nameTextBox.Text);
            suiteFilePath += ".json";
            _suite.SuiteFilePath = suiteFilePath;

            _suite.Tests = new List<Test>();
            foreach (Test t in testList)
                _suite.Tests.Add(t);

            if (_editstate == EditState.New)
            {
                string json = JsonConvert.SerializeObject(_suite, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(suiteFilePath, json);
            }

            Close();
        }
    }
}

using FWR.Auxilary;
using FWR.Database;
using FWR.Engine;
using FWR.UI_Aux;
using FWR.UI_Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace FWR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ListView QueueObj;
        public int latestCycle = 0;
        private int totalSecondsRunning;
        private List<UiTodoListItem> uiTodoList = new List<UiTodoListItem>();
        private string logPath = Path.Combine(Path.GetTempPath(),"FWR");
        private Log log = new Log(Path.Combine(Path.GetTempPath(), "FWR", "MainWindow.log"));


        public class UiTodoListItem
        {
            public string ControlName;
            public string ControlPropertyName;
            public Object ControlPropertyToSet;
        }

        public MainWindow()
        {
            InitializeComponent();
            FWR.UI_Aux.Config.Load_Config();
            InitializeObjects();
            log.Info("Started Main Window");
            NameTextBox.Text = Namer.MakeName(2);
            log.Info("Granted Name");
            Add_Timer();
            log.Info("Added timer");
            log.Info("--------------------------");

        }

        private void InitializeObjects()
        {
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            Runtime.resources = Engine.Resources.LoadResources.PerformLoadResources();
            Runtime.SetMainWindow(this);
        }

        private void Add_Timer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new System.EventHandler(UiUpdateRunner);
            timer.Start();
        }

        void UiUpdateRunner(object sender, EventArgs e)
        {
            if (Runtime.status == Const.Status.Running)
            {
                totalSecondsRunning++;
                TimeLabel.Content = StringHandlers.IntSecondsToHhMmSsString(totalSecondsRunning);
                foreach (Cycle cycle in Runtime.queue.Cycles)
                {
                    int ID = cycle.ID;
                    Label timeCaptionLabelObj = ObjectsHandlers.FindChildObjectInObject<Label>(this).First(x => x.Name == Const.cycleTimeCaptionLabel + ID);
                    timeCaptionLabelObj.Content = StringHandlers.IntSecondsToHhMmSsString(cycle.TotalSecondsRunning);
                }

                ProcessUiTodoList();



            }
        }

        private void ProcessUiTodoList()
        {
            var listCopy = uiTodoList.ConvertAll(x => x).ToList();
            uiTodoList = new List<UiTodoListItem>();

            foreach (var item in listCopy)
            {
                var obj = ObjectsHandlers.FindChildObjectInObject<Control>(this).First(x => x.Name == item.ControlName);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    PropertyInfo pInfo = obj.GetType().GetProperty(item.ControlPropertyName);
                    TypeConverter tc = TypeDescriptor.GetConverter(pInfo.PropertyType);
                    pInfo.SetValue(obj, item.ControlPropertyToSet);
                }));

            }

            listCopy = null;
        }

        static Object GetProperty(object control, string propertyName)
        {
            var controlType = control.GetType();
            var property = controlType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
                throw new InvalidOperationException(string.Format("Property “{0}” does not exist in type “{1}”.", propertyName, controlType.FullName));
            return property;
        }

        private void Edit_Config(object sender, RoutedEventArgs e)
        {
            UI_Aux.Config.Edit_Config();
        }

        private void Add_Cycle(object sender, RoutedEventArgs e)
        {
            var newCycle = new Cycle("New Cycle");
            Add_Cycle_Perform(newCycle, sender, e);
        }

        private void Save_Queue(object sender, RoutedEventArgs e)
        {
            string _cycleJsonFilePath = FileSystemPickers.FilePicker(System.IO.Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.projectSubfolder), "json");
            Runtime.queue.Name = NameTextBox.Text;
            string json = JsonConvert.SerializeObject(Runtime.queue, Newtonsoft.Json.Formatting.Indented);

            if (_cycleJsonFilePath?.Length > 0)
                System.IO.File.WriteAllText(_cycleJsonFilePath, json);
        }

        private void Load_Queue(object sender, RoutedEventArgs e)
        {
            string _cycleJsonFilePath = FileSystemPickers.FilePicker(System.IO.Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.projectSubfolder), "json");
            Queue queue = JsonConvert.DeserializeObject<Queue>(System.IO.File.ReadAllText(_cycleJsonFilePath));
            NameTextBox.Text = queue.Name;
            foreach (Cycle cycle in queue.Cycles)
            {
                Add_Cycle_Perform(cycle, sender, e);

                cycle.Suites = cycle.Suites ?? new List<Suite>();

                List<Suite> suitesToAdd = new List<Suite>();

                foreach (Suite suite in cycle.Suites)
                    suitesToAdd.Add(suite);

                foreach (Suite suite in suitesToAdd)
                    AddSuiteToCycleUiObject(cycle, suite);
            }
        }

        private void Add_Cycle_Perform(Cycle cycle, object sender, RoutedEventArgs e)
        {
            latestCycle++;

            if (Runtime.queue.Cycles == null)
                Runtime.queue.Cycles = new List<Cycle>();
            cycle.ID = latestCycle;
            cycle.Active = true;
            Runtime.queue.Cycles.Add(cycle);

            QueueObj = (ListView)FindName("Queue");

            // === Cycle UI control ==================================
            ListView cycleObject = new ListView();
            cycleObject.Name = Const.cycleObject + latestCycle;
            var zeroThickns = new Thickness(0, 0, 0, 0);
            cycleObject.BorderThickness = zeroThickns;
            // --- Header  -------------------------------------------
            WrapPanel header = new WrapPanel();
            header.Height = 30;
            // Expander
            Expander cycleExpander = new Expander();
            cycleExpander.Name = Const.cycleExpander + latestCycle;
            cycleExpander.Expanded += CycleExpanderChange;
            cycleExpander.Collapsed += CycleExpanderChange;
            // Active-Checkbox
            CheckBox activeCheckBox = new CheckBox();
            activeCheckBox.IsChecked = true;
            activeCheckBox.Name = Const.cycleActiveCheckbox + latestCycle;
            activeCheckBox.Click += CycleActiveCheckboxClicked;
            // Name
            RichTextBox nameBox = new RichTextBox();
            nameBox.Name = Const.cycleNamePrefix + latestCycle;
            nameBox.Width = 120;
            nameBox.AppendText(cycle.Name);
            nameBox.TextChanged += CycleNameChanged;
            // Add-suite button
            Button addSuiteButton = new Button();
            addSuiteButton.Name = Const.addSuiteButton + latestCycle;
            addSuiteButton.Content = "Add Suite";
            addSuiteButton.Click += AddSuiteClicked;
            // New-suite button
            Button newSuiteButton = new Button();
            newSuiteButton.Name = Const.newSuiteButton + latestCycle;
            newSuiteButton.Content = "New Suite";
            newSuiteButton.Click += NewSuiteClicked;

            // TODO NADAV HERE

            // Cycle Status Label
            // Cycle Status Caption

            // Cycle Time Label
            Label timeLabelLabel = new Label();
            timeLabelLabel.Name = Const.cycleTimeLabelLabel + latestCycle;
            timeLabelLabel.Content = "tot.Run:";

            // Cycle Time Caption
            Label timeCaptionLabel = new Label();
            timeCaptionLabel.Name = Const.cycleTimeCaptionLabel + latestCycle;
            timeCaptionLabel.Content = "00:00:00";

            // OpenEditLog Button

            // Stop Button

            // Remove Button

            //void OnFormClosing(object sender, FormClosingEventArgs e)
            //{
            //foreach (Delegate d in FindClicked.GetInvocationList())
            //{
            //FindClicked -= (FindClickedHandler)d;
            //}
            //}

            // Add object to Header -----------------------------------
            header.Children.Add(cycleExpander);
            header.Children.Add(activeCheckBox);
            header.Children.Add(nameBox);
            header.Children.Add(timeLabelLabel);
            header.Children.Add(timeCaptionLabel);
            header.Children.Add(addSuiteButton);
            header.Children.Add(newSuiteButton);
            // --- Body -----------------------------------------------
            var cycleBody = new StackPanel();
            cycleBody.Name = Const.cycleBody + latestCycle;
            // ===== Add objects to cycle object ======================
            cycleObject.Items.Add(header);
            cycleObject.Items.Add(cycleBody);
            // ========================================================

            QueueObj.Items.Add(cycleObject);
        }

        private void CycleActiveCheckboxClicked(object sender, RoutedEventArgs e)
        {
            CheckBox _sender = (sender as CheckBox);
            var name = _sender.Name;
            var ID = int.Parse(name.Replace(Const.cycleActiveCheckbox, string.Empty));
            Cycle cycle = Runtime.queue.Cycles.Find(c => c.ID == ID);

            cycle.Active = (bool)_sender?.IsChecked;
        }

        private void CycleExpanderChange(object sender, RoutedEventArgs e)
        {
            Control expanderControl = (e.Source as Expander);
            var name = expanderControl.Name;
            var ID = int.Parse(name.Replace(Const.cycleExpander, string.Empty));
            var cycleBodyObj = ObjectsHandlers.FindChildObjectInObject<StackPanel>(this).First(x => x.Name == Const.cycleBody + ID);
            if (cycleBodyObj.IsVisible)
                cycleBodyObj.Visibility = Visibility.Collapsed;
            else
                cycleBodyObj.Visibility = Visibility.Visible;
        }

        private void AddSuiteClicked(object sender, RoutedEventArgs e)
        {
            Button _sender = (sender as Button);
            var name = _sender.Name;
            var ID = int.Parse(name.Replace(Const.addSuiteButton, string.Empty));
            Cycle cycle = Runtime.queue.Cycles.Find(c => c.ID == ID);

            string _suiteJsonFilePath = FileSystemPickers.FilePicker(System.IO.Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.projectSubfolder), "json");

            if (_suiteJsonFilePath?.Length > 1)
            {
                Suite suite = JsonConvert.DeserializeObject<Suite>(System.IO.File.ReadAllText(_suiteJsonFilePath));

                if (suite.Status == Const.Status.New)
                {
                    cycle.Suites = cycle.Suites ?? new List<Suite>();
                    cycle.Suites.Add(suite);
                    AddSuiteToCycleUiObject(cycle, suite);
                }
            }
        }

        private void NewSuiteClicked(object sender, RoutedEventArgs e)
        {
            Button _sender = (sender as Button);
            var name = _sender.Name;
            var ID = int.Parse(name.Replace(Const.newSuiteButton, string.Empty));
            Cycle cycle = Runtime.queue.Cycles.Find(c => c.ID == ID);
            Suite suite = new Suite();
            FWR.Editors.SuiteEdit newSuiteWindow = new Editors.SuiteEdit(ref cycle, ref suite, FWR.Editors.SuiteEdit.EditState.New);
            newSuiteWindow.ShowDialog();
            if (suite.Status == Const.Status.New)
            {
                cycle.Suites = cycle.Suites ?? new List<Suite>();
                cycle.Suites.Add(suite);
                AddSuiteToCycleUiObject(cycle, suite);
            }

            newSuiteWindow = null;
        }

        private void AddSuiteToCycleUiObject(Cycle cycle, Suite suite)
        {
            string cycleBodyName = Const.cycleBody + cycle.ID;
            StackPanel cycleBodyObj = ObjectsHandlers.FindChildObjectInObject<StackPanel>(this).First(x => x.Name == Const.cycleBody + cycle.ID);

            var newSuiteObject = new UI_Controls.SuiteInCycleControl(suite, cycle);
            newSuiteObject.Name = StringHandlers.CleanName(Const.cycleBodySuite + cycle.Name + suite.Name);
            newSuiteObject.nameLabel.Content = suite.Name;
            cycleBodyObj.Children.Add(newSuiteObject);

            foreach (Test test in suite.Tests)
            {
                AddTestToCycleSuiteUiObject(newSuiteObject, test);
            }
        }

        public void UpdateSuiteUI(Suite suite, Cycle cycle)
        {
            string controlName = UI_Aux.StringHandlers.CleanName(Const.cycleBodySuite + cycle.Name + suite.Name);
            SuiteInCycleControl cycleBodyObj = ObjectsHandlers.FindChildObjectInObject<SuiteInCycleControl>(this).First(x => x.Name == controlName);
        }

        private void AddTestToCycleSuiteUiObject(UI_Controls.SuiteInCycleControl suiteObject, Test test)
        {
            var newTestObject = new TestInSuiteInQueueControl(suiteObject.GetSuite(), test);
            newTestObject.Name = StringHandlers.CleanName(Const.TestInSuiteUiObj + test.ID);
            newTestObject.nameLabel.Content = test.Name;
            newTestObject.selectedCheckbox.IsChecked = test.Selected;
            newTestObject.exeWindowGrid.Visibility = Visibility.Collapsed;
            test.testInSuiteInQueueControl = newTestObject;
            suiteObject.testsPanel.Children.Add(newTestObject);
        }

        private void CycleNameChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            RichTextBox _sender = (sender as RichTextBox);
            var name = _sender.Name;
            var ID = name.Replace(Const.cycleNamePrefix, string.Empty);
            int ID_Int = int.Parse(ID);
            Cycle element = Runtime.queue.Cycles.Find(e => e.ID == ID_Int);
            element.Name = StringHandlers.CleanName((new TextRange(_sender.Document.ContentStart, _sender.Document.ContentEnd).Text).ToString());
        }

        private void Click_Run(object sender, RoutedEventArgs e)
        {
            Runtime.engine.StartRun();
        }

        private void Click_Stop(object sender, RoutedEventArgs e)
        {
            Runtime.engine.StopRun();
        }

        private void Delete_All(object sender, RoutedEventArgs e)
        {
            QueueObj = (ListView)FindName("Queue");
            QueueObj.Items.Clear();
            Runtime.queue = new Queue();
            Runtime.queue.Cycles = new List<Cycle>();

        }

        private void View_DB(object sender, RoutedEventArgs e)
        {
            DBViewer myDb = new DBViewer();
            myDb.Show();
        }


        public void AddToTodoList(string controlName, string propertyName, object valuePropertyToSet)
        {
            uiTodoList.Add(new UiTodoListItem(){ControlName = controlName, ControlPropertyName = propertyName, ControlPropertyToSet = valuePropertyToSet});
        }
    }
}

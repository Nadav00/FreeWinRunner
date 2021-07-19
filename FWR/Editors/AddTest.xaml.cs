using FWR.Engine;
using FWR.UI_Aux;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace FWR.Editors
{
    /// <summary>
    /// Interaction logic for AddTest.xaml
    /// </summary>
    public partial class AddTest : Window
    {
        public Test returnTest { get; set; }
        public List<Resource> resourceList { get; set; }
        private ObservableCollection<Resource> ViewedResourcesList { get; set; }
        private string initialSearchFilterText;
        private string _scriptPath;
        private Test _test;

        public AddTest(ref Test newTest)
        {
            _test = newTest;
            MainConstructor();
        }

        public AddTest()
        {
            MainConstructor();
        }

        private void MainConstructor()
        {
            InitializeComponent();
            this.DataContext = this;
            InitializeObjects();
        }

        private void InitializeObjects()
        {
            initialSearchFilterText = this.searchResourceTextbox.Text;
            resourceList = new List<Resource>();
            ViewedResourcesList = new ObservableCollection<Resource>();
            PopulateResources(false);
            this.resourcesGrid.DataContext = ViewedResourcesList;
            this.resourcesGrid.ItemsSource = ViewedResourcesList;

            string unescapedFolder = StringHandlers.Unescape(Runtime.config.MAIN_DIR);
            pathTextbox.Text = Path.Combine(unescapedFolder, Const.projectSubfolder);
        }

        private void PopulateResources(bool isViewOnly = false, string stringToContain = "")
        {
            ViewedResourcesList = new ObservableCollection<Resource>();

            foreach (var item in Runtime.resources)
            {
                Resource newItem = item;
                newItem.ResourceJsonFilePath = newItem.ResourceJsonFilePath.Replace(Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.EnvironmentSubfolder), "");
                Resource CloneItem = UI_Aux.ObjectsHandlers.Clone<Resource>(newItem);

                if (!isViewOnly)
                {
                    resourceList.Add(CloneItem);
                    ViewedResourcesList.Add(CloneItem);
                }
                else if (initialSearchFilterText != null && !(searchResourceTextbox.Text == initialSearchFilterText ))
                {
                    item.NickName = item.NickName ??  "";
                    if (item.ResourceJsonFilePath.Contains(stringToContain) || item.NickName.Contains(stringToContain))
                        ViewedResourcesList.Add(CloneItem);
                }
            }
        }

        private void RowClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                                                    e.OriginalSource as DependencyObject) as DataGridRow;
                if (row == null) return;

                var currentItem = row.Item as Resource;

                var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
                DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);
                while (cell != null && !(cell is System.Windows.Controls.DataGridCell)) cell = VisualTreeHelper.GetParent(cell);
                System.Windows.Controls.DataGridCell targetCell = cell as System.Windows.Controls.DataGridCell;
                var Header = targetCell.Column.Header;

                if (Header as string == "Register")
                {
                    currentItem.Registered ^= true;
                    if (!currentItem.Registered)
                        currentItem.Locked = false;
                }
                else if (Header as string == "Lock")
                {
                    currentItem.Locked ^= true;
                    if (currentItem.Locked)
                        currentItem.Registered = true;

                }

                var resource = resourceList.Where(x => x.ResourceJsonFilePath.Contains(currentItem.ResourceJsonFilePath)).FirstOrDefault();

                resource = currentItem;

                resourcesGrid.Items.Refresh();
            }
            catch
            {
                Console.WriteLine("Test Resources RowClick() - Not inside a value cell");
            }
        }

        private void ReloadResources_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchResourceTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchResourceTextbox.Text != initialSearchFilterText)
                PopulateResources(true, searchResourceTextbox.Text);

            this.resourcesGrid.ItemsSource = null;
            this.resourcesGrid.ItemsSource = ViewedResourcesList;
            resourcesGrid.Items.Refresh();
        }

        private void SearchResourceTextbox_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (searchResourceTextbox.Text == initialSearchFilterText)
                searchResourceTextbox.Text = "";
        }

        private void BrowseScriptPath_Click(object sender, RoutedEventArgs e)
        {
            _scriptPath = FileSystemPickers.FilePicker(Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.projectSubfolder), "ps1");
            pathTextbox.Text = _scriptPath?.Replace(StringHandlers.Unescape(Runtime.config.MAIN_DIR), ".");
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            _scriptPath = FileSystemPickers.FolderPicker(Path.Combine(StringHandlers.Unescape(Runtime.config.MAIN_DIR), Const.projectSubfolder));
            folderTextbox.Text = _scriptPath.Replace(StringHandlers.Unescape(Runtime.config.MAIN_DIR), ".");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _test.Status = Const.Status.New;
            _test.Name = nameTextBox.Text;
            _test.ScriptPath = _scriptPath;
            _test.Resources =  ViewedResourcesList.Where(x => x.Registered == true).ToList();

            this.Close();
        }
    }
}

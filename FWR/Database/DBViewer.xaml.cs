using System.Windows;
using SQLDatabase.Net.SQLDatabaseClient;


namespace FWR.Database
{
    /// <summary>
    /// Interaction logic for DBViewer.xaml
    /// </summary>
    public partial class DBViewer : Window
    {
        public DBViewer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Database myDB = new Database("/FreeWinRunner/FWR/FWR.db");
        }
    }
}

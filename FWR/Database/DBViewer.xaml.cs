using System.Windows;
using SQLDatabase.Net.SQLDatabaseClient;

namespace FWR.Database
{
    public partial class DBViewer : Window
    {
        private Database myDB;

        public DBViewer()
        {
            InitializeComponent();

            var result =  Database.Instance.ExecuteToDB($"Select * FROM {new Tables.TestResults().TableName}");
                        
        }
    }
}

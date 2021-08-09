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
            myDB = new Database("/FreeWinRunner/FWR/FWR.db");
        }
    }
}

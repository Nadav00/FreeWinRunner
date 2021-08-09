using SQLDatabase.Net.SQLDatabaseClient;
using System;
using System.Data;

namespace FWR.Database
{
    public class Database
    {
        private string _filePath;
        private SqlDatabaseConnection cnn;

        public Database(string filePath)
        {
            _filePath = filePath;
            cnn = new SqlDatabaseConnection();
            cnn.ConnectionString = $"SchemaName=db;uri=file://{_filePath}";
            cnn.Open();

            if (!TableExists("TestResults"))
            {
                if (cnn.State == ConnectionState.Open)
                {
                    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand())
                    {
                        cmd.Connection = cnn;
                        cmd.CommandText = CreateTableString("TestResults", new Tables.TestResults()); 
                        cmd.ExecuteReader();
                    }
                }
            }
        }

        public bool TableExists(string tableName)
        {
            try
            {
                cnn.ConnectionString = $"SchemaName=db;uri=file://{_filePath}";
                cnn.Open();

                if (cnn.State == ConnectionState.Open)
                {
                    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand())

                    {
                        cmd.Connection = cnn;
                        cmd.CommandText = "Select * From TestResults";
                        cmd.ExecuteReader();
                        return true;
                    }
                }
            }
            catch { return false; }

            return false;
        }

        public string CreateTableString(string tableName, Tables.Table table)
        {
            string strng = "CREATE TABLE " + tableName + " (";

            foreach (var column in table.Columns)
                strng += column.Key + " " + column.Value + ",";

            strng = strng.Substring(0, strng.Length - 1) + ")";

            return strng;
        }
    }
}

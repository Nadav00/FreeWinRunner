using SQLDatabase.Net.SQLDatabaseClient;
<<<<<<< HEAD
using System;
using System.Data;
=======
>>>>>>> 32bc39bf5187e0d025759231a6953d4239efa84b

namespace FWR.Database
{
    public class Database
    {
        private string _filePath;
<<<<<<< HEAD
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
                        cmd.CommandText = "CREATE TABLE TestResults (ID BIGINT IDENTITY(1,1) , Name varchar(255));"; 
                        cmd.ExecuteReader();
                    }
                }
            }
        }

        public bool TableExists(string tableName)
        {
            try
            {
                using (SqlDatabaseConnection cnn = new SqlDatabaseConnection())
                {
                    cnn.ConnectionString = $"SchemaName=db;uri=file://{_filePath}";
                    cnn.Open();

                    if (cnn.State == ConnectionState.Open)
                    {
                        using (SqlDatabaseCommand cmd = new SqlDatabaseCommand())

                        {
                            cmd.Connection = cnn;

                            cmd.CommandText = "Select * From TestResults";

                            //cmd.CommandText = "SELECT * FROM Table";
                            cmd.ExecuteReader();
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
=======

        public Database(string filePath)
        {
            _filePath
        }


>>>>>>> 32bc39bf5187e0d025759231a6953d4239efa84b


    }
}

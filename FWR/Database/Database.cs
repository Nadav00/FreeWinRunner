using FWR.Engine;
using FWR.UI_Aux;
using SQLDatabase.Net.SQLDatabaseClient;
using System;
using System.Data;
using System.IO;

namespace FWR.Database
{

    public class Database
    {
        private string _filePath;
        private SqlDatabaseConnection cnn = new SqlDatabaseConnection();

        private static readonly Lazy<Database>
        lazy = new Lazy<Database>(() => new Database());

        public static Database Instance { get { return lazy.Value; } }
        private Database instance;

        private string identityColumnSubstring = "IDENTITY";
        private string charTypeSubstring = "CHAR";

        private Database()
        {
            string filePath = Runtime.config.DB_FILE_START_WITH_DOT_TO_MAKE_RELATIVE_TO_MAIN_DIR;

            if (filePath.Contains("."))
            {
                string baseDir = StringHandlers.Unescape(Runtime.config.MAIN_DIR);
                string relativePath = filePath;
                _filePath = Path.Combine(baseDir, relativePath);
            }
            else
            {
                _filePath = StringHandlers.Unescape(Runtime.config.DB_FILE_START_WITH_DOT_TO_MAKE_RELATIVE_TO_MAIN_DIR);
            }

            cnn.ConnectionString = $"SchemaName=db;uri=file://{_filePath}";
            cnn.Open();

            var testResultTable = new Tables.TestResults();
            if (!TableExists(testResultTable.TableName))
                ExecuteToDB(CreateTableString(testResultTable));
        }

        public Database(string filePath)
        {
            instance = new Database();
        }

        public bool TableExists(string tableName)
        {
            try
            {
                var reply = ExecuteToDB("Select * From TestResults");
                if (reply.HasRows)
                    return true;
            }
            catch { }

            return false;
        }

        public SqlDatabaseDataReader ExecuteToDB(string command)
        {
            try
            {

                if (cnn.State != ConnectionState.Open)
                {
                    cnn.ConnectionString = $"SchemaName=db;uri=file://{_filePath}";
                    cnn.Open();
                }
                else if (cnn.State == ConnectionState.Open)
                {
                    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand())
                    {
                        cmd.Connection = cnn;
                        cmd.CommandText = command;
                        return cmd.ExecuteReader();
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Failed execute to DB at SqlDatabaseDataReader TransactToDB(string tableName):" + ex.ToString());
            }

            return null;
        }

        public long InsertToTable(Tables.Table table, string[] values)
        {
            int counter = 0;
            string prefix;
            string suffix;

            string statement = $"INSERT INTO {table.TableName} (";

            foreach (var col in table.Columns)
            {
                if (!col.Value.ToUpper().Contains(identityColumnSubstring))
                {
                    if (counter == values.Length)
                        break;

                    if (col.Value.ToUpper().Contains(charTypeSubstring))
                        prefix = suffix = "'";
                    else
                        prefix = suffix = String.Empty;

                    statement += $"{StringHandlers.NoLineFeed(col.Key)},";

                    counter++;
                }
            }

            statement = statement.Substring(0, statement.Length - 1) + ")" +  Environment.NewLine + "VALUES (" ;

            counter = 0;
            foreach (var col in table.Columns)
            {
                if (!col.Value.ToUpper().Contains(identityColumnSubstring))
                {
                    if (counter == values.Length)
                        break;

                    if (col.Value.ToUpper().Contains(charTypeSubstring))
                        prefix = suffix = "'";
                    else
                        prefix = suffix = String.Empty;

                    statement+= $"{prefix}{StringHandlers.NoLineFeed(values[counter] ?? "") }{suffix},";

                    counter++;
                }
            }

            statement = statement.Substring(0, statement.Length - 1) + ")";

            var result = ExecuteToDB(statement);

            return cnn.LastInsertRowId;
        }

        public void UpdateInTable(Tables.Table table,long dbRowId, string[] values)
        {
            int counter = 0;
            string prefix;
            string suffix;

            string statement = $"UPDATE {table.TableName}" + Environment.NewLine + "Set ";

            foreach (var col in table.Columns)
            {
                if (!col.Value.ToUpper().Contains(identityColumnSubstring))
                {
                    if (counter == values.Length)
                        break;

                    if (col.Value.ToUpper().Contains(charTypeSubstring))
                        prefix = suffix = "'";
                    else
                        prefix = suffix = String.Empty;

                    statement += $"{StringHandlers.NoLineFeed(col.Key)} = {prefix}{values[counter] ?? "" }{suffix},";

                    counter++;
                }
            }

            statement = statement.Substring(0, statement.Length - 1) + Environment.NewLine + "WHERE ID=" + dbRowId + ";";

            var result = ExecuteToDB(statement);
        }


        public string CreateTableString(Tables.Table table)
        {
            string tableName = table.TableName;
            string strng = "CREATE TABLE " + tableName + " (";

            foreach (var column in table.Columns)
                strng += column.Key + " " + column.Value + ",";

            strng = strng.Substring(0, strng.Length - 1) + ")";

            return strng;
        }
    }
}

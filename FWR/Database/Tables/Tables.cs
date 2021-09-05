

using System.Collections.Generic;

namespace FWR.Database.Tables
{
    public class Table
    {
        public string TableName { get; set; }
        public Dictionary<string,string> Columns { get; set; }
    }

    public class TestResults : Table
    {
     public TestResults()
        {
            TableName = "TestResults";
            Columns = new Dictionary<string, string>() {
                {"ID", "BIGINT IDENTITY(1,1)" },
                {"DATE_TIME", "DATETIME" },
                {"QUEUE_NAME", "varchar(255)" },
                {"CYCLE_NAME", "varchar(255)" },
                {"SUITE_NAME", "varchar(255)" },
                {"TEST_NAME", "varchar(255)" },
                {"CONFIGURATION_NAME", "varchar(255)" },
                {"RESULT", "varchar(255)" },
                {"RUNTIME_SECONDS", "BIGINT" },
                {"LOG_ARTIFACTS_FOLDER_ID", "BIGINT" },
                {"ERROR_MESSAGE", "varchar(255)" },
                {"OUTPUT_DIRECTORY_PATH", "varchar(255)" },
                {"EXCLUDED", "BOOLEAN" },
            };
        }
    }
}

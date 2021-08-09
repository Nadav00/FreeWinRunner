

using System.Collections.Generic;

namespace FWR.Database.Tables
{
    public class Table
    {
        public Dictionary<string,string> Columns;
    }

    public class TestResults : Table
    {
     
     public TestResults()
        {
            Columns = new Dictionary<string, string>() {
                {"ID", "BIGINT IDENTITY(1,1)" },
                {"QUEUE_NAME", "varchar(255)" },
                {"CYCLE_NAME", "varchar(255)" },
                {"SUITE_NAME", "varchar(255)" },
                {"TEST_NAME", "varchar(255)" },
                {"CONFIGURATION_NAME", "varchar(255)" },
                {"Result", "INT" },
                {"RUNTIME_SECONDS", "BIGINT" },
                {"LOG_ARTIFACT_ID", "BIGINT" },
                {"ERROR_MESSAGE", "varchar(255)" },
                {"OUTPUT_DIRECTORY_PATH", "varchar(255)" },
            };
        }
    }
}

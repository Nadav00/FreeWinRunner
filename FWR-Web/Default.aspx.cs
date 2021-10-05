using FWR.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using TextBox = System.Web.UI.WebControls.TextBox;

namespace FWR_Web
{
    public partial class _Default : Page
    {
        private string endTable = "</table>";
        private string htmlToPresent;

        protected void Page_Load(object sender, EventArgs e)
        {
            var table = new FWR.Database.Tables.TestResults();
            var result = Database.Instance.ExecuteToDB($"Select * FROM {table.TableName} ORDER BY QUEUE_NAME,CYCLE_NAME,SUITE_NAME,DATE_TIME;");

            htmlToPresent = ResultsToHTML(result);

            headerListVeiw.Controls.Add(new LiteralControl(htmlToPresent));

            //RunsGridView.DataSource = result;
            //RunsGridView.DataBind();

            //CreateChildControls();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hey");
        }

        private string ResultsToHTML(SQLDatabase.Net.SQLDatabaseClient.SqlDatabaseDataReader result)
        {
            int count = 0;
            string returnString = string.Empty;
            string lastQueue = string.Empty;
            string lastCycle = string.Empty;
            string lastSuite = string.Empty;
            string cycleHtml = string.Empty;

            foreach (var entry in result)
            {
                count++;
                IDataRecord dataRow = (IDataRecord)entry;
                var entryQueue = dataRow[2];
                var entryCycle = dataRow[3];
                var entrySuite = dataRow[4];
                var entryTestName = dataRow[5];

                FWR.Engine.Test curTest = new FWR.Engine.Test
                {
                    Name = entryTestName.ToString(),
                    Result = (FWR.Engine.Const.Result) Enum.Parse(typeof(FWR.Engine.Const.Result), dataRow[7].ToString(), true),
                };

                if (!entryQueue.Equals(lastQueue))
                {
                    if (lastQueue != string.Empty)
                    {
                        lastCycle = string.Empty;
                        lastSuite = string.Empty;

                        cycleHtml += Environment.NewLine + endTable + "<!-- 1 -->"; //queue
                        cycleHtml += Environment.NewLine + endTable + "<!-- 2 -->"; //cycle
                        cycleHtml += Environment.NewLine + endTable + "<!-- 3 -->"; //suite
                    }

                    lastQueue = entryQueue.ToString();

                    returnString += cycleHtml;

                    cycleHtml = Environment.NewLine +
                    $"<table data-toggle=\"toggle\" style=\"width: 100 %; background-color:gold;\" border=\"0\"> <tr class=\"header\"> <td colspan=\"1\"> RUN:{lastQueue}</td></tr>";
                }

                if (!entryCycle.Equals(lastCycle))
                {
                    if (!lastCycle.Equals(string.Empty))
                    {
                      cycleHtml += endTable + "<!-- 4 -->"; //cycle
                    }

                    lastSuite = string.Empty;

                    lastCycle = entryCycle.ToString();

                    cycleHtml += Environment.NewLine +
                    $"    <tr><td><table style=\"width: 100 %; background-color:black;color:white\" border=\"0\"> <tr class=\"header hidetr\" > <td colspan=\"1\"> Cycle:{lastCycle}</td></tr>";
                }

                if (!entrySuite.Equals(lastSuite))
                {
                    if (!lastSuite.Equals(string.Empty))
                    {
                        cycleHtml += endTable + "<!-- 5 -->"; //cycle
                    }

                    lastSuite = entrySuite.ToString();

                    cycleHtml += Environment.NewLine +
                    $"      <tr><td><table style=\"width: 100 %; background-color:blue;color:white\" border=\"0\"> <tr class=\"header hidetr\"> <td colspan=\"1\"> Suite:{lastSuite}</td></tr>";
                }

                cycleHtml += Environment.NewLine +
                $"          <tr style=\"background-color:yellowgreen; color:white;\"><td> ,{entryTestName},{curTest.Result.ToString()} </td></tr>";
            }


            cycleHtml += Environment.NewLine + endTable + "<!-- 6 -->"; //suite
            cycleHtml += Environment.NewLine + endTable + "<!-- 7 -->"; //cycle

            returnString += cycleHtml;

            returnString += Environment.NewLine + endTable + "<!-- 8 -->"; //queue
            returnString += Environment.NewLine + endTable + "<!-- 9 -->"; //table

            return returnString;
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void CreateChildControls()
        {
            //headerListVeiw.Controls.Add(new LiteralControl(htmlToPresent));
        }

    }
}
using FWR.Database;
using System;
using System.Collections.Generic;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            SQLDatabase.Net.SQLDatabaseClient.SqlDatabaseDataReader result = Database.Instance.ExecuteToDB($"Select * FROM {new FWR.Database.Tables.TestResults().TableName}");


            RunsGridView.DataSource = result;
            RunsGridView.DataBind();

            CreateChildControls();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hey");

        }


        // Add two LiteralControls that render HTML H3 elements and text.
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void CreateChildControls()
        {
            headerListVeiw.Controls.Add(new LiteralControl("<div><BR> HELLO!!!! </div>"));
        }

    }
}
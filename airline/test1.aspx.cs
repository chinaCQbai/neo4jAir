using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace neo4jtest.neo4j
{
    public partial class test1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["middle"] = "";
            Session["mids"] = "";
        }
        protected void Unnamed_Click(object sender, ImageClickEventArgs e)
        {
            Session["origincode"] = txtOrigin.Text;
            Session["origindate"] = "day" + txtOrigindate.Value;
            Session["origintime"] = txtOrigindate.Value;
            Session["destcode"] = txtDest.Text;
            Session["destdate"] = "day" + txtDestdate.Value;
            Session["desttime"] =  txtDestdate.Value;
            Session["middle"] = dropmid.SelectedValue;
            Session["mids"] = txtMid.Text;
            Response.Redirect("airfare.aspx");
        }
    }
}
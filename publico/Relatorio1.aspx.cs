using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class publico_Relatorio1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddl.DataSource = ComboData.Data().ToList();
            ddl.DataTextField = "DayName";
            ddl.DataValueField = "DayValue";
            ddl.DataBind();
        }
    }

    protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
    {
        lbData.Text = ddl.SelectedItem.Value;
    }
}

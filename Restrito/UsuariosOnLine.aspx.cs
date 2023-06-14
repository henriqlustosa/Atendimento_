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

public partial class publico_UsuariosOnLine : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        var users = Membership.GetAllUsers();
        rpt.DataSource = users;
        rpt.DataBind();
        Label1.Text = "Última atualização " + DateTime.Now.ToString();
        int qtdOnline = Membership.GetNumberOfUsersOnline();
        lbQuantidadeOnline.Text = qtdOnline + " online";
    }

}

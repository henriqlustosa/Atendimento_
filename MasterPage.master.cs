using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var user = UsuarioDAO.getUsuario(Page.User.Identity.Name.ToString());
            if (user.Uimage == null)
            {
                imgAvatar.Src = "~/UserImages/User.png";
                img1.Src = "~/UserImages/User.png";
            }
            else
            {
                imgAvatar.Src = user.Uimage;
                img1.Src = user.Uimage;
            }
        }
    }
}

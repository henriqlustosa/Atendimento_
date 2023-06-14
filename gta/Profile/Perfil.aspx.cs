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
using System.Collections.Generic;
using System.Data.SqlClient;

public partial class publico_Perfil : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack){
            Usuario user = new Usuario();
            user = UsuarioDAO.getUsuario(User.Identity.Name);

            if (user.Uimage == null)
            {
                imgAvatar.Src = "~/UserImages/User.png";
            }
            else
            {
                imgAvatar.Src = user.Uimage;
                lbNomeCompleto.Text = "Nome - " + user.Nome_Completo;
                lbSetor.Text = " Setor - " + user.Setor;
                lbCargo.Text = " Cargo - " + user.Cargo;
            }
            imgAvatar.Width = 190;
            imgAvatar.Height = 200;
        }
    }

    protected void trocaAvatar_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Profile/UploadImageProfile.aspx?usuario=" + User.Identity.Name);
    }

    

    protected void btnEncaminhaAlteracao_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Profile/ComplementoUsuario.aspx?usuario=" + User.Identity.Name);
    }
}

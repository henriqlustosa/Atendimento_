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
using System.IO;
using System.Data.SqlClient;

public partial class publico_ComplementoUsuario : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txbLogin.Text = Request.QueryString["usuario"];
            var user = UsuarioDAO.getUsuario(txbLogin.Text);

            txbnomeCompleto.Text = user.Nome_Completo;
            txbSetor.Text = user.Setor;
            txbCargo.Text = user.Cargo;
        }
    }

    protected void btnGravar_Click(object sender, EventArgs e)
    {
        Usuario usuario = new Usuario();

        string _Uimage = "~/UserImages/User.png";
        string pathImage = "~/UserImages/";

        usuario.UserLogin = txbLogin.Text;
        usuario.Nome_Completo = txbnomeCompleto.Text;
        usuario.Cargo = txbCargo.Text;
        usuario.Setor = txbSetor.Text;

        if (FileUpload1.PostedFile != null)
        {
            string strpath = Path.GetExtension(FileUpload1.FileName);

            //string strname = _login;

            if (strpath != ".jpg" && strpath != ".jpeg" && strpath != ".gif" && strpath != "png")
            {
                lbMsg.Text = "Permitido apenas imagens do tipo (jpg, jpeg, gif e png)";
                lbMsg.ForeColor = System.Drawing.Color.Red;
            }
            string fileimg = Path.GetFileName(FileUpload1.PostedFile.FileName);

            FileUpload1.SaveAs(Server.MapPath(pathImage) + usuario.UserLogin + strpath);

            _Uimage = pathImage + usuario.UserLogin + strpath;
        }


        if (UsuarioDAO.getUsuario(txbLogin.Text).UserLogin == null || UsuarioDAO.getUsuario(txbLogin.Text).UserLogin == "")
        {
            UsuarioDAO.CadastroComplementar(usuario.UserLogin, usuario.Nome_Completo, usuario.Setor, usuario.Cargo, _Uimage);
        }
        else
        {
            UsuarioDAO.AtualizaCadastroComplementar(usuario.UserLogin, usuario.Nome_Completo, usuario.Setor, usuario.Cargo, _Uimage);
        }
       

        Response.Redirect("~/Profile/Perfil.aspx");
    }


}

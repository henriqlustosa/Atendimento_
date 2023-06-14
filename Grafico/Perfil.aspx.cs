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
    MembershipUserCollection users;
    protected void Page_Load(object sender, EventArgs e)
    {
        users = Membership.GetAllUsers();

        if (!IsPostBack)
        {
            // Bind users to ListBox.
            BindUsers();
            getDadosUsuario();
        }
    }

    protected void BindUsers()
    {
        UsersListBox.DataSource = users;
        UsersListBox.DataBind();
    }

    protected void Selected_IndexChanged(object sender, EventArgs args)
    {
        getDadosUsuario();
        getUltimasAtividadeUsuario(UsersListBox.SelectedItem.Value.ToUpper());
    }

    protected void getUltimasAtividadeUsuario(string usuario)
    {
        var atividades = AtivoDAO.getAtivosRecentAtividadesUsuario(usuario);
        rpt.DataSource = atividades;
        rpt.DataBind();
    }

    protected void getDadosUsuario()
    {
        lbNameUser.Text = UsersListBox.SelectedItem.Value.ToUpper();

        MembershipUser u = users[UsersListBox.SelectedItem.Value];

        EmailLabel.Text = u.Email;
        LastLoginDateLabel.Text = u.LastLoginDate.ToString();
        CreationDateLabel.Text = u.CreationDate.ToString();

        Usuario user = new Usuario();
        user = UsuarioDAO.getUsuario(u.UserName);

        imgAvatar.Src = user.Uimage;
        lbSetor.Text = user.Setor;
        lbCargo.Text = user.Cargo;
        imgAvatar.Width = 190;
        imgAvatar.Height = 200;
    }


    protected void btnEncaminhaAlteracao_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Profile/ComplementoUsuario.aspx?usuario=" + lbNameUser.Text);
    }
}

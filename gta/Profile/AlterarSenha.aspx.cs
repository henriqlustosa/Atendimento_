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

public partial class publico_AlterarSenha : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    public void ChangePassword_OnClick(object sender, EventArgs args)
    {
        // Update the password.

        MembershipUser u = Membership.GetUser(User.Identity.Name);

        try
        {
            if (u.ChangePassword(OldPasswordTextbox.Text, PasswordTextbox.Text))
            {
                Msg.Text = "Senha alterada.";
            }
            else
            {
                Msg.Text = "Falha na mudança de senha. Digite novamente seus valores e tente novamente.";
            }
        }
        catch (Exception e)
        {
            Msg.Text = "Ocorreu uma exceção: " + Server.HtmlEncode(e.Message) + ". Digite novamente seus valores e tente novamente.";
        }
    }
}

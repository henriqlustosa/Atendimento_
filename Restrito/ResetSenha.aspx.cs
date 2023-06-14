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

public partial class publico_ResetSenha : System.Web.UI.Page
{
    MembershipUser u;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Membership.EnablePasswordReset)
        {
            FormsAuthentication.RedirectToLoginPage();
        }

        Msg.Text = "";

        if (!IsPostBack)
        {
            Msg.Text = "Por favor, forneça um nome de usuário.";
        }
        else
        {
            VerifyUsername();
        }
    }


    public void VerifyUsername()
    {
        u = Membership.GetUser(UsernameTextBox.Text, false);

        if (u == null)
        {
            Msg.Text = "Usuário " + Server.HtmlEncode(UsernameTextBox.Text) + " não encontrado. Por favor, verifique os dados e tente novamente.";

            ResetPasswordButton.Enabled = false;
        }
        else
        {
            ResetPasswordButton.Enabled = true;
        }
    }

    public void ResetPassword_OnClick(object sender, EventArgs args)
    {
        string newPassword;

        u = Membership.GetUser(UsernameTextBox.Text, false);

        if (u == null)
        {
            Msg.Text = "Usuário " + Server.HtmlEncode(UsernameTextBox.Text) + " não encontrado. Por favor, verifique os dados e tente novamente.";
            return;
        }

        try
        {
            newPassword = u.ResetPassword();
        }
        catch (MembershipPasswordException e)
        {
            Msg.Text = "Invalid password answer. Please re-enter and try again.";
            return;
        }
        catch (Exception e)
        {
            Msg.Text = e.Message;
            return;
        }

        if (newPassword != null)
        {
            Msg.Text = "Senha resetada. Sua nova senha é: " + Server.HtmlEncode(newPassword);
        }
        else
        {
            Msg.Text = "Falha ao resetar senha. Por favor, entre com os valores novamente.";
        }
    }
}

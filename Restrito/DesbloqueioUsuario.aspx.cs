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

public partial class Restrito_DesbloqueioUsuario : System.Web.UI.Page
{
    MembershipUser u;

    protected void Page_Load(object sender, EventArgs e)
    {
        Msg.Text = "";

        if (!IsPostBack)
        {
            Msg.Text = "Por favor, forneça um nome de usuário.";

            getUsersLocked();
        }
        else
        {
            VerifyUsername();
        }

    }


    /// <summary>
    ///  criar metodo datatable
    /// </summary>

    protected void getUsersLocked()
    {
        //DataTable dt = new DataTable();

        MembershipUserCollection filteredUsers = new MembershipUserCollection();

        try
        {
            var users = Membership.GetAllUsers();

            foreach (MembershipUser user in users)
            {
                // if user is currently online, add to gridview list
                if (user.IsLockedOut == true)
                {
                    filteredUsers.Add(user);
                }
            }

        }
        catch (Exception ex)
        {
            string error = ex.Message;
        }

        rpt.DataSource = filteredUsers;
        rpt.DataBind();
    }


    public void VerifyUsername()
    {
        u = Membership.GetUser(txbUser.Text, false);

        if (u == null)
        {
            Msg.Text = "Usuário " + Server.HtmlEncode(txbUser.Text) + " não encontrado. Por favor, verifique os dados e tente novamente.";

            UnlockButton.Enabled = false;
        }
        else
        {
            UnlockButton.Enabled = true;
        }
    }


    public void UnlockUser_OnClick(object sender, EventArgs args)
    {
        u = Membership.GetUser(txbUser.Text, false);

        if (u == null)
        {
            Msg.Text = "Usuário " + Server.HtmlEncode(txbUser.Text) + " não encontrado. Por favor, verifique os dados e tente novamente.";
            return;
        }

        try
        {
            u.UnlockUser();
            Msg.Text = "Usuário " + Server.HtmlEncode(txbUser.Text) + " desbloqueado.";
            Response.Redirect(Request.RawUrl);
        }
        catch (MembershipPasswordException e)
        {
            Msg.Text = "Não foi possivel desbloquear. Tente novamente.";
            return;
        }
        catch (Exception e)
        {
            Msg.Text = e.Message;
            return;
        }
    }
}

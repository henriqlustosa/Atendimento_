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
using System.Data.SqlClient;

public partial class publico_AlterarCadastro : System.Web.UI.Page
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lbMsg.Text = "";
        }
    }

    public void UpdateUserNameButton_OnClick(object sender, EventArgs args)
    {
        string oldUserName = LoginAntigoTextBox.Text;
        string newUserName = LoginNovoTextBox.Text;

        string msg = ChangeUsername(oldUserName, newUserName);
        lbMsg.Text = msg;
    }

    public static string ChangeUsername(string oldUsername, string newUsername)
    {
        string msg = "";

         using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;
            try
            {
                cmm.CommandText = "UPDATE dbo.aspnet_Users " +
                                    " SET UserName=@NewUsername" +
                                    ",LoweredUserName=@LoweredNewUsername " +
                                    " WHERE UserName=@OldUsername";

                cmm.Parameters.Add(new SqlParameter("@OldUsername", oldUsername));
                cmm.Parameters.Add(new SqlParameter("@NewUsername", newUsername));
                cmm.Parameters.Add(new SqlParameter("@LoweredNewUsername", newUsername.ToLower()));

                cmm.ExecuteNonQuery();

                mt.Commit();
                mt.Dispose();
                cnn.Close();
                msg = "Alteração realizada com sucesso!";
                
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                mt.Rollback();
            }
        }
         return msg;
    }
}

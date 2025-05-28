using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Login : System.Web.UI.Page
{
    private const string Url = "~/encaminhamento/cadencaminhamento.aspx";

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string login = txtUsuario.Text.Trim();
        string senha = txtSenha.Text.Trim();

        // 1. Autenticação no AD
        try
        {
            using (DirectoryEntry entry = new DirectoryEntry("LDAP://10.10.68.43", login, senha))
            {
                object nativeObject = entry.NativeObject; // Autenticado

                // 2. Verificar no banco se o usuário está cadastrado e ativo
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
                {
                    string sql = "SELECT Id FROM Usuarios WHERE LoginRede = @LoginRede";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@LoginRede", login);

                    con.Open();
                    object perfil = cmd.ExecuteScalar();
                    con.Close();

                    if (perfil == null)
                    {
                        lblMensagem.Text = "Usuário sem permissão de acesso.";
                        return;
                    }


                    // 3. Salva informações em sessão

                    Session["login"] = login;

                    //Session["perfil"] = perfil.ToString();

                    // Exemplo: usuário tem perfil 1 e 2

                    List<int> perfisDoUsuario = new List<int> { 1, 2 };

                    Session["perfis"] = perfisDoUsuario;

                    // 4. Redireciona
                    Response.Redirect(Url);
                }
            }
        }
        catch (Exception ex)
        {
            string erro = ex.Message;
            lblMensagem.Text = "Login inválido.";
        }
    }
}


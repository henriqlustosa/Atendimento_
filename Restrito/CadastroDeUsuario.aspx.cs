using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Restrito_CadastroDeUsuario : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 1. Verifica se o usuário está logado (existe sessão)
        if (Session["login"] == null)
        {
            Response.Redirect("~/login.aspx"); // Redireciona se não estiver logado
            return;
        }

        // 2. Verifica se o perfil é diferente de "1" (Administrador)
        if (Session["perfil"].ToString() != "1")
        {
            Response.Redirect("~/SemPermissao.aspx"); // Redireciona se não for admin
        }
        if (!IsPostBack)
        {
            CarregarPerfis();
            if (!string.IsNullOrEmpty(Request.QueryString["login"]))
            {
                txtLogin.Text = Request.QueryString["login"];
                CarregarUsuario(txtLogin.Text);               
            }
            CarregarUsuarios();
        }
    }

    private void CarregarPerfis()
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Reserva_anfiteatroConnectionString"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("SELECT Id, Nome FROM Perfis", con);
            con.Open();
            cblPerfis.DataSource = cmd.ExecuteReader();
            cblPerfis.DataTextField = "Nome";
            cblPerfis.DataValueField = "Id";
            cblPerfis.DataBind();
        }
    }

    private void CarregarUsuario(string login)
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Reserva_anfiteatroConnectionString"].ToString()))
        {
            string query = @"SELECT NomeCompleto, Email FROM Usuarios WHERE LoginRede = @LoginRede";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@LoginRede", login);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                txtNome.Text = reader["NomeCompleto"].ToString();
                txtEmail.Text = reader["Email"].ToString();
            }
            reader.Close();

            // Carrega perfis do usuário
            query = @"
    SELECT up.PerfilId 
    FROM UsuariosPerfis up
    INNER JOIN Usuarios u ON u.Id = up.UsuarioId
    WHERE u.LoginRede = @LoginRede";

            cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@LoginRede", login);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ListItem item = cblPerfis.Items.FindByValue(reader["PerfilId"].ToString());
                if (item != null)
                    item.Selected = true;
            }
            reader.Close();

        }
    }

        protected void btnBuscarAD_Click(object sender, EventArgs e)
    {
        
        string login = txtLogin.Text.Trim();
        CarregarUsuario(login);

        try
        {
            using (DirectoryEntry entry = new DirectoryEntry("LDAP://10.10.68.43"))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(sAMAccountName=" + login + ")";
                    searcher.PropertiesToLoad.Add("cn");
                    searcher.PropertiesToLoad.Add("mail");

                    SearchResult result = searcher.FindOne();

                    if (result != null)
                    {
                        txtNome.Text = result.Properties.Contains("cn") ? result.Properties["cn"][0].ToString() : "";
                        txtEmail.Text = result.Properties.Contains("mail") ? result.Properties["mail"][0].ToString() : "";
                    }
                    else
                    {
                        lblResultado.Text = "Usuário não encontrado no AD.";
                        lblResultado.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblResultado.Text = "Erro: " + ex.Message;
            lblResultado.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void btnSalvar_Click(object sender, EventArgs e)
    {
        string login = txtLogin.Text.Trim();
        string nome = txtNome.Text.Trim();
        string email = txtEmail.Text.Trim();

        if (string.IsNullOrEmpty(login))
        {
            lblResultado.Text = "Preencha todos os campos obrigatórios.";
            lblResultado.ForeColor = System.Drawing.Color.Red;
            return;
        }

        try
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Reserva_anfiteatroConnectionString"].ToString()))
            {
                con.Open();

                string query = @"IF EXISTS (SELECT 1 FROM Usuarios WHERE LoginRede = @LoginRede)
                                UPDATE Usuarios SET NomeCompleto = @NomeCompleto, Email = @Email WHERE LoginRede = @LoginRede
                                ELSE
                                INSERT INTO Usuarios (LoginRede, NomeCompleto, Email) VALUES (@LoginRede, @NomeCompleto, @Email)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@LoginRede", login);
                cmd.Parameters.AddWithValue("@NomeCompleto", nome);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.ExecuteNonQuery();

                // Buscar o ID do usuário
                cmd = new SqlCommand("SELECT Id FROM Usuarios WHERE LoginRede = @LoginRede", con);
                cmd.Parameters.AddWithValue("@LoginRede", login);
                int usuarioId = Convert.ToInt32(cmd.ExecuteScalar());

                // Remove perfis antigos
                cmd = new SqlCommand("DELETE FROM UsuariosPerfis WHERE UsuarioId = @UsuarioId", con);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmd.ExecuteNonQuery();

                // Insere os novos perfis
                foreach (ListItem item in cblPerfis.Items)
                {
                    if (item.Selected)
                    {
                        cmd = new SqlCommand("INSERT INTO UsuariosPerfis (UsuarioId, PerfilId) VALUES (@UsuarioId, @PerfilId)", con);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@PerfilId", item.Value);
                        cmd.ExecuteNonQuery();
                    }
                }


                lblResultado.Text = "Usuário salvo com sucesso.";
                lblResultado.ForeColor = System.Drawing.Color.Green;
            }
        }
        catch (Exception ex)
        {
            lblResultado.Text = "Erro ao salvar: " + ex.Message;
            lblResultado.ForeColor = System.Drawing.Color.Red;
        }
        CarregarUsuarios(); //carrega lista de usuarios com opção de deletar 

    }

    private void CarregarUsuarios()
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Reserva_anfiteatroConnectionString"].ToString()))
        {
            string query = "SELECT LoginRede, NomeCompleto, Email FROM Usuarios ORDER BY NomeCompleto";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            gvUsuarios.DataSource = reader;
            gvUsuarios.DataBind();
        }
    }
    protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Excluir")
        {
            string login = e.CommandArgument.ToString();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Reserva_anfiteatroConnectionString"].ToString()))
            {
                con.Open();

                // Remove os perfis vinculados primeiro
                SqlCommand cmd = new SqlCommand("DELETE FROM UsuarioPerfil WHERE UsuarioLogin = @LoginRede", con);
                cmd.Parameters.AddWithValue("@LoginRede", login);
                cmd.ExecuteNonQuery();

                // Depois remove o usuário
                cmd = new SqlCommand("DELETE FROM Usuarios WHERE LoginRede = @LoginRede", con);
                cmd.Parameters.AddWithValue("@LoginRede", login);
                cmd.ExecuteNonQuery();
            }

            lblResultado.Text = "Usuário excluído com sucesso.";
            lblResultado.ForeColor = System.Drawing.Color.Green;

            CarregarUsuarios(); // Atualiza o GridView
        }
    }

}

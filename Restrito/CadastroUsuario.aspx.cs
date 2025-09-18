//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.DirectoryServices;
//using System.Linq;
//using System.Web;
//using System.Web.Security;
//using System.Web.Services;
//using System.Web.UI;
//using System.Web.UI.HtmlControls;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Xml.Linq;

//public partial class Restrito_CadastroUsuario : BasePage
//{
   
//    protected void Page_Load(object sender, EventArgs e)
//    {
//        if (Session["login"] == null)
//        {
//            Response.Redirect("~/login.aspx"); // Redireciona se não estiver logado
//            return;
//        }

//        // 2. Verifica se o perfil é diferente de "1" (Administrador)
//        List<int> perfis = Session["perfis"] as List<int>;
//        if (perfis == null || (!perfis.Contains(3)))
//        {
//            Response.Redirect("~/aberto/SemPermissao.aspx");
//        }
//        if (!IsPostBack)
//        {
//            CarregarPerfis();
//            if (!string.IsNullOrEmpty(Request.QueryString["login"]))
//            {
//                txtLogin.Text = Request.QueryString["login"];
//                CarregarUsuario(txtLogin.Text);
//            }
//            CarregarUsuarios();
           
//        }
//    }

//    private void CarregarPerfis()
//    {
//        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//        {
//            SqlCommand cmd = new SqlCommand("SELECT idPerfil, Nome FROM Perfis", con);
//            con.Open();
//            cblPerfis.DataSource = cmd.ExecuteReader();
//            cblPerfis.DataTextField = "Nome";
//            cblPerfis.DataValueField = "idPerfil";
//            cblPerfis.DataBind();
//        }
//    }

//    private void CarregarUsuario(string login)
//    {
//        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//        {
//            string query = @"SELECT NomeCompleto, Email FROM Usuarios WHERE LoginRede = @LoginRede";
//            SqlCommand cmd = new SqlCommand(query, con);
//            cmd.Parameters.AddWithValue("@LoginRede", login);
//            con.Open();
//            SqlDataReader reader = cmd.ExecuteReader();

//            if (reader.Read())
//            {
//                txtNome.Text = reader["NomeCompleto"].ToString();
//                txtEmail.Text = reader["Email"].ToString();
//            }
//            reader.Close();

//            // Carrega perfis do usuário
//            query = @"
//    SELECT up.idPerfil 
//    FROM UsuarioPerfil up
//    INNER JOIN Usuarios u ON u.UsuarioId = up.UsuarioId
//    WHERE u.LoginRede = @LoginRede";

//            cmd = new SqlCommand(query, con);
//            cmd.Parameters.AddWithValue("@LoginRede", login);
//            reader = cmd.ExecuteReader();

//            while (reader.Read())
//            {
//                ListItem item = cblPerfis.Items.FindByValue(reader["idPerfil"].ToString());
//                if (item != null)
//                    item.Selected = true;
//            }
//            reader.Close();

//        }
//    }

//    protected void btnBuscarAD_Click(object sender, EventArgs e)
//    {
//        txtRF.Text = "";
//        //txtSetor.Text = "";
//        //txtRamal1.Text = "";
//        cblPerfis.ClearSelection();
//        string login = txtLogin.Text.Trim();
//        CarregarUsuario(login);

//        try
//        {
//            using (DirectoryEntry entry = new DirectoryEntry("LDAP://10.10.68.43"))
//            {
//                using (DirectorySearcher searcher = new DirectorySearcher(entry))
//                {
//                    searcher.Filter = "(sAMAccountName=" + login + ")";
//                    searcher.PropertiesToLoad.Add("cn");
//                    searcher.PropertiesToLoad.Add("mail");

//                    SearchResult result = searcher.FindOne();

//                    if (result != null)
//                    {
//                        txtNome.Text = result.Properties.Contains("cn") ? result.Properties["cn"][0].ToString() : "";
//                        txtEmail.Text = result.Properties.Contains("mail") ? result.Properties["mail"][0].ToString() : "";
//                    }
//                    else
//                    {
//                        lblResultado.Text = "Usuário não encontrado no AD.";
//                        lblResultado.ForeColor = System.Drawing.Color.Red;
//                    }
//                }

//            }
//            carreDadosCordenador(login);
//        }
//        catch (Exception ex)
//        {
//            lblResultado.Text = "Erro: " + ex.Message;
//            lblResultado.ForeColor = System.Drawing.Color.Red;
//        }
//    }



//    protected void btnSalvar_Click(object sender, EventArgs e)
//    {
//        string login = txtLogin.Text.Trim();
//        string nome = txtNome.Text.Trim();
//        string email = txtEmail.Text.Trim();
//        string rfCordernador = txtRF.Text.Trim();
//        //string ramal1 = txtRamal1.Text.Trim();
//        int ativo = 1;
//        //string setorCordenador = txtSetor.Text.Trim();

//        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(nome))
//        {
//            lblResultado.Text = "Preencha todos os campos obrigatórios.";
//            lblResultado.ForeColor = System.Drawing.Color.Red;
//            return;
//        }

//        // Validação dos perfis
//        bool perfilSelecionado = false;
//        foreach (ListItem item in cblPerfis.Items)
//        {
//            if (item.Selected)
//            {
//                perfilSelecionado = true;
//                break;
//            }
//        }
//        if (txtRF.Text.Length < 7)
//        {
//            lblResultado.Text = "O campo RF do coordenador tem que ter 7 numeros.";
//            lblResultado.ForeColor = System.Drawing.Color.Red;
//            return;
//        }

//        if (!perfilSelecionado)
//        {
//            lblResultado.Text = "Selecione pelo menos um perfil de acesso.";
//            lblResultado.ForeColor = System.Drawing.Color.Red;
//            return;
//        }

//        try
//        {
//            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//            {
//                con.Open();

//                string query = @"IF EXISTS (SELECT 1 FROM Usuarios WHERE LoginRede = @LoginRede)
//                            UPDATE Usuarios SET NomeCompleto = @NomeCompleto, Email = @Email, RF =@RF
//                            ,Ativo=@Ativo
//                            WHERE LoginRede = @LoginRede
//                            ELSE
//                            INSERT INTO Usuarios (LoginRede, NomeCompleto, Email, RF,  Ativo)
//                            VALUES (@LoginRede, @NomeCompleto, @Email,@RF,@Ativo)";

//                SqlCommand cmd = new SqlCommand(query, con);
//                cmd.Parameters.AddWithValue("@LoginRede", login);
//                cmd.Parameters.AddWithValue("@NomeCompleto", nome);
//                cmd.Parameters.AddWithValue("@Email", email);
//                cmd.Parameters.AddWithValue("@RF", rfCordernador);
//                //cmd.Parameters.AddWithValue("@ramal_Coordenador", ramal1);
//                cmd.Parameters.AddWithValue("@Ativo", ativo);
//                //cmd.Parameters.AddWithValue("@setor_Coordenador", setorCordenador);

//                cmd.ExecuteNonQuery();

//                // Buscar o ID do usuário
//                cmd = new SqlCommand("SELECT UsuarioId FROM Usuarios WHERE LoginRede = @LoginRede", con);
//                cmd.Parameters.AddWithValue("@LoginRede", login);
//                int usuarioId = Convert.ToInt32(cmd.ExecuteScalar());

//                // Remove perfis antigos
//                cmd = new SqlCommand("DELETE FROM UsuarioPerfil WHERE UsuarioId = @UsuarioId", con);
//                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
//                cmd.ExecuteNonQuery();

//                // Insere os novos perfis
//                foreach (ListItem item in cblPerfis.Items)
//                {
//                    if (item.Selected)
//                    {
//                        cmd = new SqlCommand("INSERT INTO UsuarioPerfil (UsuarioId, idPerfil) VALUES (@UsuarioId, @idPerfil)", con);
//                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
//                        cmd.Parameters.AddWithValue("@idPerfil", item.Value);
//                        cmd.ExecuteNonQuery();
//                    }
//                }

//                lblResultado.Text = "Usuário salvo com sucesso.";
//                lblResultado.ForeColor = System.Drawing.Color.Green;
//            }
//        }
//        catch (Exception ex)
//        {
//            lblResultado.Text = "Erro ao salvar: " + ex.Message;
//            lblResultado.ForeColor = System.Drawing.Color.Red;
//        }

//        //Grava centro de custo do usuario
//        for (int i = 0; i < select1.Items.Count; i++)
//        {
//            if (select1.Items[i].Selected)
//            {
//                OsDAO.GravaCentroDecustoDofuncionario(login, int.Parse(select1.Items[i].Value));
//            }
//        }

//        CarregarUsuarios(); //carrega lista de usuarios com opção de deletar 
//        CarregarUsuariosInativos();
//        limpaDados();
//    }


//    private void CarregarUsuarios()
//    {
//        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//        {
//            string query = "SELECT UsuarioId, LoginRede, NomeCompleto, Email, RF FROM Usuarios where Ativo=1 ORDER BY NomeCompleto";
//            SqlCommand cmd = new SqlCommand(query, con);
//            con.Open();

//            SqlDataReader reader = cmd.ExecuteReader();
//            gvUsuarios.DataSource = reader;
//            gvUsuarios.DataBind();
//        }
//    }
//    private void CarregarUsuariosInativos()
//    {
//        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//        {
//            string query = "SELECT UsuarioId, LoginRede, NomeCompleto, Email, RF FROM Usuarios where Ativo=0 ORDER BY NomeCompleto";
//            SqlCommand cmd = new SqlCommand(query, con);
//            con.Open();

//            SqlDataReader reader = cmd.ExecuteReader();
//            gvUsuariosInativos.DataSource = reader;
//            gvUsuariosInativos.DataBind();
//        }
//    }
//    protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
//    {
//        if (e.CommandName == "Inativar")
//        {
//            string Id2 = e.CommandArgument.ToString();
//            int Id = Convert.ToInt32(Id2);

//            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//            {
//                con.Open();
//                try
//                {
//                    // Remove os perfis vinculados primeiro
//                    SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Usuarios] SET[Ativo] = 0 WHERE UsuarioId = @UsuarioId", con);
//                    cmd.Parameters.AddWithValue("@UsuarioId", Id);
//                    cmd.ExecuteNonQuery();

//                    // Depois remove o usuário
//                    //cmd = new SqlCommand("DELETE FROM Usuarios WHERE Id = @Id", con);
//                    //cmd.Parameters.AddWithValue("@Id", Id);
//                    //cmd.ExecuteNonQuery();

//                    lblResultado.Text = "Usuário Inativado com sucesso.";
//                    lblResultado.ForeColor = System.Drawing.Color.Green;
//                }
//                catch (Exception ex)
//                {
//                    lblResultado.Text = "Erro ao Inativar: " + ex.Message;
//                    lblResultado.ForeColor = System.Drawing.Color.Red;
//                }
//            }

//            CarregarUsuarios(); // Atualiza o GridView
//            CarregarUsuariosInativos();
//        }
//        else if (e.CommandName == "centro_custo")
//        {
//            string loginRede = e.CommandArgument.ToString();

//            DataTable dt = new DataTable();
//            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//            {
//                string query = "SELECT UsuarioCentroDeCustoId, nomeUsuario, codigoCentroDeCusto, descricaoCentroDeCusto FROM Vw_Usuario_CentroDeCusto WHERE LoginDeRedeUsuario = @LoginDeRedeUsuario";
//                SqlCommand cmd = new SqlCommand(query, con);
//                cmd.Parameters.AddWithValue("@LoginDeRedeUsuario", loginRede);
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                da.Fill(dt);
//            }

//            gvCentroCusto.DataSource = dt;
//            gvCentroCusto.DataBind();

//            // ABRE O MODAL (usando API do Bootstrap 5)
//            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModal", @"
//            var myModal = new bootstrap.Modal(document.getElementById('modalCentroCusto'));
//            myModal.show();
//        ", true);

//        }
//    }

//    protected void gvUsuariosInativos_RowCommand(object sender, GridViewCommandEventArgs e)
//    {
//        if (e.CommandName == "Ativar")
//        {
//            string Id2 = e.CommandArgument.ToString();
//            int Id = Convert.ToInt32(Id2);

//            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//            {
//                con.Open();
//                try
//                {
//                    // Remove os perfis vinculados primeiro
//                    SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Usuarios] SET[Ativo] = 1 WHERE UsuarioId = @UsuarioId", con);
//                    cmd.Parameters.AddWithValue("@UsuarioId", Id);
//                    cmd.ExecuteNonQuery();

//                    // Depois remove o usuário
//                    //cmd = new SqlCommand("DELETE FROM Usuarios WHERE Id = @Id", con);
//                    //cmd.Parameters.AddWithValue("@Id", Id);
//                    //cmd.ExecuteNonQuery();

//                    lblResultado.Text = "Usuário Ativado com sucesso.";
//                    lblResultado.ForeColor = System.Drawing.Color.Green;
//                }
//                catch (Exception ex)
//                {
//                    lblResultado.Text = "Erro ao Ativar: " + ex.Message;
//                    lblResultado.ForeColor = System.Drawing.Color.Red;
//                }
//            }

//            CarregarUsuarios(); // Atualiza o GridView
//            CarregarUsuariosInativos();
//        }
//        else if (e.CommandName == "centro_custo")
//        {
//            string loginRede = e.CommandArgument.ToString();

//            DataTable dt = new DataTable();
//            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//            {
//                string query = "SELECT UsuarioCentroDeCustoId, nomeUsuario, codigoCentroDeCusto, descricaoCentroDeCusto FROM Vw_Usuario_CentroDeCusto WHERE loginrede = @login";
//                SqlCommand cmd = new SqlCommand(query, con);
//                cmd.Parameters.AddWithValue("@login", loginRede);
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                da.Fill(dt);
//            }

//            gvCentroCusto.DataSource = dt;
//            gvCentroCusto.DataBind();

//            // ABRE O MODAL (usando API do Bootstrap 5)
//            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModal", @"
//            var myModal = new bootstrap.Modal(document.getElementById('modalCentroCusto'));
//            myModal.show();
//        ", true);

//        }
//    }

//    protected void gvCentroCusto_RowCommand(object sender, GridViewCommandEventArgs e)
//    {
//        if (e.CommandName == "Remover")
//        {
//            string Id2 = e.CommandArgument.ToString();
//            int Id = Convert.ToInt32(Id2);
//            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//            {
//                con.Open();
//                try
//                {
//                    // Remove os perfis vinculados primeiro
//                    SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[UsuarioCentroDeCusto] WHERE UsuarioCentroDeCustoId = @UsuarioCentroDeCustoId", con);
//                    cmd.Parameters.AddWithValue("@UsuarioCentroDeCustoId", Id);
//                    cmd.ExecuteNonQuery();

//                }
//                catch (Exception ex)
//                {
//                    lblResultado.Text = "Erro ao Ativar: " + ex.Message;
//                    lblResultado.ForeColor = System.Drawing.Color.Red;
//                }
//            }

//            CarregarUsuarios(); // Atualiza o GridView
//            CarregarUsuariosInativos();
//        }
//    }

//    [WebMethod]
//    public static string[] getSetor(string prefixo)
//    {
//        List<string> clientes = new List<string>();
//        using (SqlConnection conn = new SqlConnection())
//        {
//            conn.ConnectionString = ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ConnectionString;
//            using (SqlCommand cmd = new SqlCommand())
//            {
//                cmd.CommandText = "SELECT top 20 [setor_Coordenador] FROM [hspm_OS].[dbo].[Usuarios] where setor_Coordenador LIKE '%' + @Texto +'%' group by setor_Coordenador";
//                cmd.Parameters.AddWithValue("@Texto", prefixo);
//                cmd.Connection = conn;
//                conn.Open();
//                using (SqlDataReader sdr = cmd.ExecuteReader())
//                {
//                    while (sdr.Read())
//                    {
//                        clientes.Add(string.Format("{0}", sdr["setor_Coordenador"]));
//                    }
//                }
//                conn.Close();
//            }
//        }
//        return clientes.ToArray();
//    }
//    private void carreDadosCordenador(string login)
//    {
//        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hspm_OSConnectionString"].ToString()))
//        {
//            string query = @"SELECT RF 
//                              FROM [hspm_OS].[dbo].[Usuarios] WHERE LoginRede = @LoginRede";
//            SqlCommand cmd = new SqlCommand(query, con);
//            cmd.Parameters.AddWithValue("@LoginRede", login);
//            con.Open();
//            SqlDataReader reader = cmd.ExecuteReader();

//            if (reader.Read())
//            {
//                txtRF.Text = reader["RF"].ToString();
//                //txtSetor.Text = reader["setor_Coordenador"].ToString();
//                //txtRamal1.Text = reader["ramal_Coordenador"].ToString();
//            }
//            reader.Close();
//            con.Close();
//        }
//    }
//    private void limpaDados()
//    {
//        txtLogin.Text = "";
//        txtNome.Text = "";
//        txtEmail.Text = "";
//        txtRF.Text = "";
//        //txtSetor.Text = "";
//        select1.SelectedIndex = -1;
//        //txtRamal1.Text = "";
//        cblPerfis.ClearSelection();
//    }

//}


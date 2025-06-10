using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

/// <summary>
/// Summary description for PedidoDAO
/// </summary>
public class PedidoDAO
{
    public static int GravaPedidoConsulta(int _prontuario, string _nome_paciente, DateTime _data_pedido, int _cod_espec, string _exames, string _outras_info, string _solicitante, string _usuario)
    {
        string mensagem = "";
        string _dtcadastro = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        string _dtcadastro_bd = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        int _codPedido=0;
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;
            
            try
            {

                cmm.CommandText = "Insert into pedido_consulta (prontuario, nome_paciente, data_pedido, data_cadastro, cod_especialidade, exames_solicitados, outras_informacoes, solicitante, status, usuario)" +
                       "values (@prontuario, @nome_paciente, @data_pedido, @data_cadastro, @cod_especialidade, @exames_solicitados, @outras_informacoes, @solicitante, @status, @usuario)";

                cmm.Parameters.Add("@prontuario", SqlDbType.Int).Value = _prontuario;
                cmm.Parameters.Add("@nome_paciente", SqlDbType.VarChar).Value = _nome_paciente;
                cmm.Parameters.Add("@data_pedido", SqlDbType.DateTime).Value = _data_pedido;
                cmm.Parameters.Add("@data_cadastro", SqlDbType.DateTime).Value = _dtcadastro;
                cmm.Parameters.Add("@cod_especialidade", SqlDbType.Int).Value = _cod_espec;
                cmm.Parameters.Add("@exames_solicitados", SqlDbType.VarChar).Value = _exames;
                cmm.Parameters.Add("@outras_informacoes", SqlDbType.VarChar).Value = _outras_info;
                cmm.Parameters.Add("@solicitante", SqlDbType.VarChar).Value = _solicitante;
                cmm.Parameters.Add("@status", SqlDbType.Int).Value = 0;
                cmm.Parameters.Add("@usuario", SqlDbType.VarChar).Value = _usuario;
                
                cmm.ExecuteNonQuery();

                mt.Commit();
                mt.Dispose();
                cnn.Close();
                mensagem = "Cadastro realizado com sucesso!";

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                mensagem = error;
                try
                {
                    mt.Rollback();
                }
                catch (Exception ex1)
                { 
                    error = ex1.Message; 
                }
            }


        }
        
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();


            string sqlConsulta = "SELECT [cod_pedido]" +

                              " FROM [pedido_consulta] " +
                              " WHERE  [status] = 0 " +
                              " AND [data_cadastro] = '" + _dtcadastro_bd + "'";
                            

            cmm.CommandText = sqlConsulta;

            try
            {
                cnn.Open();
                SqlDataReader dr1 = cmm.ExecuteReader();

                //char[] ponto = { '.', ' ' };
                if  (dr1.Read())
                {
                    
                    Pedido p = new Pedido();
                    p.cod_pedido = dr1.GetInt32(0);
                    _codPedido = p.cod_pedido;



                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }


        }


        return _codPedido;
    }

    public static List<Pedido> getListaPedidoConsultaPendentePorRH(int _prontuario)
    {
        var listaPedidos = new List<Pedido>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();


            string sqlConsulta = "SELECT [cod_pedido]" +
                              ",[prontuario]" +
                              ",[nome_paciente]" +
                              ",[data_pedido]" +
                              ",[data_cadastro]" +
                              ",[cod_especialidade]" +
                              ",[exames_solicitados]" +
                              ",[outras_informacoes]" +
                              ",[solicitante]" +
                              ",[usuario]" +
                              " FROM [pedido_consulta] " +
                              " WHERE  [status] = 0" +
                              " AND [prontuario] = "+ _prontuario +
                              " ORDER BY data_pedido DESC";

            cmm.CommandText = sqlConsulta;

            try
            {
                cnn.Open();
                SqlDataReader dr1 = cmm.ExecuteReader();

                //char[] ponto = { '.', ' ' };
                while (dr1.Read())
                {
                    Especialidade espec = new Especialidade();
                    Pedido p = new Pedido();
                    p.cod_pedido = dr1.GetInt32(0);
                    p.prontuario = dr1.GetInt32(1);
                    p.nome_paciente = dr1.GetString(2);
                    p.lista_exames = obterListaDeExames(p.cod_pedido);
                    p.lista_ressonancia = obterListaDeRessonancia(p.cod_pedido);
                    p.data_pedido = dr1.GetDateTime(3);
                    p.data_cadastro = dr1.GetDateTime(4);
                    p.cod_especialidade = dr1.GetInt32(5);
                    p.descricao_espec = EspecialidadeDAO.getEspecialidade(p.cod_especialidade);
                    p.exames_solicitados = dr1.GetString(6);
                    p.outras_informacoes = dr1.GetString(7);
                    p.solicitante = dr1.GetString(8);
                    p.usuario = dr1.GetString(9);

                    listaPedidos.Add(p);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }


            return listaPedidos;
        }
    }

    public static List<Pedido> getListaPedidoConsultaPendente()
    {
        var listaPedidos = new List<Pedido>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();


            string sqlConsulta = "SELECT [cod_pedido]" +
                              ",[prontuario]" +
                              ",[nome_paciente]" +
                              ",[data_pedido]" +
                              ",[data_cadastro]" +
                              ",[cod_especialidade]" +
                              ",[exames_solicitados]" +
                              ",[outras_informacoes]" +
                              ",[solicitante]" +
                              ",[usuario]" +
                              " FROM [pedido_consulta] " +
                              " where status =0 ORDER BY cod_pedido DESC";

            cmm.CommandText = sqlConsulta;

            try
            {
                cnn.Open();
                SqlDataReader dr1 = cmm.ExecuteReader();

                //char[] ponto = { '.', ' ' };
                while (dr1.Read())
                {
                    Especialidade espec = new Especialidade();
                    Pedido p = new Pedido();
                    p.cod_pedido = dr1.GetInt32(0);
                    p.lista_exames = obterListaDeExames(p.cod_pedido);
                    p.lista_ressonancia = obterListaDeRessonancia(p.cod_pedido);
                    p.prontuario = dr1.GetInt32(1);
                    p.nome_paciente = dr1.GetString(2);
                    p.data_pedido = dr1.GetDateTime(3);
                    p.data_cadastro = dr1.GetDateTime(4);
                    p.cod_especialidade = dr1.GetInt32(5);
                    p.descricao_espec = EspecialidadeDAO.getEspecialidade(p.cod_especialidade);
                    p.exames_solicitados = dr1.GetString(6);
                    p.outras_informacoes = dr1.GetString(7);
                    p.solicitante = dr1.GetString(8);
                    p.usuario = dr1.GetString(9);

                    listaPedidos.Add(p);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }


            return listaPedidos;
        }
    }
    public static List<Pedido> getListaPedidoConsultaArquivados()
    {
        var listaPedidos = new List<Pedido>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();


            string sqlConsulta = "SELECT [cod_pedido]" +
                              ",[prontuario]" +
                              ",[nome_paciente]" +
                              ",[data_pedido]" +
                              ",[data_cadastro]" +
                              ",[cod_especialidade]" +
                              ",[exames_solicitados]" +
                              ",[outras_informacoes]" +
                              ",[solicitante]" +
                              ",[usuario]" +
                              ",[usuario_baixa]" +
                              " FROM [pedido_consulta] " +
                              " where status = 2 ORDER BY cod_pedido DESC";

            cmm.CommandText = sqlConsulta;

            try
            {
                cnn.Open();
                SqlDataReader dr1 = cmm.ExecuteReader();

                //char[] ponto = { '.', ' ' };
                while (dr1.Read())
                {
                    Especialidade espec = new Especialidade();
                    Pedido p = new Pedido();
                    p.cod_pedido = dr1.GetInt32(0);
                    p.lista_exames = obterListaDeExames(p.cod_pedido);
                    p.lista_ressonancia = obterListaDeRessonancia(p.cod_pedido);
                    p.prontuario = dr1.GetInt32(1);
                    p.nome_paciente = dr1.GetString(2);
                    p.data_pedido = dr1.GetDateTime(3);
                    p.data_cadastro = dr1.GetDateTime(4);
                    p.cod_especialidade = dr1.GetInt32(5);
                    p.descricao_espec = EspecialidadeDAO.getEspecialidade(p.cod_especialidade);
                    p.exames_solicitados = dr1.GetString(6);
                    p.outras_informacoes = dr1.GetString(7);
                    p.solicitante = dr1.GetString(8);
                    p.usuario = dr1.GetString(9);
                    p.usuario_baixa = dr1.GetString(10);

                    listaPedidos.Add(p);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }


            return listaPedidos;
        }
    }

    private static string obterListaDeExames(int cod_pedido)
    {
        var listaEspec = new List<ExameUnico>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT e.cod_exames_unico, descricao_exames_unico " +
                             " FROM[hspmAtendimento].[dbo].[exame] e join[hspmAtendimento].[dbo].[pedido_exame] pe on e.cod_exames_unico = pe.cod_exames_unico " +
                             "  where status = 'A' and cod_pedido = " + cod_pedido;





            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    ExameUnico exm = new ExameUnico();
                    exm.cod_exames_unico = dr1.GetInt32(0);
                    exm.descricao_exames_unico = dr1.GetString(1);

                    listaEspec.Add(exm);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        string list = string.Join(", ", listaEspec.Select(c => c.descricao_exames_unico.ToString()).ToArray<string>());

        return list;
    }
    private static string obterListaDeRessonancia(int cod_pedido)
    {
        var listaEspec = new List<Ressonancia>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT e.cod_ressonancia, descricao_ressonancia " +
                             " FROM[hspmAtendimento].[dbo].[ressonancia] e join[hspmAtendimento].[dbo].[pedido_ressonancia] pe on e.cod_ressonancia = pe.cod_ressonancia " +
                             "  where status = 'A' and cod_pedido = " + cod_pedido;





            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    Ressonancia ress = new Ressonancia();
                    ress.cod_ressonancia = dr1.GetInt32(0);
                    ress.descricao_ressonancia = dr1.GetString(1);

                    listaEspec.Add(ress);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        string list = string.Join(", ", listaEspec.Select(c => c.descricao_ressonancia.ToString()).ToArray<string>());

        return list;
    }

    public static Pedido getPedidoConsulta(int _idPedido)
    {
        Especialidade espec = new Especialidade();
        Pedido pedido = new Pedido();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();


            string sqlConsulta = "SELECT [cod_pedido]" +
                              ",[prontuario]" +
                              ",[nome_paciente]" +
                              ",[data_pedido]" +
                              ",[data_cadastro]" +
                              ",[cod_especialidade]" +
                              ",[exames_solicitados]" +
                              ",[outras_informacoes]" +
                              ",[solicitante]" +
                              ",[usuario]" +
                              ",[status]" +
                              " FROM [pedido_consulta] " +
                              " WHERE  cod_pedido = " + _idPedido;

            cmm.CommandText = sqlConsulta;

            try
            {
                cnn.Open();
                SqlDataReader dr1 = cmm.ExecuteReader();
                if (dr1.Read())
                {
                    pedido.cod_pedido = dr1.GetInt32(0);
                    pedido.prontuario = dr1.GetInt32(1);

                    pedido.nome_paciente = dr1.GetString(2);
                    pedido.data_pedido = dr1.GetDateTime(3);
                    pedido.data_cadastro = dr1.GetDateTime(4);
                    pedido.cod_especialidade = dr1.GetInt32(5);
                    pedido.descricao_espec = EspecialidadeDAO.getEspecialidade(pedido.cod_especialidade);
                    pedido.exames_solicitados = dr1.GetString(6);
                    pedido.outras_informacoes = dr1.GetString(7);
                    pedido.solicitante = dr1.GetString(8);
                    pedido.usuario = dr1.GetString(9);
                    pedido.status_pedido = dr1.GetInt32(10);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return pedido;
        }

    }

    public static string AtualizaPedido(string outras_informacoes, int _idPedido)
    {
        string msg = "";
        string usuario = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;
            
            try
            {



              

                // Atualiza tabela de pedido de consulta
                cmm.CommandText = "UPDATE pedido_consulta" +
                        " SET outras_informacoes = @outras_informacoes " +
                        " WHERE  cod_pedido = @cod_ped";
                cmm.Parameters.Add(new SqlParameter("@cod_ped", _idPedido));
                cmm.Parameters.Add(new SqlParameter("@outras_informacoes", outras_informacoes));
                cmm.ExecuteNonQuery();

                mt.Commit();
                mt.Dispose();
                cnn.Close();

               LogDAO.gravaLog("UPDATE: CÓDIGO PEDIDO " + _idPedido, "CAMPO OUTRAS_INFORMACOES",  usuario);
                msg = "Cadastro realizado com sucesso!";

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                msg = error;
                try
                {
                    mt.Rollback();
                }
                catch (Exception ex1)
                { }
            }
        }

        return msg;
    }





















    public static void gravaLog(string descript_log, string origem, string usuario)
    {

        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;

            try
            {

                cmm.CommandText = "Insert into log (description_log, origem, usuario, dt_gravacao)" +
                       "values (@description_log, @origem, @usuario, @dt_gravacao)";

                cmm.Parameters.Add("@description_log", SqlDbType.VarChar).Value = descript_log;
                cmm.Parameters.Add("@origem", SqlDbType.VarChar).Value = origem;
                cmm.Parameters.Add("@usuario", SqlDbType.VarChar).Value = usuario;
                cmm.Parameters.Add("@dt_gravacao", SqlDbType.DateTime).Value = DateTime.Now;


                cmm.ExecuteNonQuery();

                mt.Commit();
                mt.Dispose();
                cnn.Close();


            }
            catch (Exception ex)
            {
                string error = ex.Message;
                try
                {
                    mt.Rollback();
                }
                catch (Exception ex1)
                { }
            }
        }
    }

    public static void  deletePedidodeConsulta(int _idPedido)
    {

        string msg = "";
        string usuario = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();
        int _status = 1;
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;

            try
            {





                // Atualiza tabela de pedido de consulta
                cmm.CommandText = "UPDATE pedido_consulta" +
                        " SET status = @status " +
                        " WHERE  cod_pedido = @cod_ped";
                cmm.Parameters.Add(new SqlParameter("@cod_ped", _idPedido));
                cmm.Parameters.Add(new SqlParameter("@status", _status));
                cmm.ExecuteNonQuery();

                mt.Commit();
                mt.Dispose();
                cnn.Close();

                LogDAO.gravaLog("DELETE: CÓDIGO PEDIDO " + _idPedido, "CAMPO STATUS", usuario);
                msg = "Cadastro realizado com sucesso!";

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                msg = error;
                try
                {
                    mt.Rollback();
                }
                catch (Exception ex1)
                { }
            }
        }

    
    }
    public static void filePedidodeConsulta(int _idPedido)
    {
        string _dtbaixa = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

        string msg = "";
        string usuario_baixa = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();
        int _status = 2;
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;

            try
            {





                // Atualiza tabela de pedido de consulta
                cmm.CommandText = "UPDATE pedido_consulta" +
                        " SET status = @status , data_baixa = @data_baixa, usuario_baixa = @usuario_baixa" +
                        " WHERE  cod_pedido = @cod_ped";
                cmm.Parameters.Add(new SqlParameter("@cod_ped", _idPedido));
                cmm.Parameters.Add(new SqlParameter("@status", _status));
                cmm.Parameters.Add(new SqlParameter("@data_baixa", _dtbaixa));
                cmm.Parameters.Add(new SqlParameter("@usuario_baixa", usuario_baixa));
                cmm.ExecuteNonQuery();

                mt.Commit();
                mt.Dispose();
                cnn.Close();

                LogDAO.gravaLog("ARQUIVADO: CÓDIGO PEDIDO " + _idPedido, "CAMPO STATUS", usuario_baixa);
                msg = "Cadastro realizado com sucesso!";

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                msg = error;
                try
                {
                    mt.Rollback();
                }
                catch (Exception ex1)
                { }
            }
        }


    }

}

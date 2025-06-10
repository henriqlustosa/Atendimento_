using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

/// <summary>
/// Summary description for ExamesUnicosDAO
/// </summary>
public class PreOperatorioDAO
{
    public PreOperatorioDAO()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void AtualizaExamesPorPedidos(List<PreOperatorio> preoperatorios, int _cod_pedido)
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

                cmm.CommandText = "UPDATE pedido_pre_operatorio" +
                 " SET status = @status " +
                 " WHERE  cod_pedido = " + _cod_pedido ;
                cmm.Parameters.Add(new SqlParameter("@status", "I"));

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


        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            string status = "A";
            string _dtcadastro_bd = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;
            try
            {

                foreach (PreOperatorio preoperatorio in preoperatorios)
                {
                    cmm.CommandText = "Insert into pedido_pre_operatorio ([cod_pre_operatorio], cod_pedido,data_cadastro,status)"
                    + " values ('"
                                + preoperatorio.cod_pre_operatorio + "','"
                                + _cod_pedido + "','"
                                + _dtcadastro_bd + "','"
                                + status
                                + "');";
                    cmm.ExecuteNonQuery();


                }

                mt.Commit();
                mt.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                mt.Rollback();
            }
        }

    }

    public static void GravaExamesPorPedidos(List<PreOperatorio> preoperatorios, int _cod_pedido)
    {
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            string status = "A";
            string _dtcadastro_bd = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;
            try
            {

                foreach (PreOperatorio preoperatorio in preoperatorios)
                {
                    cmm.CommandText = "Insert into [pedido_pre_operatorio]([cod_pedido], [cod_pre_operatorio],[data_cadastro],[status])"
                    + " values ('"
                    + _cod_pedido + "','"
                                + preoperatorio.cod_pre_operatorio + "','"
                               
                                + _dtcadastro_bd + "','"
                                + status 
                                + "');";
                    cmm.ExecuteNonQuery();


                }

                mt.Commit();
                mt.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                mt.Rollback();
            }
        }

       









    }

    public static List<PreOperatorio> listaExame()
    {
        var lista = new List<PreOperatorio>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT cod_pre_operatorio, descricao_pre_operatorio, status_pre_operatorio " +
                             " FROM [hspmAtendimento].[dbo].[pre_operatorio]  " +
                             " ORDER BY cod_pre_operatorio";

            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    PreOperatorio preOperatorio = new PreOperatorio();
                    preOperatorio.cod_pre_operatorio = dr1.GetInt32(0);
                    preOperatorio.descricao_pre_operatorio = dr1.GetString(1);
                    preOperatorio.status_pre_operatorio = dr1.GetString(2);
                    lista.Add(preOperatorio);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

        }
        return lista;
    }

    public static List<PreOperatorio> ObterListaDeExamesEscolhidos(int idPedido)
    {
        var lista = new List<PreOperatorio>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT e.cod_pre_operatorio, descricao_pre_operatorio " +
                             " FROM [hspmAtendimento].[dbo].[pre_operatorio] e join [hspmAtendimento].[dbo].[pedido_pre_operatorio] pe on e.[cod_pre_operatorio] = pe.[cod_pre_operatorio] " +
                             "  where status = 'A' and cod_pedido = "+ idPedido;
            
                           
                            


            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    PreOperatorio preOperatorio = new PreOperatorio();
                    preOperatorio.cod_pre_operatorio = dr1.GetInt32(0);
                    preOperatorio.descricao_pre_operatorio = dr1.GetString(1);
                 
                    lista.Add(preOperatorio);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

        }
        return lista;

    }
}
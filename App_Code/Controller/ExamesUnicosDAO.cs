using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

/// <summary>
/// Summary description for ExameDAO
/// </summary>
public class ExamesUnicosDAO
{
    public ExamesUnicosDAO()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void AtualizaExamesPorPedidos(List<ExameUnico> exames, int _cod_pedido)
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

                cmm.CommandText = "UPDATE pedido_exames_unico" +
                 " SET status_exames_unico = @status " +
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

                foreach (ExameUnico exame in exames)
                {
                    cmm.CommandText = "Insert into pedido_exame (cod_exame, cod_pedido,data_cadastro,status)"
                    + " values ('"
                                + exame.cod_exames_unico + "','"
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

    public static void GravaExamesPorPedidos(List<ExameUnico> exames, int _cod_pedido)
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

                foreach (ExameUnico exame in exames)
                {
                    cmm.CommandText = "Insert into pedido_exame (cod_exame, cod_pedido,data_cadastro,status)"
                    + " values ('"
                                + exame.cod_exames_unico + "','"
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

    public static List<ExameUnico> listaExame()
    {
        var listaEspec = new List<ExameUnico>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT cod_exame, descricao_exame, status_exame " +
                             " FROM [hspmAtendimento].[dbo].[exames_unico] " +
                             " ORDER BY cod_exame";

            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    ExameUnico exm = new ExameUnico();
                    exm.cod_exames_unico = dr1.GetInt32(0);
                    exm.descricao_exames_unico = dr1.GetString(1);
                    exm.status_exames_unico = dr1.GetString(2);
                    listaEspec.Add(exm);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

        }
        return listaEspec;
    }

    public static List<ExameUnico> ObterListaDeExamesEscolhidos(int idPedido)
    {
        var listaEspec = new List<ExameUnico>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT e.cod_exame, descricao_exame " +
                             " FROM[hspmAtendimento].[dbo].[exame_unico] e join[hspmAtendimento].[dbo].[pedido_exames_unico] pe on e.cod_exames_unico = pe.cod_exame " +
                             "  where status = 'A' and cod_pedido = "+ idPedido;
            
                           
                            


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
        return listaEspec;

    }
}
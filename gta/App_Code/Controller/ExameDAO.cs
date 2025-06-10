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

                cmm.CommandText = "UPDATE pedido_exame" +
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

                foreach (ExameUnicoexame in exames)
                {
                    cmm.CommandText = "Insert into pedido_exame (cod_exames_unico, cod_pedido,data_cadastro,status)"
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

                foreach (ExameUnicoexame in exames)
                {
                    cmm.CommandText = "Insert into pedido_exame (cod_exames_unico, cod_pedido,data_cadastro,status)"
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
            cmm.CommandText = "SELECT cod_exames_unico, descricao_exames_unico, status_exame " +
                             " FROM [hspmAtendimento].[dbo].[exame] " +
                             " ORDER BY cod_exames_unico";

            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    ExameUnicoexm = new Exame();
                    exm.cod_exames_unico = dr1.GetInt32(0);
                    exm.descricao_exames_unico = dr1.GetString(1);
                    exm.status_exame = dr1.GetString(2);
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
            cmm.CommandText = "SELECT e.cod_exames_unico, descricao_exames_unico " +
                             " FROM[hspmAtendimento].[dbo].[exame] e join[hspmAtendimento].[dbo].[pedido_exame] pe on e.cod_exames_unico = pe.cod_exames_unico " +
                             "  where status = 'A' and cod_pedido = "+ idPedido;
            
                           
                            


            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    ExameUnicoexm = new Exame();
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
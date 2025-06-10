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
public class TeleConsultaDAO
{
    public TeleConsultaDAO()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void AtualizaExamesPorPedidos(List<TeleConsulta> teleconsultas, int _cod_pedido)
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

                cmm.CommandText = "UPDATE pedido_teleconsulta" +
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

                foreach (TeleConsulta teleconsulta in teleconsultas)
                {
                    cmm.CommandText = "Insert into pedido_teleconsulta (cod_teleconsulta, cod_pedido,data_cadastro,status)"
                    + " values ('"
                                + teleconsulta.cod_teleconsulta + "','"
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

    public static void GravaExamesPorPedidos(List<TeleConsulta> teleconsultas, int _cod_pedido)
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

                foreach (TeleConsulta teleconsulta in teleconsultas)
                {
                    cmm.CommandText = "Insert into pedido_teleconsulta (cod_teleconsulta, cod_pedido,data_cadastro,status)"
                    + " values ('"
                                + teleconsulta.cod_teleconsulta + "','"
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

    public static List<TeleConsulta> listaExame()
    {
        var listaEspec = new List<TeleConsulta>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT cod_teleconsulta, descricao_teleconsulta, status_teleconsulta" +
                             " FROM [hspmAtendimento].[dbo].[teleconsulta] " +
                             " ORDER BY cod_teleconsulta";

            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    TeleConsulta teleconsulta = new TeleConsulta();
                    teleconsulta.cod_teleconsulta = dr1.GetInt32(0);
                    teleconsulta.descricao_teleconsulta = dr1.GetString(1);
                    teleconsulta.status_teleconsulta = dr1.GetString(2);
                    listaEspec.Add(teleconsulta);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

        }
        return listaEspec;
    }

    public static List<TeleConsulta> ObterListaDeExamesEscolhidos(int idPedido)
    {
        var listaEspec = new List<TeleConsulta>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT e.cod_exames_unico, descricao_exames_unico " +
                             " FROM[hspmAtendimento].[dbo].[teleconsulta] e join[hspmAtendimento].[dbo].[pedido_teleconsulta] pe on e.cod_exames_unico = pe.cod_exames_unico " +
                             "  where status = 'A' and cod_pedido = "+ idPedido;
            
                           
                            


            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    TeleConsulta teleconsulta = new TeleConsulta();
                    teleconsulta.cod_teleconsulta = dr1.GetInt32(0);
                    teleconsulta.descricao_teleconsulta = dr1.GetString(1);
                 
                    listaEspec.Add(teleconsulta);
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
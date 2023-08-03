using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for RessonanciaDAO
/// </summary>
public class RessonanciaDAO
{
	public RessonanciaDAO()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static void AtualizaRessonanciaPorPedidos(List<Ressonancia> ressonancias, int _cod_pedido)
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

                cmm.CommandText = "UPDATE pedido_ressonancia" +
                 " SET status = @status " +
                 " WHERE  cod_pedido = " + _cod_pedido;
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

                foreach (Ressonancia ressonancia in ressonancias)
                {
                    cmm.CommandText = "Insert into pedido_ressonancia (cod_ressonancia, cod_pedido,data_cadastro,status)"
                    + " values ('"
                                + ressonancia.cod_ressonancia + "','"
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

    public static void GravaRessonanciaPorPedidos(List<Ressonancia> ressonancias, int _cod_pedido)
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

                foreach (Ressonancia ressonancia in ressonancias)
                {
                    cmm.CommandText = "Insert into pedido_ressonancia (cod_ressonancia, cod_pedido,data_cadastro,status)"
                    + " values ('"
                                + ressonancia.cod_ressonancia + "','"
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

    public static List<Ressonancia> listaRessonancia()
    {
        var listaRessonancia = new List<Ressonancia>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT cod_ressonancia, descricao_ressonancia, status_ressonancia " +
                             " FROM [hspmAtendimento_Call].[dbo].[ressonancia] " +
                             " ORDER BY cod_ressonancia";

            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    Ressonancia ress = new Ressonancia();
                    ress.cod_ressonancia = dr1.GetInt32(0);
                    ress.descricao_ressonancia = dr1.GetString(1);
                    ress.status_ressonancia = dr1.GetString(2);
                    listaRessonancia.Add(ress);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

        }
        return listaRessonancia;
    }

    public static List<Ressonancia> ObterListaDeRessonanciasEscolhidos(int idPedido)
    {
        var listaRessonancia = new List<Ressonancia>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT e.cod_ressonancia, descricao_ressonancia " +
                             " FROM[hspmAtendimento_Call].[dbo].[ressonancia] e join[hspmAtendimento_Call].[dbo].[pedido_ressonancia] pe on e.cod_ressonancia = pe.cod_ressonancia " +
                             "  where status = 'A' and cod_pedido = " + idPedido;





            try
            {
                cnn.Open();

                SqlDataReader dr1 = cmm.ExecuteReader();

                while (dr1.Read())
                {
                    Ressonancia ress = new Ressonancia();
                    ress.cod_ressonancia = dr1.GetInt32(0);
                    ress.descricao_ressonancia = dr1.GetString(1);

                    listaRessonancia.Add(ress);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

        }
        return listaRessonancia;

    }
}

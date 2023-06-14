using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ConsultasRemarcarDAO
/// </summary>
public class ConsultasRemarcarDAO
{
	public ConsultasRemarcarDAO()
	{
		
	}

    public static String GravaInformacaoConCancelada(int _id_cancelada, string _informacao, string _usuario)
    {
        string mensagem = "";
        
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cnn;
            cnn.Open();
            SqlTransaction mt = cnn.BeginTransaction();
            cmm.Transaction = mt;
            try
            {
                cmm.CommandText = "Insert into info_con_cancelada " +
                    "(descricao_con_cancelada, id_consultas_cancelar, usuario, data_informacao)" +
                    "values (@informacao, @id_cancelada, @usuario, @data_informacao)";


                cmm.Parameters.Add("@informacao", SqlDbType.VarChar).Value = _informacao.ToUpper();
                cmm.Parameters.Add("@id_cancelada", SqlDbType.Int).Value = _id_cancelada;
                cmm.Parameters.Add("@usuario", SqlDbType.VarChar).Value = _usuario.ToUpper();
                cmm.Parameters.Add("@data_informacao", SqlDbType.DateTime).Value = DateTime.Now;
                cmm.ExecuteNonQuery();

                mt.Commit();
                mt.Dispose();
                cnn.Close();
                AtualizaConCancelada(_id_cancelada);

                mensagem = "Informação gravada!";
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
                { }
            }
            return mensagem;
        }
    }

    private static void AtualizaConCancelada(int _idCancela)
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
                cmm.CommandText = "UPDATE consultas_cancelar" +
                     " SET stat_cancelar = 1" +
                     " WHERE  id_cancela = @id_cancelada";
                cmm.Parameters.Add(new SqlParameter("@id_cancelada", _idCancela));
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
}
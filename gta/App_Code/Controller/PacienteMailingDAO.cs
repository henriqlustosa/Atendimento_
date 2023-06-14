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
/// Summary description for PacienteMailingDAO
/// </summary>
public class PacienteMailingDAO
{
	public PacienteMailingDAO()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static bool getExisteCadastroPacienteMailing(int _prontuario)
    {
        bool exite = false;
        PacienteMailling paciente = new PacienteMailling();

        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT * FROM paciente_Mailling WHERE prontuario = " + _prontuario;

            cnn.Open();
            SqlDataReader dr = cmm.ExecuteReader();
            if (dr.Read())
            {
                exite = true;
            }
        }
        return exite;
    }

    public static PacienteMailling getDadosPaciente(int _prontuario)
    {
        PacienteMailling paciente = new PacienteMailling();

        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();
            cmm.CommandText = "SELECT nome_paciente, telefone1, telefone2, telefone3, telefone4 FROM paciente_Mailling WHERE prontuario = " + _prontuario;

            cnn.Open();
            SqlDataReader dr = cmm.ExecuteReader();
            if (dr.Read())
            {
                paciente.Nome = dr.GetString(0);
                paciente.Telefone1 = dr.GetString(1);
                paciente.Telefone2 = dr.GetString(2);
                paciente.Telefone3 = dr.GetString(3);
                paciente.Telefone4 = dr.GetString(4);
            }
        }
        return paciente;
    }

    public static string AtualizaTelefones(int _prontuario ,string _tel1, string _tel2, string _tel3, string _tel4)
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
                
                    cmm.CommandText = "UPDATE paciente_Mailling" +
                     " SET telefone1 = @tel1 " +
                     ", telefone2 = @tel2 " +
                     ", telefone3 = @tel3 " +
                     ", telefone4 = @tel4 " +
                     " WHERE  prontuario = @prontuario";
                    cmm.Parameters.Add(new SqlParameter("@prontuario", _prontuario));
                    cmm.Parameters.Add(new SqlParameter("@tel1", _tel1));
                    cmm.Parameters.Add(new SqlParameter("@tel2", _tel2));
                    cmm.Parameters.Add(new SqlParameter("@tel3", _tel3));
                    cmm.Parameters.Add(new SqlParameter("@tel4", _tel4));
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
                { }
            }
        }
        return mensagem;
    }
}

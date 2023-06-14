using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LogDAO
/// </summary>
public class LogDAO
{
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
}
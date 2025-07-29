using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

public static class ControleAcessoDb
{
    public static List<string> ObterPaginasPermitidas(List<int> perfis)
    {
        List<string> urls = new List<string>();

        if (perfis == null || perfis.Count == 0)
            return urls;

        // Converte lista de inteiros para string: "1,2,3"
        string perfisSql = string.Join(",", perfis.Select(p => p.ToString()).ToArray());


        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            con.Open();
            string sql = string.Format(@"
    SELECT DISTINCT p.Url
    FROM PerfisPaginas pp
    INNER JOIN Paginas p ON pp.PaginaId = p.Id
    WHERE pp.PerfilId IN ({0})", perfisSql);


            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    urls.Add(reader.GetString(0));
                }
            }
        }

        return urls;
    }

    public static bool UsuarioTemAcesso(string url, List<int> perfis)
    {
        var paginas = ObterPaginasPermitidas(perfis);

        foreach (var prefixo in paginas)
        {
            if (url.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}

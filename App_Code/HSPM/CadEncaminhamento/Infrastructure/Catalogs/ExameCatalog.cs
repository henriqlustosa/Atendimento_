using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Hspm.CadEncaminhamento.Domain;

namespace Hspm.CadEncaminhamento.Infrastructure
{
    public sealed class ExameCatalog : IExameCatalog
    {
        private readonly IConnectionFactory _factory;

        public ExameCatalog(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public IList<ListItemDto> ListarPreOperatorio()
        {
            return Query("SELECT cod_pre_operatorio AS Value, descricao_pre_operatorio AS Text FROM pre_operatorio WHERE [status_pre_operatorio]='A'ORDER BY Text;");
        }

        public IList<ListItemDto> ListarRessonancia()
        {
            return Query("SELECT cod_ressonancia AS Value, descricao_ressonancia AS Text FROM ressonancia WHERE [status_ressonancia]='A'ORDER BY Text;");
        }

        public IList<ListItemDto> ListarTeleconsulta()
        {
            return Query("SELECT cod_teleconsulta AS Value, descricao_teleconsulta AS Text FROM teleconsulta WHERE [status_teleconsulta]='A'ORDER BY Text;");
        }

        public IList<ListItemDto> ListarExamesUnicos()
        {
            return Query("SELECT cod_exames_unico AS Value, descricao_exames_unico AS Text FROM [exames_unico] WHERE [status_exames_unico]='A' ORDER BY Text;");
        }

        private IList<ListItemDto> Query(string sql)
        {
            IList<ListItemDto> list = new List<ListItemDto>();

            using (SqlConnection conn = _factory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string text = rd["Text"] as string;
                        int val = Convert.ToInt32(rd["Value"]);
                        if (string.IsNullOrEmpty(text)) continue;

                        bool exists = false;
                        foreach (ListItemDto it in list)
                        {
                            if (string.Equals(it.Text, text, StringComparison.InvariantCultureIgnoreCase))
                            {
                                exists = true; break;
                            }
                        }
                        if (exists) continue;

                        ListItemDto dto = new ListItemDto();
                        dto.Text = text;
                        dto.Value = val;
                        list.Add(dto);
                    }
                }
            }

            return list;
        }
    }
}

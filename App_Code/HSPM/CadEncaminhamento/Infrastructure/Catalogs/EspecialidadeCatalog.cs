using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Hspm.CadEncaminhamento.Domain;

namespace Hspm.CadEncaminhamento.Infrastructure
{
    public sealed class EspecialidadeCatalog : IEspecialidadeCatalog
    {
        private readonly IConnectionFactory _factory;

        public EspecialidadeCatalog(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public IList<ListItemDto> Listar()
        {
            IList<ListItemDto> list = new List<ListItemDto>();

            using (SqlConnection conn = _factory.Create())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT cod_especialidade, descricao_especialidade FROM especialidade WHERE [status_especialidade] = 'A' ORDER BY descricao_especialidade;", conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string text = rd["descricao_especialidade"] as string;
                        int val = Convert.ToInt32(rd["cod_especialidade"]);
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

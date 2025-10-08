using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

public class TipoMeta
{
    public string Categoria;        // "exames_unico" | "pre_operatorio" | "ressonancia" | "teleconsulta"
    public string Tabela;
    public string ColId;
    public string ColDescricao;
    public string ColStatus;
    public bool StatusChar1;        // true: CHAR(1) ("A"/"I") ; false: NVARCHAR(50) ("Ativo"/"Inativo")

    public static TipoMeta FromCategoria(string cat)
    {
        switch ((cat ?? string.Empty).ToLower())
        {
            case "pre_operatorio":
                return new TipoMeta
                {
                    Categoria = "pre_operatorio",
                    Tabela = "dbo.pre_operatorio",
                    ColId = "cod_pre_operatorio",
                    ColDescricao = "descricao_pre_operatorio",
                    ColStatus = "status_pre_operatorio",
                    StatusChar1 = true
                };
            case "ressonancia":
                return new TipoMeta
                {
                    Categoria = "ressonancia",
                    Tabela = "dbo.ressonancia",
                    ColId = "cod_ressonancia",
                    ColDescricao = "descricao_ressonancia",
                    ColStatus = "status_ressonancia",
                    StatusChar1 = true
                };
            case "teleconsulta":
                return new TipoMeta
                {
                    Categoria = "teleconsulta",
                    Tabela = "dbo.teleconsulta",
                    ColId = "cod_teleconsulta",
                    ColDescricao = "descricao_teleconsulta",
                    ColStatus = "status_teleconsulta",
                    StatusChar1 = true
                };
            default:
                return new TipoMeta
                {
                    Categoria = "exames_unico",
                    Tabela = "dbo.exames_unico",
                    ColId = "cod_exames_unico",
                    ColDescricao = "descricao_exames_unico",
                    ColStatus = "status_exames_unico",
                    StatusChar1 = true
                };
        }
    }

    public string ToDbStatus(string statusPadrao) // "A"|"I" -> valor da coluna
    {
        if (StatusChar1) return statusPadrao;                 // mantém "A"/"I"
        return statusPadrao == "A" ? "Ativo" : "Inativo";     // NVARCHAR(50)
    }

    public string ToPadraoStatus(object dbValue) // coluna -> "A"/"I"
    {
        if (dbValue == null || dbValue is DBNull) return "A";
        string s = Convert.ToString(dbValue).Trim();
        if (StatusChar1) return (s == "A") ? "A" : "I";
        return s.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ? "A" : "I";
    }
}

public class TipoRegistro
{
    public int Id;
    public string Descricao;
    public string StatusPadrao; // "A" ou "I"
}

public static class TiposExameDAO
{
    private static readonly string CS =
        ConfigurationManager.ConnectionStrings["gtaConnectionString"].ConnectionString;

    public static List<TipoRegistro> Listar(TipoMeta m)
    {
        string sql = string.Format(
            "SELECT {0} AS Id, {1} AS Descricao, {2} AS StatusCol FROM {3} WITH(NOLOCK) ORDER BY {1}",
            m.ColId, m.ColDescricao, m.ColStatus, m.Tabela);

        var list = new List<TipoRegistro>();
        using (var conn = new SqlConnection(CS))
        using (var cmd = new SqlCommand(sql, conn))
        {
            conn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new TipoRegistro
                    {
                        Id = Convert.ToInt32(rd["Id"]),
                        Descricao = Convert.ToString(rd["Descricao"]),
                        StatusPadrao = m.ToPadraoStatus(rd["StatusCol"])
                    });
                }
            }
        }
        return list;
    }

    public static TipoRegistro Obter(TipoMeta m, int id)
    {
        string sql = string.Format(
            "SELECT {0} AS Id, {1} AS Descricao, {2} AS StatusCol FROM {3} WITH(NOLOCK) WHERE {0}=@id",
            m.ColId, m.ColDescricao, m.ColStatus, m.Tabela);

        using (var conn = new SqlConnection(CS))
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                if (!rd.Read()) return null;

                return new TipoRegistro
                {
                    Id = Convert.ToInt32(rd["Id"]),
                    Descricao = Convert.ToString(rd["Descricao"]),
                    StatusPadrao = m.ToPadraoStatus(rd["StatusCol"])
                };
            }
        }
    }

    public static void Inserir(TipoMeta m, string descricao, string statusPadrao)
    {
        string sql = string.Format(
            "INSERT INTO {0}({1}, {2}) VALUES(@d, @s)",
            m.Tabela, m.ColDescricao, m.ColStatus);

        using (var conn = new SqlConnection(CS))
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@d", descricao);
            cmd.Parameters.AddWithValue("@s", m.ToDbStatus(statusPadrao));
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static void Atualizar(TipoMeta m, int id, string descricao, string statusPadrao)
    {
        string sql = string.Format(
            "UPDATE {0} SET {1}=@d, {2}=@s WHERE {3}=@id",
            m.Tabela, m.ColDescricao, m.ColStatus, m.ColId);

        using (var conn = new SqlConnection(CS))
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@d", descricao);
            cmd.Parameters.AddWithValue("@s", m.ToDbStatus(statusPadrao));
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static void Excluir(TipoMeta m, int id)
    {
        string sql = string.Format(
            "DELETE FROM {0} WHERE {1}=@id",
            m.Tabela, m.ColId);

        using (var conn = new SqlConnection(CS))
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}

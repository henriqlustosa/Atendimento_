using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

public class PedidoDAO
{
    // =========================
    // Helpers (compatíveis com C# 3)
    // =========================
    private static string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString(); }
    }

    private static SqlConnection GetConn()
    {
        return new SqlConnection(ConnStr);
    }

    private static DataTable ExecuteDataTable(SqlCommand cmd)
    {
        var dt = new DataTable();
        using (var da = new SqlDataAdapter(cmd))
        {
            if (cmd.Connection.State != ConnectionState.Open)
                cmd.Connection.Open();
            da.Fill(dt);
        }
        return dt;
    }

    private static void Add(SqlCommand cmd, string name, SqlDbType type, object value)
    {
        var p = cmd.Parameters.Add(name, type);
        p.Value = (value ?? DBNull.Value);
    }

    private static void EnsureDescricaoEspecColumn(DataTable dt)
    {
        if (dt == null) return;
        if (!dt.Columns.Contains("descricao_espec"))
            dt.Columns.Add("descricao_espec", typeof(string));

        foreach (DataRow row in dt.Rows)
        {
            object oCod = (dt.Columns.Contains("cod_especialidade") ? row["cod_especialidade"] : null);
            if (oCod != null && oCod != DBNull.Value)
            {
                int cod = Convert.ToInt32(oCod);
                row["descricao_espec"] = EspecialidadeDAO.getEspecialidade(cod);
            }
        }
    }

    private static string CoerceSort(string sortExpression)
    {
        if (string.IsNullOrEmpty(sortExpression)) return string.Empty;

        string[] allowed =
        {
            "cod_pedido", "prontuario", "nome_paciente",
            "data_pedido", "data_cadastro",
            "cod_especialidade", "descricao_espec",
            "exames_solicitados", "outras_informacoes",
            "carga_geral", "usuario", "usuario_baixa"
        };

        string sort = sortExpression.Trim();
        string[] parts = sort.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string col = parts.Length > 0 ? parts[0] : "";
        string dir = parts.Length > 1 ? parts[1] : "";

        bool okCol = false;
        foreach (string a in allowed)
        {
            if (string.Equals(a, col, StringComparison.OrdinalIgnoreCase))
            {
                okCol = true; break;
            }
        }
        if (!okCol) return string.Empty;

        dir = (string.Equals(dir, "DESC", StringComparison.OrdinalIgnoreCase)) ? "DESC" : "ASC";
        return col + " " + dir;
    }

    // =========================
    // INSERT (retorna cod_pedido)
    // =========================
    public static int GravaPedidoConsulta(
        int prontuario, string nome_paciente, DateTime data_pedido, int cod_espec,
        string exames, string outras_info, string solicitante, string usuario)
    {
        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
INSERT INTO pedido_consulta
(prontuario, nome_paciente, data_pedido, data_cadastro, cod_especialidade,
 exames_solicitados, outras_informacoes, solicitante, status, usuario)
OUTPUT INSERTED.cod_pedido
VALUES (@prontuario, @nome_paciente, @data_pedido, @data_cadastro, @cod_especialidade,
        @exames_solicitados, @outras_informacoes, @solicitante, @status, @usuario);";

            Add(cmd, "@prontuario", SqlDbType.Int, prontuario);
            Add(cmd, "@nome_paciente", SqlDbType.VarChar, nome_paciente);
            Add(cmd, "@data_pedido", SqlDbType.DateTime, data_pedido);
            Add(cmd, "@data_cadastro", SqlDbType.DateTime, DateTime.Now);
            Add(cmd, "@cod_especialidade", SqlDbType.Int, cod_espec);
            Add(cmd, "@exames_solicitados", SqlDbType.VarChar, exames);
            Add(cmd, "@outras_informacoes", SqlDbType.VarChar, (object)outras_info ?? DBNull.Value);
            Add(cmd, "@solicitante", SqlDbType.VarChar, solicitante);
            Add(cmd, "@status", SqlDbType.Int, 0);
            Add(cmd, "@usuario", SqlDbType.VarChar, usuario);

            cnn.Open();
            var id = cmd.ExecuteScalar();
            return (id == null || id == DBNull.Value) ? 0 : Convert.ToInt32(id);
        }
    }

    // =========================
    // PAGINAÇÃO (legados) – ROW_NUMBER()
    // =========================
    public static DataTable GetPendentesPaged(int pageIndex, int pageSize, out int totalRows)
    {
        using (var cnn = GetConn())
        using (var cmdCount = cnn.CreateCommand())
        {
            cmdCount.CommandText = "SELECT COUNT(1) FROM pedido_consulta WITH (NOLOCK) WHERE status = 0;";
            cnn.Open();
            object o = cmdCount.ExecuteScalar();
            totalRows = (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
        }

        int start = pageIndex * pageSize;

        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
;WITH X AS (
    SELECT
        ROW_NUMBER() OVER (ORDER BY data_pedido DESC, cod_pedido DESC) AS rn,
        cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
        cod_especialidade, exames_solicitados, outras_informacoes, carga_geral, usuario
    FROM pedido_consulta WITH (NOLOCK)
    WHERE status = 0
)
SELECT cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
       cod_especialidade, exames_solicitados, outras_informacoes, carga_geral, usuario
FROM X
WHERE rn BETWEEN (@start + 1) AND (@start + @size)
ORDER BY rn;";

            Add(cmd, "@start", SqlDbType.Int, start);
            Add(cmd, "@size", SqlDbType.Int, pageSize);

            var dt = ExecuteDataTable(cmd);
            EnsureDescricaoEspecColumn(dt);
            return dt;
        }
    }

    public static DataTable GetPendentesPorRHPaged(int prontuario, int pageIndex, int pageSize, out int totalRows)
    {
        using (var cnn = GetConn())
        using (var cmdCount = cnn.CreateCommand())
        {
            cmdCount.CommandText = @"
SELECT COUNT(1) FROM pedido_consulta WITH (NOLOCK)
WHERE status = 0 AND prontuario = @p;";
            Add(cmdCount, "@p", SqlDbType.Int, prontuario);
            cnn.Open();
            object o = cmdCount.ExecuteScalar();
            totalRows = (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
        }

        int start = pageIndex * pageSize;

        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
;WITH X AS (
    SELECT
        ROW_NUMBER() OVER (ORDER BY data_pedido DESC, cod_pedido DESC) AS rn,
        cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
        cod_especialidade, exames_solicitados, outras_informacoes, carga_geral, usuario
    FROM pedido_consulta WITH (NOLOCK)
    WHERE status = 0 AND prontuario = @pront
)
SELECT cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
       cod_especialidade, exames_solicitados, outras_informacoes, carga_geral, usuario
FROM X
WHERE rn BETWEEN (@start + 1) AND (@start + @size)
ORDER BY rn;";

            Add(cmd, "@pront", SqlDbType.Int, prontuario);
            Add(cmd, "@start", SqlDbType.Int, start);
            Add(cmd, "@size", SqlDbType.Int, pageSize);

            var dt = ExecuteDataTable(cmd);
            EnsureDescricaoEspecColumn(dt);
            return dt;
        }
    }

    // =========================
    // Métodos para ObjectDataSource (Opção A)
    // =========================

    // IMPORTANTE: Count aceita também sortExpression (mesmo sem usar)
    public static int SelectPendentesCount(string rh, string sortExpression)
    {
        int pront;
        bool hasPront = int.TryParse((rh ?? "").Trim(), out pront);

        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT COUNT(1)
FROM pedido_consulta WITH (NOLOCK)
WHERE status = 0 AND (@hasPront = 0 OR prontuario = @pront);";

            Add(cmd, "@hasPront", SqlDbType.Int, hasPront ? 1 : 0);
            Add(cmd, "@pront", SqlDbType.Int, hasPront ? (object)pront : DBNull.Value);

            cnn.Open();
            object o = cmd.ExecuteScalar();
            return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
        }
    }

    public static DataTable SelectPendentes(string rh, int startRowIndex, int maximumRows, string sortExpression)
    {
        int pront;
        bool hasPront = int.TryParse((rh ?? "").Trim(), out pront);

        string orderBy = "data_pedido DESC, cod_pedido DESC";
        string safeSort = CoerceSort(sortExpression);
        if (!string.IsNullOrEmpty(safeSort))
            orderBy = safeSort;

        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
;WITH X AS (
    SELECT
        ROW_NUMBER() OVER (ORDER BY " + orderBy + @") AS rn,
        cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
        cod_especialidade, exames_solicitados, outras_informacoes, carga_geral, usuario
    FROM pedido_consulta WITH (NOLOCK)
    WHERE status = 0
      AND (@hasPront = 0 OR prontuario = @pront)
)
SELECT cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
       cod_especialidade, exames_solicitados, outras_informacoes, carga_geral, usuario
FROM X
WHERE rn BETWEEN (@start + 1) AND (@start + @size)
ORDER BY rn;";

            Add(cmd, "@hasPront", SqlDbType.Int, hasPront ? 1 : 0);
            Add(cmd, "@pront", SqlDbType.Int, hasPront ? (object)pront : DBNull.Value);
            Add(cmd, "@start", SqlDbType.Int, startRowIndex);
            Add(cmd, "@size", SqlDbType.Int, maximumRows);

            var dt = ExecuteDataTable(cmd);
            EnsureDescricaoEspecColumn(dt);
            return dt;
        }
    }

    public static List<Pedido_> getListaPedidoConsultaPendentePorRH(int _prontuario)
    {
        var listaPedidos = new List<Pedido_>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();


            string sqlConsulta = "SELECT [cod_pedido]" +
                              ",[prontuario]" +
                              ",[nome_paciente]" +
                              ",[data_pedido]" +
                              ",[data_cadastro]" +
                              ",[cod_especialidade]" +
                              ",[exames_solicitados]" +
                              ",[outras_informacoes]" +
                              ",[carga_geral]" +
                              ",[usuario]" +
                              " FROM [pedido_consulta] " +
                              " WHERE  [status] = 0" +
                              " AND [prontuario] = " + _prontuario +
                              " ORDER BY cod_pedido DESC";

            cmm.CommandText = sqlConsulta;

            try
            {
                cnn.Open();
                SqlDataReader dr1 = cmm.ExecuteReader();

                //char[] ponto = { '.', ' ' };
                while (dr1.Read())
                {
                    Especialidade espec = new Especialidade();
                    Pedido_ p = new Pedido_();
                    p.cod_pedido = dr1.GetInt32(0);
                    p.prontuario = dr1.GetInt32(1);
                    p.nome_paciente = dr1.GetString(2);
                   // p.lista_exames = obterListaDeExames(p.cod_pedido);
                 //   p.lista_ressonancia = obterListaDeRessonancia(p.cod_pedido);
                    p.data_pedido = dr1.GetDateTime(3);
                    p.data_cadastro = dr1.GetDateTime(4);
                    p.cod_especialidade = dr1.GetInt32(5);
                    p.descricao_espec = EspecialidadeDAO.getEspecialidade(p.cod_especialidade);
                    p.exames_solicitados = dr1.GetString(6);
                    p.outras_informacoes = dr1.IsDBNull(7) ? "" : dr1.GetString(7);
                    p.carga_geral = dr1.GetInt32(8);
                    p.usuario = dr1.GetString(9);

                    listaPedidos.Add(p);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }


            return listaPedidos;
        }
    }


    public static List<Pedido_> getListaPedidoConsultaPendente()
    {
        var lista = new List<Pedido_>();
        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
       cod_especialidade, exames_solicitados, outras_informacoes, carga_geral, usuario
FROM pedido_consulta WITH (NOLOCK)
WHERE status = 0
ORDER BY cod_pedido DESC;";

            cnn.Open();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var p = new Pedido_();
                    p.cod_pedido = r.GetInt32(0);
                    p.prontuario = r.GetInt32(1);
                    p.nome_paciente = r.GetString(2);
                    p.data_pedido = r.GetDateTime(3);
                    p.data_cadastro = r.GetDateTime(4);
                    p.cod_especialidade = r.GetInt32(5);
                    p.descricao_espec = EspecialidadeDAO.getEspecialidade(r.GetInt32(5));
                    p.exames_solicitados = r.GetString(6);
                    p.outras_informacoes = r.IsDBNull(7) ? "" : r.GetString(7);
                    p.carga_geral = r.GetInt32(8);
                    p.usuario = r.GetString(9);
                    //p.lista_exames = obterListaDeExames(p.cod_pedido);
                   // p.lista_ressonancia = obterListaDeRessonancia(p.cod_pedido);
                    lista.Add(p);
                }
            }
        }
        return lista;
    }

    public static List<Pedido_> getListaPedidoConsultaArquivados()
    {
        var lista = new List<Pedido_>();
        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
       cod_especialidade, exames_solicitados, outras_informacoes, carga_geral,
       usuario, usuario_baixa, retirado_informacoes
FROM pedido_consulta WITH (NOLOCK)
WHERE status = 2
ORDER BY cod_pedido DESC;";

            cnn.Open();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    string info = r.IsDBNull(11) ? "" : r.GetString(11);
                    info = info.Replace("RG ou CPF:", "<br/>RG ou CPF: ")
                               .Replace("Data:", "<br/>Data: ");

                    var p = new Pedido_();
                    p.cod_pedido = r.GetInt32(0);
                    p.prontuario = r.GetInt32(1);
                    p.nome_paciente = r.GetString(2);
                    p.data_pedido = r.GetDateTime(3);
                    p.data_cadastro = r.GetDateTime(4);
                    p.cod_especialidade = r.GetInt32(5);
                    p.descricao_espec = EspecialidadeDAO.getEspecialidade(r.GetInt32(5));
                    p.exames_solicitados = r.GetString(6);
                    p.outras_informacoes = r.IsDBNull(7) ? "" : r.GetString(7);
                    p.carga_geral = r.GetInt32(8);
                    p.usuario = r.GetString(9);
                    p.usuario_baixa = r.GetString(10);
                    p.retirado_informacoes = info;
                    //p.lista_exames = obterListaDeExames(p.cod_pedido);
                   // p.lista_ressonancia = obterListaDeRessonancia(p.cod_pedido);
                    lista.Add(p);
                }
            }
        }
        return lista;
    }

    public static object getListaPedidoConsultaArquivadaPorRH(int prontuario)
    {
        var listaPedidos = new List<Pedido_>();
        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["gtaConnectionString"].ToString()))
        {
            SqlCommand cmm = cnn.CreateCommand();


            string sqlConsulta = "SELECT [cod_pedido]" +
                              ",[prontuario]" +
                              ",[nome_paciente]" +
                              ",[data_pedido]" +
                              ",[data_cadastro]" +
                              ",[cod_especialidade]" +
                              ",[exames_solicitados]" +
                              ",[outras_informacoes]" +
                              ",[carga_geral]" +
                              ",[usuario_baixa]" +
                              ",[retirado_informacoes]" +
                              " FROM [pedido_consulta] " +
                              " WHERE  [status] = 2" +
                              " AND [prontuario] = " + prontuario +
                              " ORDER BY cod_pedido DESC";

            cmm.CommandText = sqlConsulta;

            try
            {
                cnn.Open();
                SqlDataReader dr1 = cmm.ExecuteReader();

                //char[] ponto = { '.', ' ' };
                while (dr1.Read())
                {
                    Especialidade espec = new Especialidade();
                    Pedido_ p = new Pedido_();

                    string info = dr1.IsDBNull(10) ? "" : dr1.GetString(10);

                    // Reformatar para várias linhas
                    info = info.Replace("RG ou CPF:", "<br/>RG ou CPF: ")
                               .Replace("Data:", "<br/>Data: ");
                    p.cod_pedido = dr1.GetInt32(0);
                    p.prontuario = dr1.GetInt32(1);
                    p.nome_paciente = dr1.GetString(2);
                   // p.lista_exames = obterListaDeExames(p.cod_pedido);
                  //  p.lista_ressonancia = obterListaDeRessonancia(p.cod_pedido);
                    p.data_pedido = dr1.GetDateTime(3);
                    p.data_cadastro = dr1.GetDateTime(4);
                    p.cod_especialidade = dr1.GetInt32(5);
                    p.descricao_espec = EspecialidadeDAO.getEspecialidade(p.cod_especialidade);
                    p.exames_solicitados = dr1.GetString(6);
                    p.outras_informacoes = dr1.IsDBNull(7) ? "" : dr1.GetString(7);
                    p.carga_geral = dr1.GetInt32(8);
                    p.usuario_baixa = dr1.GetString(9);
                    p.retirado_informacoes = info;

                    listaPedidos.Add(p);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }


            return listaPedidos;
        }
    }

    // =========================
    // Leitura única
    // =========================
    public static Pedido_ getPedidoConsulta(int idPedido)
    {
        var pedido = new Pedido_();
        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT cod_pedido, prontuario, nome_paciente, data_pedido, data_cadastro,
       cod_especialidade, exames_solicitados, outras_informacoes,
       carga_geral, usuario, status
FROM pedido_consulta WITH (NOLOCK)
WHERE cod_pedido = @id;";
            Add(cmd, "@id", SqlDbType.Int, idPedido);

            cnn.Open();
            using (var r = cmd.ExecuteReader())
            {
                if (r.Read())
                {
                    pedido.cod_pedido = r.GetInt32(0);
                    pedido.prontuario = r.GetInt32(1);
                    pedido.nome_paciente = r.GetString(2);
                    pedido.data_pedido = r.GetDateTime(3);
                    pedido.data_cadastro = r.GetDateTime(4);
                    pedido.cod_especialidade = r.GetInt32(5);
                    pedido.descricao_espec = EspecialidadeDAO.getEspecialidade(pedido.cod_especialidade);
                    pedido.exames_solicitados = r.GetString(6);
                    pedido.outras_informacoes = r.IsDBNull(7) ? "" : r.GetString(7);
                    pedido.carga_geral = r.GetInt32(8);
                    pedido.usuario = r.GetString(9);
                    pedido.status_pedido = r.GetInt32(10);
                }
            }
        }
        return pedido;
    }

    // =========================
    // Updates
    // =========================
    public static string AtualizaPedido(string outras_informacoes, int idPedido)
    {
        string usuario = "";
        if (System.Web.HttpContext.Current != null &&
            System.Web.HttpContext.Current.User != null &&
            System.Web.HttpContext.Current.User.Identity != null &&
            System.Web.HttpContext.Current.User.Identity.Name != null)
        {
            usuario = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();
        }

        using (var cnn = GetConn())
        {
            cnn.Open();
            using (var tran = cnn.BeginTransaction(IsolationLevel.ReadCommitted))
            using (var cmd = cnn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"
UPDATE pedido_consulta
SET outras_informacoes = @outras
WHERE cod_pedido = @id;";

                Add(cmd, "@outras", SqlDbType.VarChar, (object)outras_informacoes ?? DBNull.Value);
                Add(cmd, "@id", SqlDbType.Int, idPedido);

                try
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    LogDAO.gravaLog(
                        string.Format("UPDATE: CÓDIGO PEDIDO {0}", idPedido),
                        "CAMPO OUTRAS INFORMACOES",
                        usuario
                    );
                    return "Cadastro realizado com sucesso!";
                }
                catch (Exception ex)
                {
                    try { tran.Rollback(); } catch { }
                    return ex.Message;
                }
            }
        }
    }

    public static void deletePedidodeConsulta(int idPedido)
    {
        string usuario = "";
        if (System.Web.HttpContext.Current != null &&
            System.Web.HttpContext.Current.User != null &&
            System.Web.HttpContext.Current.User.Identity != null &&
            System.Web.HttpContext.Current.User.Identity.Name != null)
        {
            usuario = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();
        }

        using (var cnn = GetConn())
        {
            cnn.Open();
            using (var tran = cnn.BeginTransaction(IsolationLevel.ReadCommitted))
            using (var cmd = cnn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"
UPDATE pedido_consulta
SET status = @status
WHERE cod_pedido = @id;";
                Add(cmd, "@status", SqlDbType.Int, 1);
                Add(cmd, "@id", SqlDbType.Int, idPedido);

                try
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    LogDAO.gravaLog(
                        string.Format("DELETE: CÓDIGO PEDIDO {0}", idPedido),
                        "CAMPO STATUS",
                        usuario
                    );
                }
                catch
                {
                    try { tran.Rollback(); } catch { }
                    throw;
                }
            }
        }
    }

    public static void AtualizarOutrasInformacoes(int idPedido, string outrasInformacoes)
    {
        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
UPDATE pedido_consulta
SET retirado_informacoes = @info
WHERE cod_pedido = @id;";
            Add(cmd, "@info", SqlDbType.VarChar, (object)outrasInformacoes ?? DBNull.Value);
            Add(cmd, "@id", SqlDbType.Int, idPedido);

            cnn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static void filePedidodeConsulta(int idPedido, string usuario_baixa)
    {
        if (string.IsNullOrEmpty(usuario_baixa)) usuario_baixa = "desconhecido";

        using (var cnn = GetConn())
        {
            cnn.Open();
            using (var tran = cnn.BeginTransaction(IsolationLevel.ReadCommitted))
            using (var cmd = cnn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"
UPDATE pedido_consulta
SET status = @status, data_baixa = @data_baixa, usuario_baixa = @usuario_baixa
WHERE cod_pedido = @id;";
                Add(cmd, "@status", SqlDbType.Int, 2);
                Add(cmd, "@data_baixa", SqlDbType.DateTime, DateTime.Now);
                Add(cmd, "@usuario_baixa", SqlDbType.VarChar, usuario_baixa);
                Add(cmd, "@id", SqlDbType.Int, idPedido);

                try
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    LogDAO.gravaLog(
                        string.Format("ARQUIVADO: CÓDIGO PEDIDO {0}", idPedido),
                        "CAMPO STATUS",
                        usuario_baixa
                    );
                }
                catch
                {
                    try { tran.Rollback(); } catch { }
                    throw;
                }
            }
        }
    }

    // =========================
    // Auxiliares (listas)
    // =========================
    private static string obterListaDeExames(int cod_pedido)
    {
        var lista = new List<ExameUnico>();
        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT pe.cod_especialidade, pe.descricao_especialidade
FROM hspmAtendimento.dbo.pedido_consulta e
JOIN hspmAtendimento.dbo.especialidade pe
  ON e.cod_especialidade = pe.cod_especialidade
WHERE pe.status_especialidade = 'A' AND e.cod_pedido = @id;";
            Add(cmd, "@id", SqlDbType.Int, cod_pedido);

            cnn.Open();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var exm = new ExameUnico();
                    exm.cod_exames_unico = r.GetInt32(0);
                    exm.descricao_exames_unico = r.GetString(1);
                    lista.Add(exm);
                }
            }
        }
        return string.Join(", ", lista.Select(x => x.descricao_exames_unico).ToArray());
    }

    private static string obterListaDeRessonancia(int cod_pedido)
    {
        var lista = new List<Ressonancia>();
        using (var cnn = GetConn())
        using (var cmd = cnn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT r.cod_ressonancia, r.descricao_ressonancia
FROM hspmAtendimento.dbo.ressonancia r
JOIN hspmAtendimento.dbo.pedido_ressonancia pr
  ON r.cod_ressonancia = pr.cod_ressonancia
WHERE r.status = 'A' AND pr.cod_pedido = @id;";
            Add(cmd, "@id", SqlDbType.Int, cod_pedido);

            cnn.Open();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var ress = new Ressonancia();
                    ress.cod_ressonancia = r.GetInt32(0);
                    ress.descricao_ressonancia = r.GetString(1);
                    lista.Add(ress);
                }
            }
        }
        return string.Join(", ", lista.Select(x => x.descricao_ressonancia).ToArray());
    }
}

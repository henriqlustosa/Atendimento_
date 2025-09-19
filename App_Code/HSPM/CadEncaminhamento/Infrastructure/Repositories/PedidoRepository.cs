using Hspm.CadEncaminhamento.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Hspm.CadEncaminhamento.Infrastructure
{
    public sealed class PedidoRepository : IPedidoRepository
    {
        private readonly IConnectionFactory _factory;

        public PedidoRepository(IConnectionFactory factory)
        {
            _factory = factory;
        }
        public int Gravar(Hspm.CadEncaminhamento.Domain.Pedido pedido)
        {
            using (SqlConnection conn = _factory.Create())
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Se seu SQL for 2005, troque por GETDATE()
                        const string sql = @"
INSERT INTO pedido_consulta
  (prontuario, nome_paciente, data_pedido, cod_especialidade, exames_solicitados, outras_informacoes, solicitante, usuario, data_cadastro)
VALUES (@prontuario, @nome, @data, @cod_esp, @exames_texto, @obs, @solicitante, @usuario, GETDATE());
SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(sql, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@prontuario", pedido.Prontuario);
                            cmd.Parameters.AddWithValue("@nome", pedido.NomePaciente ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@data", pedido.DataPedido);
                            cmd.Parameters.AddWithValue("@cod_esp", pedido.CodEspecialidade);
                            cmd.Parameters.AddWithValue("@exames_texto", (object)pedido.ExamesSolicitadosTexto ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@obs", (object)pedido.OutrasInformacoes ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@solicitante", (object)pedido.Solicitante ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@usuario", (object)pedido.Usuario ?? DBNull.Value);

                            object scalar = cmd.ExecuteScalar();

                            // SCOPE_IDENTITY() -> decimal em ADO.NET; converta corretamente
                            int id = Convert.ToInt32(scalar);

                            tx.Commit();
                            return id;
                        }
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { /* ignore rollback errors */ }

                        // >>> DIAGNÓSTICO: não masque a exceção aqui
                        // Em produção: logue e lance uma ApplicationException com inner
                        // Em dev: re-lance para ver o Yellow Screen ou trate como preferir
                        throw new ApplicationException("Erro ao gravar pedido_consulta: " + ex.Message, ex);
                    }
                }
            }
        }


        public void GravarRelacionamentos(int pedidoId, IList<ExameSelecionado> exames)
        {
            if (exames == null || exames.Count == 0) return;

            using (SqlConnection conn = _factory.Create())
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < exames.Count; i++)
                        {
                            ExameSelecionado ex = exames[i];
                            string grupo = (ex.Grupo ?? string.Empty).ToLowerInvariant();

                            if (grupo == "preop")
                                InsertRel(conn, tx, "pedido_pre_operatorio", "cod_pre_operatorio", pedidoId, ex.Codigo);
                            else if (grupo == "ressonancia")
                                InsertRel(conn, tx, "pedido_ressonancia", "cod_ressonancia", pedidoId, ex.Codigo);
                            else if (grupo == "teleconsulta")
                                InsertRel(conn, tx, "pedido_teleconsulta", "cod_teleconsulta", pedidoId, ex.Codigo);
                            else if (grupo == "unico")
                                InsertRel(conn, tx, "pedido_exames_unico", "cod_exames_unico", pedidoId, ex.Codigo);
                            else
                                throw new ApplicationException("Grupo de exame desconhecido: " + ex.Grupo);
                        }

                        tx.Commit();
                    }
                    catch (SqlException ex) // pega detalhes do SQL Server
                    {
                        try { tx.Rollback(); } catch { /* ignore */ }
                        throw new ApplicationException(FormatSqlException("GravarRelacionamentos", pedidoId, ex), ex);
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { /* ignore */ }
                        throw new ApplicationException("Falha ao gravar relacionamentos do pedido " + pedidoId + ": " + ex.Message, ex);
                    }
                }
            }
        }

        // Monta uma mensagem rica com todos os erros do SQL
        private static string FormatSqlException(string op, int pedidoId, SqlException ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Erro ao executar " + op + " para o pedido " + pedidoId + ".");
            sb.AppendLine("Mensagem: " + ex.Message);

            for (int i = 0; i < ex.Errors.Count; i++)
            {
                var e = ex.Errors[i];
                sb.AppendLine(string.Format(
                    "#{0} | Number={1} | Class={2} | State={3} | Line={4} | Procedure={5}",
                    i + 1, e.Number, e.Class, e.State, e.LineNumber, string.IsNullOrEmpty(e.Procedure) ? "-" : e.Procedure
                ));
                if (!string.IsNullOrEmpty(e.Message))
                    sb.AppendLine("   -> " + e.Message);
            }
            return sb.ToString();
        }


        private static void InsertRel(SqlConnection conn, SqlTransaction tx,
                                          string table, string fkColumn,
                                          int pedidoId, int codigoExame)
        {
            string sql = "INSERT INTO " + table + " (cod_pedido, " + fkColumn + ", data_cadastro, status) " +
                         "VALUES (@pedido, @cod, SYSDATETIME(), @status);";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@pedido", SqlDbType.Int).Value = pedidoId;
                cmd.Parameters.Add("@cod", SqlDbType.Int).Value = codigoExame;
            
                cmd.Parameters.Add("@status", SqlDbType.NVarChar, 1).Value = "A";
                cmd.ExecuteNonQuery();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Hspm.CadEncaminhamento.Domain;

namespace Hspm.CadEncaminhamento.Infrastructure
{
    public sealed class PedidoRepository : IPedidoRepository
    {
        private readonly IConnectionFactory _factory;

        public PedidoRepository(IConnectionFactory factory)
        {
            _factory = factory;
        }

        // >>> use o tipo totalmente qualificado para garantir que casa com a interface
        public int Gravar(Hspm.CadEncaminhamento.Domain.Pedido pedido)
        {
            using (SqlConnection conn = _factory.Create())
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand(
@"INSERT INTO pedido_consulta
  (prontuario, nome_paciente, data_pedido, cod_especialidade, exames_solicitados, outras_informacoes, solicitante, usuario, dt_criacao)
VALUES (@prontuario, @nome, @data, @cod_esp, @exames_texto, @obs, @solicitante, @usuario, SYSDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx);

                        cmd.Parameters.AddWithValue("@prontuario", pedido.Prontuario);
                        cmd.Parameters.AddWithValue("@nome", pedido.NomePaciente);
                        cmd.Parameters.AddWithValue("@data", pedido.DataPedido);
                        cmd.Parameters.AddWithValue("@cod_esp", pedido.CodEspecialidade);
                        cmd.Parameters.AddWithValue("@exames_texto", (object)pedido.ExamesSolicitadosTexto ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@obs", (object)pedido.OutrasInformacoes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@solicitante", (object)pedido.Solicitante ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@usuario", (object)pedido.Usuario ?? DBNull.Value);

                        int id = (int)cmd.ExecuteScalar();
                        tx.Commit();
                        return id;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
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
                                InsertRel(conn, tx, "pedido_pre_operatorio", "cod_pre_operatorio", pedidoId, ex.Codigo, ex.Descricao);
                            else if (grupo == "ressonancia")
                                InsertRel(conn, tx, "pedido_ressonancia", "cod_ressonancia", pedidoId, ex.Codigo, ex.Descricao);
                            else if (grupo == "teleconsulta")
                                InsertRel(conn, tx, "pedido_teleconsulta", "cod_teleconsulta", pedidoId, ex.Codigo, ex.Descricao);
                            else if (grupo == "unico")
                                InsertRel(conn, tx, "pedido_exames_unico", "cod_exames_unico", pedidoId, ex.Codigo, ex.Descricao);
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void InsertRel(SqlConnection conn, SqlTransaction tx,
                                      string table, string fkColumn,
                                      int pedidoId, int codigoExame, string descricaoExame)
        {
            string sql = "INSERT INTO " + table + " (cod_pedido, " + fkColumn + ", descricao, data_cadastro, status) " +
                         "VALUES (@pedido, @cod, @desc, SYSDATETIME(), @status);";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@pedido", SqlDbType.Int).Value = pedidoId;
                cmd.Parameters.Add("@cod", SqlDbType.Int).Value = codigoExame;
                cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 255).Value = (object)descricaoExame ?? DBNull.Value;
                cmd.Parameters.Add("@status", SqlDbType.NVarChar, 1).Value = "A";
                cmd.ExecuteNonQuery();
            }
        }
    }
}

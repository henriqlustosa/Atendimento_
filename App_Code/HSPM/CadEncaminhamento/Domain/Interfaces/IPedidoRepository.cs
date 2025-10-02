using System.Collections.Generic;

namespace Hspm.CadEncaminhamento.Domain
{
    public interface IPedidoRepository
    {
        int Gravar(Pedido pedido);
        void GravarRelacionamentos(int pedidoId, IList<ExameSelecionado> exames);
        // ===== NOVOS p/ EDIÇÃO =====
        // Cabeçalho do pedido
        Pedido ObterPorId(int id);

        // Listas de selecionados (códigos)
        IList<int> ObterRessonanciaSelecionados(int idPedido);
        IList<int> ObterPreOperatorioSelecionados(int idPedido);
        IList<int> ObterTeleconsultaSelecionados(int idPedido);
        IList<int> ObterExamesUnicosSelecionados(int idPedido);

        // Update do cabeçalho
        void Atualizar(Pedido pedido);

        void SincronizarRelacionamentos(int pedidoId, IList<ExameSelecionado> exames);
    }
}

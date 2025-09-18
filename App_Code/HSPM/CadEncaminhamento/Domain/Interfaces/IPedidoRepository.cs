using System.Collections.Generic;

namespace Hspm.CadEncaminhamento.Domain
{
    public interface IPedidoRepository
    {
        int Gravar(Pedido pedido);
        void GravarRelacionamentos(int pedidoId, IList<ExameSelecionado> exames);
    }
}

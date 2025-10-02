using System;

namespace Hspm.CadEncaminhamento.Application
{
    public interface IPedidoQuery
    {
        PedidoDetailsDto ObterPorId(int id);
    }
}

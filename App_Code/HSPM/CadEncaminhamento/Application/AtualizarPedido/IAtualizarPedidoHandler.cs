namespace Hspm.CadEncaminhamento.Application
{
    public interface IAtualizarPedidoHandler
    {
        void Handle(AtualizarPedidoCommand cmd);
    }
}
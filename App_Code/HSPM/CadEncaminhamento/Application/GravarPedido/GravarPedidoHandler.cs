namespace Hspm.CadEncaminhamento.Application
{
    public interface IGravarPedidoHandler
    {
        int Handle(GravarPedidoCommand command);
    }
}

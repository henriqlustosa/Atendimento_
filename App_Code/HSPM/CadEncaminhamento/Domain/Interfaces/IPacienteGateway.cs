namespace Hspm.CadEncaminhamento.Domain
{
    public interface IPacienteGateway
    {
        PacienteDto ObterPorProntuario(int prontuario);
    }
}

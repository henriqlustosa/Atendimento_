namespace Hspm.CadEncaminhamento.Domain
{
    public sealed class ExameSelecionado
    {
        public int Codigo { get; private set; }
        public string Descricao { get; private set; }
        public string Grupo { get; private set; }

        public ExameSelecionado(int codigo, string descricao, string grupo)
        {
            Codigo = codigo;
            Descricao = descricao ?? string.Empty;
            Grupo = grupo ?? string.Empty;
        }
    }
}

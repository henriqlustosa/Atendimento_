using System;

namespace Hspm.CadEncaminhamento.Domain
{
    public sealed class Pedido
    {
        public int Id { get; private set; }
        public int Prontuario { get; private set; }
        public string NomePaciente { get; private set; }
        public DateTime DataPedido { get; private set; }
        public int CodEspecialidade { get; private set; }
        public string ExamesSolicitadosTexto { get; private set; }
        public string OutrasInformacoes { get; private set; }
       
        public string Usuario { get; private set; }

        // 🔹 Novo campo
        public int CargaGeral { get; private set; }

        public Pedido(int prontuario, string nomePaciente, DateTime dataPedido,
                      int codEspecialidade, string examesSolicitadosTexto,
                      string outrasInformacoes, string usuario,
                      int cargaGeral)
        {
            Prontuario = prontuario;
            NomePaciente = (nomePaciente ?? string.Empty).Trim();
            DataPedido = dataPedido;
            CodEspecialidade = codEspecialidade;
            ExamesSolicitadosTexto = examesSolicitadosTexto ?? string.Empty;
            OutrasInformacoes = outrasInformacoes ?? string.Empty;
        
            Usuario = usuario ?? string.Empty;
            CargaGeral = cargaGeral;
        }

        public Pedido()
        {
        }

        public void DefinirId(int value)
        {
            Id = value;
        }

        public void AlterarCabecalho(int prontuario, string nome, DateTime data, int codEsp,
                                     string examesTexto, string obs,
                                     string usuario, int cargaGeral)
        {
            // Validações de domínio (se existirem) podem ser aplicadas aqui.
            Prontuario = prontuario;
            NomePaciente = nome;
            DataPedido = data;
            CodEspecialidade = codEsp;
            ExamesSolicitadosTexto = examesTexto;
            OutrasInformacoes = obs;
            
            Usuario = usuario;
            CargaGeral = cargaGeral;
        }
    }
}

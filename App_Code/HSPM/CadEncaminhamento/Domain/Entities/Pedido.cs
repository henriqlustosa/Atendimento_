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
        public string Solicitante { get; private set; }
        public string Usuario { get; private set; }

        public Pedido(int prontuario, string nomePaciente, DateTime dataPedido,
                      int codEspecialidade, string examesSolicitadosTexto,
                      string outrasInformacoes, string solicitante, string usuario)
        {
            Prontuario = prontuario;
            NomePaciente = (nomePaciente ?? string.Empty).Trim();
            DataPedido = dataPedido;
            CodEspecialidade = codEspecialidade;
            ExamesSolicitadosTexto = examesSolicitadosTexto ?? string.Empty;
            OutrasInformacoes = outrasInformacoes ?? string.Empty;
            Solicitante = (solicitante ?? string.Empty).Trim();
            Usuario = usuario ?? string.Empty;
        }

        public Pedido()
        {
        }
        public void DefinirId(int value) { this.Id = value; }

        public void AlterarCabecalho(int prontuario, string nome, DateTime data, int codEsp,
                             string examesTexto, string obs, string solicitante, string usuario)
        {
            // validações de domínio aqui, se houver
            this.Prontuario = prontuario;
            this.NomePaciente = nome;
            this.DataPedido = data;
            this.CodEspecialidade = codEsp;
            this.ExamesSolicitadosTexto = examesTexto;
            this.OutrasInformacoes = obs;
            this.Solicitante = solicitante;
            this.Usuario = usuario;
        }
    }
}
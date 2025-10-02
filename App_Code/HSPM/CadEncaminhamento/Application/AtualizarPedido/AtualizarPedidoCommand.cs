using System;
using System.Collections.Generic;

namespace Hspm.CadEncaminhamento.Application
{
    public class AtualizarPedidoCommand
    {
        public int Id { get; set; }
        public int Prontuario { get; set; }
        public string NomePaciente { get; set; }
        public DateTime DataPedido { get; set; }
        public int CodEspecialidade { get; set; }
        public string Observacoes { get; set; }
        public string Solicitante { get; set; }
        public string Usuario { get; set; }

        // opcional: texto consolidado dos exames para histórico
        public string ExamesPreOpTextoParaHistorico { get; set; }

        public IList<int> CodigosRessonancia { get; set; }
        public IList<int> CodigosPreOperatorio { get; set; }
        public IList<int> CodigosTeleconsulta { get; set; }
        public IList<int> CodigosExamesUnicos { get; set; }

        public AtualizarPedidoCommand()
        {
            CodigosRessonancia = new List<int>();
            CodigosPreOperatorio = new List<int>();
            CodigosTeleconsulta = new List<int>();
            CodigosExamesUnicos = new List<int>();
        }
    }
}

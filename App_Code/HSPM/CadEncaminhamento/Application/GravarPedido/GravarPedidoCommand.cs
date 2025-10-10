using System;
using System.Collections.Generic;
using Hspm.CadEncaminhamento.Domain;

namespace Hspm.CadEncaminhamento.Application
{
    public sealed class GravarPedidoCommand
    {
        public int Prontuario { get; set; }
        public string NomePaciente { get; set; }
        public DateTime DataPedido { get; set; }
        public int CodEspecialidade { get; set; }
        public string OutrasInformacoes { get; set; }
   
        public string Usuario { get; set; }
        // novo campo indicando se é carga geral (1 = sim, 0 = não)
        public int CargaGeral { get; set; }

        public IList<ExameSelecionado> Exames { get; set; }

        // compatibilidade com legado (histórico textual)
        public string ExamesPreOpTextoParaHistorico { get; set; }
    }
}

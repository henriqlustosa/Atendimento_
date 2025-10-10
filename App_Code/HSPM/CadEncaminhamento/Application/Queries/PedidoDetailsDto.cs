using System;
using System.Collections.Generic;

namespace Hspm.CadEncaminhamento.Application
{
    public class PedidoDetailsDto
    {
        public int Id { get; set; }
        public string Prontuario { get; set; }
        public string NomePaciente { get; set; }
        public DateTime DataPedido { get; set; }
        public int CodEspecialidade { get; set; }
        public string Observacoes { get; set; }
        public int CargaGeral { get; set; }

        public IList<int> Ressonancia { get; set; }
        public IList<int> PreOperatorio { get; set; }
        public IList<int> Teleconsulta { get; set; }
        public IList<int> ExamesUnicos { get; set; }

        public PedidoDetailsDto()
        {
            Ressonancia = new List<int>();
            PreOperatorio = new List<int>();
            Teleconsulta = new List<int>();
            ExamesUnicos = new List<int>();
        }
    }
}

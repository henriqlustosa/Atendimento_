using System;
using System.Collections.Generic;
using System.Data;
using Hspm.CadEncaminhamento.Domain;

namespace Hspm.CadEncaminhamento.Application
{
    public class PedidoQuery : IPedidoQuery
    {
        private readonly IPedidoRepository _repo;

        public PedidoQuery(IPedidoRepository repo)
        {
            _repo = repo;
        }

        public PedidoDetailsDto ObterPorId(int id)
        {
            Pedido p = _repo.ObterPorId(id);
            if (p == null) return null;

            // Mapeia para o DTO de leitura
            var dto = new PedidoDetailsDto();
            // Se o get de Id não estiver público, use: dto.Id = id;
            dto.Id = p.Id;
            dto.Prontuario = p.Prontuario.ToString();
            dto.NomePaciente = p.NomePaciente;
            dto.DataPedido = p.DataPedido;
            dto.CodEspecialidade = p.CodEspecialidade;
            // No seu domínio o campo de observações é "OutrasInformacoes"
            dto.Observacoes = p.OutrasInformacoes;
            dto.CargaGeral = p.CargaGeral;

            // Listas de selecionados
            dto.Ressonancia = _repo.ObterRessonanciaSelecionados(id);
            dto.PreOperatorio = _repo.ObterPreOperatorioSelecionados(id);
            dto.Teleconsulta = _repo.ObterTeleconsultaSelecionados(id);
            dto.ExamesUnicos = _repo.ObterExamesUnicosSelecionados(id);

            return dto;
        }
    }
}

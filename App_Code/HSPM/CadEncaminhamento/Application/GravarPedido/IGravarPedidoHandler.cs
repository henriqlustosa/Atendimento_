using Hspm.CadEncaminhamento.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hspm.CadEncaminhamento.Application
{
    public sealed class GravarPedidoHandler : IGravarPedidoHandler
    {
        private readonly IPedidoRepository _repo;
        private readonly IValidator<GravarPedidoCommand> _validator;

        public GravarPedidoHandler(IPedidoRepository repo, IValidator<GravarPedidoCommand> validator)
        {
            _repo = repo;
            _validator = validator;
        }

        public int Handle(GravarPedidoCommand c)
        {
            ValidationResult vr = _validator.Validate(c);
            if (!vr.IsValid)
                throw new ApplicationException(string.Join("\n", new List<string>(vr.Errors).ToArray()));


            Pedido pedido = new Pedido(
                c.Prontuario,
                c.NomePaciente,
                c.DataPedido,
                c.CodEspecialidade,
                c.ExamesPreOpTextoParaHistorico,
                c.OutrasInformacoes,
                c.Usuario,
                c.CargaGeral
                
            );

            int id = _repo.Gravar(pedido);
            _repo.GravarRelacionamentos(id, c.Exames);

            return id;
        }
    }
}

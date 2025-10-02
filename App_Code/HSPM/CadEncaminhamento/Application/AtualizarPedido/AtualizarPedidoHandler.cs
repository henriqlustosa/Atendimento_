using System;
using System.Collections.Generic;
using Hspm.CadEncaminhamento.Domain;

namespace Hspm.CadEncaminhamento.Application
{
    public class AtualizarPedidoHandler : IAtualizarPedidoHandler
    {
        private readonly IPedidoRepository _repo;
        private readonly IValidator<AtualizarPedidoCommand> _validator;

        public AtualizarPedidoHandler(IPedidoRepository repo, IValidator<AtualizarPedidoCommand> validator)
        {
            _repo = repo;
            _validator = validator; // pode ser null
        }

        public void Handle(AtualizarPedidoCommand cmd)
        {
            if (_validator != null)
            {
                var vr = _validator.Validate(cmd);
                if (_validator != null)
                {
                    ValidationResult vr_ = _validator.Validate(cmd);
                    if (!vr_.IsValid)
                        throw new ApplicationException(BuildValidationMessage(vr_));
                }

            }

            var p = _repo.ObterPorId(cmd.Id);
            if (p == null) throw new ApplicationException("Pedido não encontrado.");

            p.AlterarCabecalho(
                cmd.Prontuario,
                (cmd.NomePaciente ?? "").Trim().ToUpperInvariant(),
                cmd.DataPedido,
                cmd.CodEspecialidade,
                cmd.ExamesPreOpTextoParaHistorico,
                cmd.Observacoes,
                (cmd.Solicitante ?? "").Trim().ToUpperInvariant(),
                cmd.Usuario
            );

            _repo.Atualizar(p);

            var itens = new List<ExameSelecionado>();
            AddExames(itens, cmd.CodigosPreOperatorio, "PreOp");
            AddExames(itens, cmd.CodigosRessonancia, "Ressonancia");
            AddExames(itens, cmd.CodigosTeleconsulta, "Teleconsulta");
            AddExames(itens, cmd.CodigosExamesUnicos, "Unico");
            _repo.SincronizarRelacionamentos(cmd.Id, itens);
        }


        private static void AddExames(IList<ExameSelecionado> list, IList<int> codigos, string grupo)
        {
            if (codigos == null) return;
            for (int i = 0; i < codigos.Count; i++)
            {
                // ExameSelecionado(int codigo, string descricao, string grupo)
                list.Add(new ExameSelecionado(codigos[i], null, grupo));
            }
        }
        private static string BuildValidationMessage(ValidationResult vr)
        {
            if (vr == null) return "Falha de validação.";
            if (vr.Errors == null || vr.Errors.Count == 0) return "Falha de validação.";

            string[] arr = new string[vr.Errors.Count];
            for (int i = 0; i < vr.Errors.Count; i++) arr[i] = vr.Errors[i];
            return string.Join("; ", arr);
        }
    }
}

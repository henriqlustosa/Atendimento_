using System;
using Hspm.CadEncaminhamento.Domain;

namespace Hspm.CadEncaminhamento.Application
{
    public sealed class GravarPedidoValidator : IValidator<GravarPedidoCommand>
    {
        public ValidationResult Validate(GravarPedidoCommand c)
        {
            ValidationResult r = new ValidationResult();

            if (c == null)
            {
                r.Add("Comando nulo.");
                return r;
            }

            if (c.Prontuario <= 0) r.Add("Prontuário inválido.");
            if (string.IsNullOrEmpty(c.NomePaciente)) r.Add("Nome do paciente é obrigatório.");
            if (c.DataPedido == default(DateTime)) r.Add("Data do pedido inválida.");
            if (c.CodEspecialidade <= 0) r.Add("Selecione a especialidade.");
            if (c.Exames == null || c.Exames.Count == 0) r.Add("Selecione ao menos um exame.");

            return r;
        }
    }
}

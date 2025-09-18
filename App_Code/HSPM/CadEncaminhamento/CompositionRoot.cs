using Hspm.CadEncaminhamento.Domain;
using Hspm.CadEncaminhamento.Application;
using Hspm.CadEncaminhamento.Infrastructure;

namespace Hspm.CadEncaminhamento
{
    public static class CompositionRoot
    {
        private static readonly IConnectionFactory _factory =
            new SqlConnectionFactory("gtaConnectionString"); // defina no Web.config

        private static readonly IPacienteGateway _pacientes =
            new PacienteGatewayApi("http://10.48.21.64:5000/hspmsgh-api");

        private static readonly IEspecialidadeCatalog _especialidades =
            new EspecialidadeCatalog(_factory);

        private static readonly IExameCatalog _exames =
            new ExameCatalog(_factory);

        private static readonly IPedidoRepository _repo =
            new PedidoRepository(_factory);

        private static readonly IValidator<GravarPedidoCommand> _validator =
            new GravarPedidoValidator();

        private static readonly IGravarPedidoHandler _handler =
            new GravarPedidoHandler(_repo, _validator);

        public static IPacienteGateway Pacientes { get { return _pacientes; } }
        public static IEspecialidadeCatalog Especialidades { get { return _especialidades; } }
        public static IExameCatalog Exames { get { return _exames; } }
        public static IGravarPedidoHandler GravarPedido { get { return _handler; } }
    }
}

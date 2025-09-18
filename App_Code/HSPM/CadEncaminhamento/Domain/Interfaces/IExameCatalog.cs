using System.Collections.Generic;

namespace Hspm.CadEncaminhamento.Domain
{
    public interface IExameCatalog
    {
        IList<ListItemDto> ListarPreOperatorio();
        IList<ListItemDto> ListarRessonancia();
        IList<ListItemDto> ListarTeleconsulta();
        IList<ListItemDto> ListarExamesUnicos();
    }
}

using System.Collections.Generic;

namespace Hspm.CadEncaminhamento.Domain
{
    public interface IEspecialidadeCatalog
    {
        IList<ListItemDto> Listar();
    }
}

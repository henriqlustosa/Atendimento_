using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;

public partial class manuais_Fluxo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void linkFluxoPdf_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/manuais/docs/FLUXOGRAMA_DE_LIGACOES.pdf");
    }
}

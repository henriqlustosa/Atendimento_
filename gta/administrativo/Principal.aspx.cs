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

public partial class administrativo_Principal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lbConsultas1Tentativa.Text = AtivoDAO.QuantidadeConsultasRealizarAtivo(0, 0).ToString();
        lbConsultas2Tentativa.Text = AtivoDAO.QuantidadeConsultasRealizarAtivo(1, 1).ToString();
        lbConsultas3Tentativa.Text = AtivoDAO.QuantidadeConsultasRealizarAtivo(1, 2).ToString();
    }
}

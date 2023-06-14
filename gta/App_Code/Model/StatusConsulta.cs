using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for StatusConsulta
/// </summary>
public class StatusConsulta
{
	public StatusConsulta()
	{
	}
    public int Id_Status { get; set; }
    public string Descricao { get; set; }
    public string Tentativa { get; set; }
    public string Ativo { get; set; }
}

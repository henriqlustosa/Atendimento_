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
/// Summary description for Ativo_Ligacao
/// </summary>
public class Ativo_Ligacao : Ativo
{
	public Ativo_Ligacao()
	{
	}
    public int Id_Ativo { get; set; }
    public int Id_Consulta { get; set; }
    public string Status { get; set; }
    public string Observacao { get; set; }
    public DateTime Data_Contato { get; set; }
    public int Tentativa { get; set; }
    public string Usuario_Contato { get; set; }
}
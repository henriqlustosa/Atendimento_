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
/// Summary description for PacienteMailling
/// </summary>
public class PacienteMailling
{
	public PacienteMailling()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int Prontuario { get; set; }
    public string Nome { get; set; }
    public string Telefone1 { get; set; }
    public string Telefone2 { get; set; }
    public string Telefone3 { get; set; }
    public string Telefone4 { get; set; }

}
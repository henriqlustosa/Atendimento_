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
/// Summary description for Usuario
/// </summary>
public class Usuario
{
	public Usuario()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int Id { get; set; }
    public string UserLogin { get; set; }
    public string Nome_Completo { get; set; }
    public string Setor { get; set; }
    public string Cargo { get; set; }
    public string Uimage { get; set; }
}

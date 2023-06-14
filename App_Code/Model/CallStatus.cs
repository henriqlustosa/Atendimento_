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
/// Summary description for CallStatus
/// </summary>
public class CallStatus
{
	public int quantidade { get; set; }
    public string descricao { get; set; }
    public string porcentagem { get; set; }
}

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
/// Summary description for ConsultasRemarcar
/// </summary>
public class ConsultasRemarcar : Ativo_Ligacao
{
    public int id_cancela { get; set; }
    public int Id_Informacao { get; set; }
    public string DescricaoRemarcar { get; set; }
    public string Stat { get; set; }
    
}
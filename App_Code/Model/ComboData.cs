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
using System.Collections.Generic;

/// <summary>
/// Summary description for ComboData
/// </summary>
public class ComboData
{
    public ComboData()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string DayValue { get; set; }
    public string DayName { get; set; }

    public static List<ComboData> Data()
    {
        //Vamos considerar que a data seja o dia de hoje, mas pode ser qualquer data.
        DateTime data = DateTime.Today;

        //DateTime com o primeiro dia do mês
        DateTime primeiroDiaDoMes = new DateTime(data.Year, data.Month, 1);

        return new List<ComboData> {

            new ComboData{DayValue=DateTime.Today.Date.ToString("dd/MM/yyyy"),DayName="Hoje" }
            ,new ComboData{DayValue=DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy"),DayName= "Ontem"}
            ,new ComboData{DayValue=DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + " | " + DateTime.Today.Date.ToString("dd/MM/yyyy"),DayName="Últimos 7 Dias"}
            ,new ComboData{DayValue=DateTime.Today.AddDays(-30).ToString("dd/MM/yyyy") +" | "+ DateTime.Today.Date.ToString("dd/MM/yyyy"),DayName="Últimos 30 Dias "}
            ,new ComboData{DayValue=primeiroDiaDoMes.ToString("dd/MM/yyyy") +" | "+ data.ToString("dd/MM/yyyy"),DayName="Este Mês" }
            };
    }
}

using System;
using System.Collections.Generic;
using System.Web;

public class BasePage : System.Web.UI.Page
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var perfis = HttpContext.Current.Session["perfis"] as List<int>;
        var url = HttpContext.Current.Request.Url.AbsolutePath;

        if (perfis == null || !ControleAcessoDb.UsuarioTemAcesso(url, perfis))
        {
            HttpContext.Current.Response.Redirect("~/SemPermissao.aspx");
        }
    }
}

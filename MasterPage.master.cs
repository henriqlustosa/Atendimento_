// Description: Code-behind for the MasterPage that handles user authentication, menu generation, and image loading.
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            CarregarMenu();
        }
    }

    private void CarregarMenu()
    {
        // Obtém os perfis do usuário da sessão
        List<int> perfis = HttpContext.Current.Session["perfis"] as List<int>;

        if (perfis == null || perfis.Count == 0)
        {
            menu.DataSource = new List<CustomMenuNode>(); // vazio
            menu.DataBind();
            return;
        }

        // Gera o menu baseado no SiteMap e nas permissões

        var menuPermitido = ObterMenuFiltrado(SiteMap.RootNode, perfis);
        menu.DataSource = menuPermitido;
        menu.DataBind();
    }

    private List<CustomMenuNode> ObterMenuFiltrado(SiteMapNode raiz, List<int> perfis)
    {
        var lista = new List<CustomMenuNode>();

        foreach (SiteMapNode node in raiz.ChildNodes)
        {
            bool temAcessoDireto = string.IsNullOrEmpty(node.Url) || !node.Url.Contains(".aspx") || ControleAcessoDb.UsuarioTemAcesso(node.Url, perfis);

            var customNode = new CustomMenuNode
            {
                Title = node.Title,
                Url = node.Url
            };

            foreach (SiteMapNode filho in node.ChildNodes)
            {
                if (!string.IsNullOrEmpty(filho.Url) && ControleAcessoDb.UsuarioTemAcesso(filho.Url, perfis))
                {
                    customNode.Children.Add(new CustomMenuNode
                    {
                        Title = filho.Title,
                        Url = filho.Url
                    });
                }
            }

            // Adiciona o nó se ele mesmo ou ao menos um filho tiver acesso
            if (temAcessoDireto || customNode.Children.Count > 0)
                lista.Add(customNode);
        }

        return lista;
    }
}


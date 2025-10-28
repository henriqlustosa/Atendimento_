using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class encaminhamento_pedidospendentes : BasePage
{
    private static string SafeTrim(string s)
    {
        return (s == null) ? "" : s.Trim();
    }

    private static string ToJsString(string s)
    {
        if (s == null) s = "";
        return "'" + s.Replace(@"\", @"\\").Replace("'", @"\'")
                      .Replace("\r", "").Replace("\n", "\\n") + "'";
    }

    private void ShowToast(string msg)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "msg",
            "alert(" + ToJsString(msg) + ");", true);
    }

    protected string FormatCargaGeral(object value)
    {
        string s = (value == null ? "" : value.ToString()).Trim().ToLower();
        return (s == "1" || s == "true" || s == "sim") ? "Sim" : "Não";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        lbTitulo.Text = "Solicitações de Exames Cadastrados (Pendentes)";
    }

    protected void GridView1_PreRender(object sender, EventArgs e)
    {
        if (GridView1.Rows.Count > 0 && GridView1.HeaderRow != null)
        {
            GridView1.UseAccessibleHeader = true;
            GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    // ===== Classe simples para o pager =====
    public class PageLink
    {
        public int Index { get; set; }     // zero-based
        public string Text { get; set; }   // "1", "2", ...
        public bool Active { get; set; }   // é a página atual?
        public bool Ellipsis { get; set; } // é um "..."
    }

    private void RebindAndUpdate()
    {
        GridView1.DataBind();
        if (updMain != null) updMain.Update(); // endRequest cuidará do initDT()
    }

    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        // Não renderizar pager nativo (evita <td colspan>)
        if (GridView1.BottomPagerRow != null) GridView1.BottomPagerRow.Visible = false;
        if (GridView1.TopPagerRow != null) GridView1.TopPagerRow.Visible = false;

        BindExternalPager();

        int page = GridView1.PageIndex + 1;
        int pages = GridView1.PageCount;
        if (lblPageInfo != null) lblPageInfo.Text = "Página " + page + " de " + pages;

        if (lnkPrev != null) lnkPrev.Enabled = (GridView1.PageIndex > 0);
        if (lnkFirst != null) lnkFirst.Enabled = (GridView1.PageIndex > 0);

        if (lnkNext != null) lnkNext.Enabled = (GridView1.PageIndex < pages - 1);
        if (lnkLast != null) lnkLast.Enabled = (GridView1.PageIndex < pages - 1);
    }

    private void BindExternalPager()
    {
        int totalPages = GridView1.PageCount;
        int current = GridView1.PageIndex; // zero-based
        int window = 2; // quantidade de páginas de cada lado

        var list = new System.Collections.Generic.List<PageLink>();
        if (totalPages <= 1)
        {
            rptPages.DataSource = list;
            rptPages.DataBind();
            return;
        }

        int start = Math.Max(0, current - window);
        int end = Math.Min(totalPages - 1, current + window);

        // primeira
        AddPage(list, 0, current);

        // ellipsis após a primeira?
        if (start > 1) list.Add(new PageLink { Ellipsis = true });

        // miolo (janela)
        for (int i = Math.Max(1, start); i <= Math.Min(end, totalPages - 2); i++)
            AddPage(list, i, current);

        // ellipsis antes da última?
        if (end < totalPages - 2) list.Add(new PageLink { Ellipsis = true });

        // última
        if (totalPages > 1) AddPage(list, totalPages - 1, current);

        rptPages.DataSource = list;
        rptPages.DataBind();
    }

    private static void AddPage(System.Collections.Generic.List<PageLink> list, int index, int current)
    {
        list.Add(new PageLink
        {
            Index = index,
            Text = (index + 1).ToString(),
            Active = (index == current),
            Ellipsis = false
        });
    }

    // ===== Eventos do pager externo =====

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        if (GridView1.PageIndex > 0) GridView1.PageIndex = 0;
        RebindAndUpdate();
    }

    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        if (GridView1.PageIndex > 0) GridView1.PageIndex--;
        RebindAndUpdate();
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        if (GridView1.PageIndex < GridView1.PageCount - 1) GridView1.PageIndex++;
        RebindAndUpdate();
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        if (GridView1.PageIndex < GridView1.PageCount - 1)
            GridView1.PageIndex = GridView1.PageCount - 1;
        RebindAndUpdate();
    }

    protected void rptPages_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page")
        {
            int index;
            if (int.TryParse((string)e.CommandArgument, out index))
            {
                if (index >= 0 && index < GridView1.PageCount)
                    GridView1.PageIndex = index;
                RebindAndUpdate();
            }
        }
    }

    protected void rptPages_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        var data = (PageLink)e.Item.DataItem;
        var lnk = (LinkButton)e.Item.FindControl("lnkPage");
        var lbl = (Label)e.Item.FindControl("lblEllipsis");

        if (data.Ellipsis)
        {
            if (lnk != null) lnk.Visible = false;
            if (lbl != null) lbl.Visible = true;
            return;
        }

        if (lbl != null) lbl.Visible = false;
        if (lnk != null)
        {
            lnk.Visible = true;
            lnk.Text = data.Text;
            lnk.CommandArgument = data.Index.ToString();
            lnk.Enabled = !data.Active;
            lnk.CssClass = data.Active ? "btn btn-primary btn-sm" : "btn btn-light btn-sm";
        }
    }

    protected void odsPendentes_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        string rh = SafeTrim(txbProntuario.Text);
        e.InputParameters["rh"] = string.IsNullOrEmpty(rh) ? null : rh;

        string sort = e.InputParameters["sortExpression"] as string;
        if (string.IsNullOrEmpty(sort))
            e.InputParameters["sortExpression"] = "cod_pedido DESC";
    }

    protected void btnPesquisar_Click(object sender, EventArgs e)
    {
        try
        {
            GridView1.PageIndex = 0;
            GridView1.DataBind();
        }
        catch (Exception ex)
        {
            ShowToast("Falha ao pesquisar: " + ex.Message);
        }
        finally
        {
            if (updMain != null) updMain.Update(); // endRequest fará o initDT()
        }
    }

    protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index;
        int idPedido;

        if (e.CommandName == "editRecord")
        {
            index = Convert.ToInt32(e.CommandArgument);
            idPedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString());
            Response.Redirect("~/encaminhamento/retornomarcado.aspx?idpedido=" + idPedido);
            return;
        }

        if (e.CommandName == "deleteRecord")
        {
            try
            {
                index = Convert.ToInt32(e.CommandArgument);
                idPedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString());
                PedidoDAO.deletePedidodeConsulta(idPedido);
                GridView1.DataBind();
                ShowToast("Registro excluído com sucesso.");
            }
            catch (Exception ex)
            {
                ShowToast("Erro ao excluir: " + ex.Message);
            }
            finally
            {
                if (updMain != null) updMain.Update(); // endRequest fará o initDT()
            }
            return;
        }
    }

    protected void btnConfirmarArquivo_Click(object sender, EventArgs e)
    {
        int id;
        if (hfPedidoId == null || hfPedidoId.Value == null || hfPedidoId.Value.Trim().Length == 0)
        {
            ShowToast("Nenhum pedido selecionado.");
            return;
        }

        if (!int.TryParse(hfPedidoId.Value, out id))
        {
            ShowToast("ID inválido.");
            return;
        }

        string retiradoPor = SafeTrim(txtRetiradoPor.Text);
        string rgCpf = SafeTrim(txtRgCpf.Text);
        string data = SafeTrim(txtData.Text);
        string hora = SafeTrim(txtHora.Text);

        string info = string.Format("Retirado por: {0} RG/CPF: {1} Data:{2} Hora:{3}",
            retiradoPor, rgCpf, data, hora);

        try
        {
            try { PedidoDAO.AtualizarOutrasInformacoes(id, info); } catch { }
            string usuario = (Session["login"] ?? "desconhecido").ToString();
            PedidoDAO.filePedidodeConsulta(id, usuario);

            // Limpa server-side
            txtRetiradoPor.Text = "";
            txtRgCpf.Text = "";
            txtData.Text = "";
            txtHora.Text = "";
            hfPedidoId.Value = "";
            GridView1.DataBind();
            ShowToast("Pedido arquivado com sucesso.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao arquivar: " + ex.Message);
        }
        finally
        {
            if (updMain != null)
                updMain.Update();

            // ✅ Fechar o modal e remover o backdrop do Bootstrap 5
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModal",
                "try { var modalEl = document.getElementById('modalArquivo'); " +
                "if (modalEl) { var m = bootstrap.Modal.getInstance(modalEl) || bootstrap.Modal.getOrCreateInstance(modalEl); m.hide(); } " +
                "$('.modal-backdrop').remove(); document.body.classList.remove('modal-open'); } catch(e){}", true);
        }
    }
}

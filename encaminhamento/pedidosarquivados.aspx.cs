using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class encaminhamento_pedidosarquivados : BasePage
{
    // Helpers
    private static string SafeTrim(string s) { return s == null ? "" : s.Trim(); }

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

    private static void EnsureGridHeader(GridView gv)
    {
        if (gv == null) return;

        if (gv.HeaderRow != null)
        {
            gv.UseAccessibleHeader = true;
            gv.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        if (gv.FooterRow != null)
        {
            gv.FooterRow.TableSection = TableRowSection.TableFooter;
        }
    }

    // Binds
    private void BindArquivadosTodos()
    {
        try
        {
            GridView1.DataSource = PedidoDAO.getListaPedidoConsultaArquivados();
            GridView1.DataBind();
            EnsureGridHeader(GridView1);

            if (GridView1.Rows.Count == 0)
                ShowToast("Nenhum registro arquivado encontrado.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao carregar arquivados: " + ex.Message);
            GridView1.DataSource = null;
            GridView1.DataBind();
        }
    }

    private void BindArquivadosPorRH(int prontuario)
    {
        try
        {
            // Se o campo prontuário não tiver valor, busca todos os registros
            if (prontuario > 0)
            {
                // Quando o prontuário é informado
                GridView1.DataSource = PedidoDAO.getListaPedidoConsultaArquivadaPorRH(prontuario);
            }
            else
            {
                // Quando não há prontuário informado, retorna tudo
                GridView1.DataSource = PedidoDAO.getListaPedidoConsultaArquivados();
            }

            GridView1.DataBind();
            EnsureGridHeader(GridView1);

            if (GridView1.Rows.Count == 0)
                ShowToast("Nenhum registro arquivado encontrado para o prontuário informado.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao carregar arquivados: " + ex.Message);
            GridView1.DataSource = null;
            GridView1.DataBind();
        }
    }

    // Eventos de Página
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        lbTitulo.Text = "Solicitações de Exames Arquivadas";
        // Carrega todos por padrão (ou deixe vazio se preferir carregar só após pesquisa)
        BindArquivadosTodos();
    }
    private static bool IsNullOrWhiteSpace(string value)
    {
        return value == null || value.Trim().Length == 0;
    }
    protected string FormatCargaGeral(object value)
    {
        var s = (value ?? "").ToString().Trim().ToLower();
        return (s == "1" || s == "true" || s == "sim") ? "Sim" : "Não";
    }
    protected void GridView1_PreRender(object sender, EventArgs e)
    {
        EnsureGridHeader(GridView1);
    }

    // Botão Pesquisar
    protected void btnPesquisar_Click(object sender, EventArgs e)
    {
        try
        {
            var s = SafeTrim(txbProntuario.Text);

            // Sem valor => traz todos
            if (IsNullOrWhiteSpace(s))
            {
                BindArquivadosTodos();
                return;
            }
            int pront;
            // Com valor => valida e filtra
            if (int.TryParse(s, out  pront) && pront > 0)
            {
                BindArquivadosPorRH(pront);
            }
            else
            {
                ShowToast("Prontuário inválido. Digite apenas números.");
                // opcional: ainda assim mostrar todos
                BindArquivadosTodos();
            }
        }
        catch (Exception ex)
        {
            ShowToast("Falha ao pesquisar: " + ex.Message);
        }
    }

    // Ações da Grid
    protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index;

        if (e.CommandName.Equals("printRecord"))
        {
            index = Convert.ToInt32(e.CommandArgument);
            int _id_pedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString());
            // Caso tenha uma rota de impressão dedicada:
            // Response.Redirect("~/Encaminhamento/impressaoArquivo.aspx?idpedido=" + _id_pedido);
            // Mantendo seu redirecionamento atual:
            Response.Redirect("~/Encaminhamento/pedidosarquivados.aspx");
        }

        if (e.CommandName.Equals("viewRecord"))
        {
            index = Convert.ToInt32(e.CommandArgument);
            int _id_pedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString());
            Response.Redirect("~/Encaminhamento/arquivomarcado.aspx?idpedido=" + _id_pedido);
        }
    }
}

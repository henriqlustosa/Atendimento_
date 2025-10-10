using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class encaminhamento_pedidospendentes : BasePage
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
    protected string FormatCargaGeral(object value)
    {
        var s = (value ?? "").ToString().Trim().ToLower();
        return (s == "1" || s == "true" || s == "sim") ? "Sim" : "Não";
    }

    // Binds
    private void BindPendentesTodos()
    {
        try
        {
            // Se houver método "todos"
            GridView1.DataSource = PedidoDAO.getListaPedidoConsultaPendente();
            GridView1.DataBind();
            EnsureGridHeader(GridView1);

            if (GridView1.Rows.Count == 0)
                ShowToast("Nenhum registro pendente encontrado.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao carregar pendentes: " + ex.Message);
            GridView1.DataSource = null;
            GridView1.DataBind();
        }
    }

    private void BindPendentesPorRH(int prontuario)
    {
        try
        {
            // Método com filtro por RH:
            // getListaPedidoConsultaPendentePorRH(int prontuario)
            GridView1.DataSource = PedidoDAO.getListaPedidoConsultaPendentePorRH(prontuario);
            GridView1.DataBind();
            EnsureGridHeader(GridView1);

            if (GridView1.Rows.Count == 0)
                ShowToast("Nenhum registro pendente encontrado para o prontuário informado.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao carregar pendentes: " + ex.Message);
            GridView1.DataSource = null;
            GridView1.DataBind();
        }
    }

    // Eventos de Página
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        lbTitulo.Text = "Solicitações de Exames Cadastrados (Pendentes)";

        // Carrega todos ao abrir (ou comente se preferir carregar só após pesquisar)
        BindPendentesTodos();
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
            string s = SafeTrim(txbProntuario.Text);
            if (s.Length == 0)
            {
                ShowToast("Informe o prontuário.");
                BindPendentesTodos();
                return;
            }

            int pront;
            if (!int.TryParse(s, out pront))
            {
                ShowToast("Prontuário inválido. Digite apenas números.");
                return;
            }

            BindPendentesPorRH(pront);
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

                string s = SafeTrim(txbProntuario.Text);
                int pront;
                if (int.TryParse(s, out pront))
                    BindPendentesPorRH(pront);
                else
                    BindPendentesTodos();

                ShowToast("Registro excluído com sucesso.");
            }
            catch (Exception ex)
            {
                ShowToast("Erro ao excluir: " + ex.Message);
            }
            return;
        }

        if (e.CommandName == "fileRecord")
        {
           
            try
            {
                index = Convert.ToInt32(e.CommandArgument);
                idPedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString());

                string usuario = (Session["login"] == null) ? "desconhecido" : Session["login"].ToString();
                PedidoDAO.filePedidodeConsulta(idPedido, usuario);

                string s = SafeTrim(txbProntuario.Text);
                int pront;
                if (int.TryParse(s, out pront))
                    BindPendentesPorRH(pront);
                else
                    BindPendentesTodos();

                ShowToast("Pedido arquivado com sucesso.");
            }
            catch (Exception ex)
            {
                ShowToast("Erro ao arquivar: " + ex.Message);
            }
         
            return;
        }
    }
}

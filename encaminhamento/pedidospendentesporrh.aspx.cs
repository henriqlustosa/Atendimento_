using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class encaminhamento_pedidospendentesporrh : BasePage
{
    // ---------------- Helpers ----------------
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
    protected string FormatCargaGeral(object value)
    {
        var s = (value ?? "").ToString().Trim().ToLower();
        return (s == "1" || s == "true" || s == "sim") ? "Sim" : "Não";
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

    // ---------------- Alternância de painéis (RBL) ----------------
    private void TogglePanels()
    {
        bool showPendentes = (rblTipo.SelectedValue == "P");
        pnlPendentes.Visible = showPendentes;
        pnlArquivadas.Visible = !showPendentes;

        lbTitulo.Text = showPendentes
            ? "Solicitações de Exames - Pendentes"
            : "Solicitações de Exames - Arquivadas";
    }

    // ---------------- Binds ----------------
    private void BindPendentes(int prontuario)
    {
        try
        {
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

    private void BindArquivados(int prontuario)
    {
        try
        {
            // Implemente este método no DAO:
            // getListaPedidoConsultaArquivadaPorRH(int prontuario)
            GridViewArquivados.DataSource = PedidoDAO.getListaPedidoConsultaArquivadaPorRH(prontuario);
            GridViewArquivados.DataBind();
            EnsureGridHeader(GridViewArquivados);

            if (GridViewArquivados.Rows.Count == 0)
                ShowToast("Nenhum registro arquivado encontrado para o prontuário informado.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao carregar arquivadas: " + ex.Message);
            GridViewArquivados.DataSource = null;
            GridViewArquivados.DataBind();
        }
    }

    // ---------------- Eventos de Página ----------------
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        if (string.IsNullOrEmpty(rblTipo.SelectedValue))
            rblTipo.SelectedValue = "P";

        TogglePanels();
        // carrega apenas quando pesquisar (mantido)
    }

    protected void rblTipo_SelectedIndexChanged(object sender, EventArgs e)
    {
        TogglePanels();

        int pront;
        string s = SafeTrim(txbProntuario.Text);
        if (int.TryParse(s, out pront))
        {
            if (rblTipo.SelectedValue == "P")
                BindPendentes(pront);
            else
                BindArquivados(pront);
        }
    }

    protected void GridView1_PreRender(object sender, EventArgs e)
    {
        EnsureGridHeader(GridView1);
    }

    protected void GridViewArquivados_PreRender(object sender, EventArgs e)
    {
        EnsureGridHeader(GridViewArquivados);
    }

    // ---------------- Ações da Grid (Pendentes) ----------------
    protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e == null || e.CommandArgument == null) return;

        int index;
        if (!int.TryParse(e.CommandArgument.ToString(), out index)) return;
        if (index < 0 || index >= GridView1.Rows.Count) return;

        int idPedido;
        if (!int.TryParse(GridView1.DataKeys[index].Value.ToString(), out idPedido)) return;

        if (e.CommandName == "editRecord")
        {
            Response.Redirect("~/encaminhamento/retornomarcado.aspx?idpedido=" + idPedido);
            return;
        }

        if (e.CommandName == "deleteRecord")
        {
            try
            {
                PedidoDAO.deletePedidodeConsulta(idPedido);

                int pront;
                if (int.TryParse(SafeTrim(txbProntuario.Text), out pront))
                    BindPendentes(pront);

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
            // arquivamento via modal (btnConfirmarArquivo_Click)
            return;
        }
    }

    // ---------------- Modal: Confirmar Arquivo ----------------
    protected void btnConfirmarArquivo_Click(object sender, EventArgs e)
    {
        if (hfPedidoId == null || hfPedidoId.Value == null || hfPedidoId.Value.Trim().Length == 0)
        {
            ShowToast("Nenhum pedido selecionado.");
            return;
        }

        int id;
        if (!int.TryParse(hfPedidoId.Value, out id))
        {
            ShowToast("ID inválido.");
            return;
        }

        string retiradoPor = SafeTrim(txtRetiradoPor.Text);
        string rgCpf = SafeTrim(txtRgCpf.Text);
        string data = SafeTrim(txtData.Text);
        string hora = SafeTrim(txtHora.Text);

        string info = string.Format(
            "Retirado por: {0} RG ou CPF: {1} Data:{2} Hora:{3}",
            retiradoPor, rgCpf, data, hora);

        try
        {
            try { PedidoDAO.AtualizarOutrasInformacoes(id, info); } catch { /* log opcional */ }

            string usuario = (Session["login"] == null) ? "desconhecido" : Session["login"].ToString();
            PedidoDAO.filePedidodeConsulta(id, usuario);

            int pront;
            if (int.TryParse(SafeTrim(txbProntuario.Text), out pront))
            {
                if (rblTipo.SelectedValue == "P")
                    BindPendentes(pront);
                else
                    BindArquivados(pront);
            }

            // Limpa campos do modal (server)
            txtRetiradoPor.Text = "";
            txtRgCpf.Text = "";
            txtData.Text = "";
            txtHora.Text = "";
            hfPedidoId.Value = "";

            // Fecha o modal no cliente (formato compatível c/ C#3 via string.Format)
            string closeAndClear = string.Format(@"
            (function () {{
              var el  = document.getElementById('modalArquivo');
              var ids = {{
                retirado:'{0}',
                rg:'{1}',
                data:'{2}',
                hora:'{3}',
                hid:'{4}'
              }};
              ['retirado','rg','data','hora','hid'].forEach(function(k){{
                var c = document.getElementById(ids[k]); if (c) c.value = '';
              }});

              if (window.bootstrap && typeof bootstrap.Modal === 'function') {{
                var m = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
                m.hide();
              }} else if (window.jQuery && jQuery.fn && jQuery.fn.modal) {{
                jQuery('#modalArquivo').modal('hide');
              }} else if (el) {{
                el.classList.remove('show'); el.style.display='none';
                document.body.classList.remove('modal-open');
                var backs = document.querySelectorAll('.modal-backdrop');
                for (var i = 0; i < backs.length; i++) {{
                  if (backs[i].remove) backs[i].remove();
                  else backs[i].parentNode.removeChild(backs[i]);
                }}
              }}
            }})();",
            txtRetiradoPor.ClientID,
            txtRgCpf.ClientID,
            txtData.ClientID,
            txtHora.ClientID,
            hfPedidoId.ClientID);

            ScriptManager.RegisterStartupScript(this, GetType(), "closeAndClearModal", closeAndClear, true);

            ShowToast("Pedido arquivado com sucesso.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao arquivar: " + ex.Message);
        }
    }

    // ---------------- Botão Pesquisar ----------------
    protected void btnPesquisar_Click(object sender, EventArgs e)
    {
        try
        {
            string s = SafeTrim(txbProntuario.Text);
            if (s.Length == 0)
            {
                ShowToast("Informe o prontuário.");
                GridView1.DataSource = null; GridView1.DataBind();
                GridViewArquivados.DataSource = null; GridViewArquivados.DataBind();
                return;
            }

            int pront;
            if (!int.TryParse(s, out pront))
            {
                ShowToast("Prontuário inválido. Digite apenas números.");
                GridView1.DataSource = null; GridView1.DataBind();
                GridViewArquivados.DataSource = null; GridViewArquivados.DataBind();
                return;
            }

            if (rblTipo.SelectedValue == "P")
                BindPendentes(pront);
            else
                BindArquivados(pront);
        }
        catch (Exception ex)
        {
            ShowToast("Falha ao pesquisar: " + ex.Message);
        }
    }
}

using System;
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

    private void ConfigureGridHeader()
    {
        if (GridView1.HeaderRow != null)
        {
            GridView1.UseAccessibleHeader = true;
            GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        if (GridView1.FooterRow != null)
        {
            GridView1.FooterRow.TableSection = TableRowSection.TableFooter;
        }
    }

    private void BindGrid(int prontuario)
    {
        try
        {
            GridView1.DataSource = PedidoDAO.getListaPedidoConsultaPendentePorRH(prontuario);
            GridView1.DataBind();
            ConfigureGridHeader();

            if (GridView1.Rows.Count == 0)
                ShowToast("Nenhum registro encontrado para o prontuário informado.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao carregar lista: " + ex.Message);
            GridView1.DataSource = null;
            GridView1.DataBind();
        }
    }

    // ---------------- Eventos de Página ----------------
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        // Se precisar, inicialize algo aqui.
    }

    protected void GridView1_PreRender(object sender, EventArgs e)
    {
        // Garante <thead>/<tfoot> em todo ciclo
        ConfigureGridHeader();
    }

    // ---------------- Ações da Grid ----------------
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
                    BindGrid(pront);

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
            // O arquivamento é feito via modal/btnConfirmarArquivo_Click.
            // Se quiser arquivar direto sem modal, implemente aqui.
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
    string rgCpf       = SafeTrim(txtRgCpf.Text);
    string data        = SafeTrim(txtData.Text);
    string hora        = SafeTrim(txtHora.Text);

    string info = string.Format(
        "Retirado por: {0} RG ou CPF: {1} Data:{2} Hora:{3}",
        retiradoPor, rgCpf, data, hora);

    try
    {
        try { PedidoDAO.AtualizarOutrasInformacoes(id, info); } catch { /* log opcional */ }

        string usuario = Session["login"] == null ? "desconhecido" : Session["login"].ToString();
        PedidoDAO.filePedidodeConsulta(id, usuario); // ajuste se seu DAO não aceitar 'usuario'

        // Recarrega a grid mantendo o filtro
        int pront;
        if (int.TryParse(SafeTrim(txbProntuario.Text), out pront))
            BindGrid(pront);

        // LIMPA os campos do modal (server)
        txtRetiradoPor.Text = "";
        txtRgCpf.Text       = "";
        txtData.Text        = "";
        txtHora.Text        = "";
        hfPedidoId.Value    = "";

        // Fecha o modal no cliente (BS5 + fallbacks) e limpa no DOM
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
            hfPedidoId.ClientID
        );

        ScriptManager.RegisterStartupScript(this, GetType(), "closeAndClearModal", closeAndClear, true);

        ShowToast("Pedido arquivado com sucesso.");
    }
    catch (Exception ex)
    {
        ShowToast("Erro ao arquivar: " + ex.Message);
    }
}


    protected void btnPesquisar_Click(object sender, EventArgs e)
    {
        try
        {
            string s = SafeTrim(txbProntuario.Text);
            if (s.Length == 0)
            {
                ShowToast("Informe o prontuário.");
                GridView1.DataSource = null;
                GridView1.DataBind();
                return;
            }

            int pront;
            if (!int.TryParse(s, out pront))
            {
                ShowToast("Prontuário inválido. Digite apenas números.");
                GridView1.DataSource = null;
                GridView1.DataBind();
                return;
            }

            BindGrid(pront);
        }
        catch (Exception ex)
        {
            ShowToast("Falha ao pesquisar: " + ex.Message);
        }
    }

   
}

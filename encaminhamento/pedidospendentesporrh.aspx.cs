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
        if (hfPedidoId == null || string.IsNullOrEmpty(hfPedidoId.Value)) return;

        int id;
        if (!int.TryParse(hfPedidoId.Value, out id)) return;

        string retiradoPor = SafeTrim(txtRetiradoPor.Text);
        string rgCpf = SafeTrim(txtRgCpf.Text);
        string data = SafeTrim(txtData.Text); // yyyy-MM-dd (ou conforme vem do input)
        string hora = SafeTrim(txtHora.Text); // HH:mm

        string info = string.Format("Retirado por: {0} RG ou CPF: {1} Data:{2} Hora:{3}",
            retiradoPor, rgCpf, data, hora);

        try
        {
            // Se existir o método, atualiza observações
            try { PedidoDAO.AtualizarOutrasInformacoes(id, info); } catch { /* opcional: log */ }

            // Arquiva
            PedidoDAO.filePedidodeConsulta(id);

            // Recarrega mantendo o filtro
            int pront;
            if (int.TryParse(SafeTrim(txbProntuario.Text), out pront))
                BindGrid(pront);

            // Fecha modal no client
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModal",
                "$('#modalArquivo').modal('hide');", true);

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

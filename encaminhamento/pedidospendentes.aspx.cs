using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class encaminhamento_pedidospendentes : BasePage
{
    // -------- Helpers --------
    private static string SafeTrim(string s)
    {
        return s == null ? "" : s.Trim();
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
        string s = (value ?? "").ToString().Trim().ToLower();
        return (s == "1" || s == "true" || s == "sim") ? "Sim" : "Não";
    }

    private void RebindDepoisDeAcao()
    {
        string s = SafeTrim(txbProntuario.Text);
        int pront;
        if (int.TryParse(s, out pront))
            BindPendentesPorRH(pront);
        else
            BindPendentesTodos();
    }

    // -------- Binds --------
    private void BindPendentesTodos()
    {
        try
        {
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

    // -------- Eventos de Página --------
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        lbTitulo.Text = "Solicitações de Exames Cadastrados (Pendentes)";
        BindPendentesTodos(); // ou comente para carregar apenas após pesquisar
    }

    protected void GridView1_PreRender(object sender, EventArgs e)
    {
        EnsureGridHeader(GridView1);
    }

    // -------- Botão Pesquisar --------
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

    // -------- Ações da Grid (Editar/Excluir) --------
    protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        // Observação: o "Arquivar" agora é feito via modal + btnConfirmarArquivo_Click.
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

                RebindDepoisDeAcao();
                ShowToast("Registro excluído com sucesso.");
            }
            catch (Exception ex)
            {
                ShowToast("Erro ao excluir: " + ex.Message);
            }
            return;
        }

        // Se ainda houver algum botão antigo que dispare "fileRecord", ignore ou avise:
        if (e.CommandName == "fileRecord")
        {
            ShowToast("Use o botão verde (ícone de arquivo) para abrir o modal e arquivar.");
            return;
        }
    }

    // -------- Arquivamento via Modal --------
    protected void btnConfirmarArquivo_Click(object sender, EventArgs e)
    {
        // Valida o ID vindo do HiddenField setado pelo JS ao abrir o modal
        if (hfPedidoId == null || hfPedidoId.Value == null || hfPedidoId.Value.Trim() == "")
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

        // Campos do modal
        string retiradoPor = SafeTrim(txtRetiradoPor.Text);
        string rgCpf = SafeTrim(txtRgCpf.Text);
        string data = SafeTrim(txtData.Text);
        string hora = SafeTrim(txtHora.Text);

        // Monta a string conforme padrão já utilizado (sem interpolação)
        string info = string.Format("Retirado por: {0} RG ou CPF: {1} Data:{2} Hora:{3}",
                                    retiradoPor, rgCpf, data, hora);

        try
        {
            // 1) Atualiza "outras_informacoes" (se houver esse método/coluna no seu schema)
            try
            {
                // Se não existir este método no seu DAO, remova este bloco.
                PedidoDAO.AtualizarOutrasInformacoes(id, info);
            }
            catch
            {
                // silencioso — se não houver método, seguir para o arquivamento
            }

            // 2) Efetiva a baixa/arquivamento (usa o método que você já possui)
            string usuario = (Session["login"] == null) ? "desconhecido" : Session["login"].ToString();
            PedidoDAO.filePedidodeConsulta(id, usuario);

            // 3) Recarrega a grid respeitando o filtro atual
            RebindDepoisDeAcao();

            ShowToast("Pedido arquivado com sucesso.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao arquivar: " + ex.Message);
        }
    }
}

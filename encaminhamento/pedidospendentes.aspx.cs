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
            gv.FooterRow.TableSection = TableRowSection.TableFooter;
    }

    // Tenta ler o filtro do campo "RH"; se não existir, usa txbProntuario
    private string GetRhFiltro()
    {
        TextBox rhCtl = FindControl("txbRH") as TextBox; // novo campo RH (se existir)
        if (rhCtl != null) return SafeTrim(rhCtl.Text);
        return SafeTrim(txbProntuario.Text);             // compatibilidade com páginas antigas
    }

    protected string FormatCargaGeral(object value)
    {
        string s = (value == null ? "" : value.ToString()).Trim().ToLower();
        return (s == "1" || s == "true" || s == "sim") ? "Sim" : "Não";
    }

    private void RebindDepoisDeAcao()
    {
        string rh = GetRhFiltro();

        // NOVA REGRA: RH vazio => retorna todos
        if (rh.Length == 0)
        {
            BindPendentesTodos();
            return;
        }

        // Se sua busca por RH for numérica (ex.: prontuário), mantém o TryParse
        int pront;
        if (int.TryParse(rh, out pront))
        {
            BindPendentesPorRH(pront);
        }
        else
        {
            // Caso RH precise ser string, troque por um método string no DAO:
            // BindPendentesPorRHString(rh);
            // Enquanto isso, evita quebra e mostra todos.
            ShowToast("Filtro de RH inválido. Exibindo todos.");
            BindPendentesTodos();
        }
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
                ShowToast("Nenhum registro pendente encontrado para o filtro informado.");
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
        BindPendentesTodos(); // carrega tudo ao abrir
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
            string rh = GetRhFiltro();

            // NOVA REGRA: RH vazio => retorna todos (sem exigir nada do usuário)
            if (rh.Length == 0)
            {
                BindPendentesTodos();
                return;
            }

            // Se seu filtro de RH é numérico
            int pront;
            if (int.TryParse(rh, out pront))
            {
                BindPendentesPorRH(pront);
                return;
            }

            // Se precisar aceitar RH alfanumérico, crie no DAO um método string:
            // GridView1.DataSource = PedidoDAO.getListaPedidoConsultaPendentePorRH(rh);
            // GridView1.DataBind();
            // EnsureGridHeader(GridView1);
            // return;

            // Fallback até existir método string:
            ShowToast("Filtro de RH inválido. Exibindo todos.");
            BindPendentesTodos();
        }
        catch (Exception ex)
        {
            ShowToast("Falha ao pesquisar: " + ex.Message);
        }
    }

    // -------- Ações da Grid (Editar/Excluir) --------
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

                RebindDepoisDeAcao();
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
            ShowToast("Use o botão verde (ícone de arquivo) para abrir o modal e arquivar.");
            return;
        }
    }

    // -------- Arquivamento via Modal --------
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

        string info = string.Format("Retirado por: {0} RG ou CPF: {1} Data:{2} Hora:{3}",
                                    retiradoPor, rgCpf, data, hora);

        try
        {
            try { PedidoDAO.AtualizarOutrasInformacoes(id, info); } catch { /* opcional log */ }

            string usuario = (Session["login"] == null) ? "desconhecido" : Session["login"].ToString();
            PedidoDAO.filePedidodeConsulta(id, usuario);

            RebindDepoisDeAcao();
            ShowToast("Pedido arquivado com sucesso.");
        }
        catch (Exception ex)
        {
            ShowToast("Erro ao arquivar: " + ex.Message);
        }
    }
}

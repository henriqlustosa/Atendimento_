using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Hspm.CadEncaminhamento;
using Hspm.CadEncaminhamento.Domain;
using Hspm.CadEncaminhamento.Application;

public partial class encaminhamento_retornomarcado : BasePage
{
    private readonly IEspecialidadeCatalog _especialidades;
    private readonly IExameCatalog _exames;

    private readonly IPedidoQuery _pedidoQuery;                 // leitura p/ edição
    private readonly IAtualizarPedidoHandler _atualizarPedido;  // salvamento da edição

    public encaminhamento_retornomarcado()
    {
        _especialidades = CompositionRoot.Especialidades;
        _exames = CompositionRoot.Exames;
        _pedidoQuery = CompositionRoot.PedidoQuery;
        _atualizarPedido = CompositionRoot.AtualizarPedido;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        // 1) Fontes (mesmo padrão do cadastro)
        BindDropDown(ddlEspecialidade, _especialidades.Listar());
        BindHtmlSelect(select2, _exames.ListarPreOperatorio());
        BindHtmlSelect(select1, _exames.ListarRessonancia());
        BindHtmlSelect(select3, _exames.ListarTeleconsulta());
        BindHtmlSelect(select4, _exames.ListarExamesUnicos());

        if (ddlEspecialidade.Items.Count == 0 || ddlEspecialidade.Items[0].Value != "")
            ddlEspecialidade.Items.Insert(0, new ListItem("Selecione...", ""));
        ddlEspecialidade.SelectedIndex = 0;

        // 2) idpedido
        int idPedido;
        if (!int.TryParse(Request.QueryString["idpedido"], out idPedido) || idPedido <= 0)
        {
            AddPageError("Parâmetro 'idpedido' ausente ou inválido.");
            return;
        }
        hfPedidoId.Value = idPedido.ToString(CultureInfo.InvariantCulture);

        // 3) Carrega dados
        CarregarPedido(idPedido);
    }

    // -------------------- Carregar dados --------------------
    private void CarregarPedido(int idPedido)
    {
        PedidoDetailsDto p = _pedidoQuery.ObterPorId(idPedido);
        if (p == null)
        {
            AddPageError("Pedido não encontrado.");
            return;
        }

        txbProntuario.Text = p.Prontuario.ToString(CultureInfo.InvariantCulture);
        txbNomePaciente.Text = (p.NomePaciente ?? "").Trim().ToUpperInvariant();
        txbDtPedido.Text = p.DataPedido.ToString("dd/MM/yyyy");
        txbOb.Text = p.Observacoes ?? "";

        // Especialidade
        ListItem it = ddlEspecialidade.Items.FindByValue(
            p.CodEspecialidade.ToString(CultureInfo.InvariantCulture)
        );
        if (it != null)
        {
            ddlEspecialidade.ClearSelection();
            it.Selected = true;
        }

        // Radios de Carga Geral (1 = Sim, 0 = Não)
        rbCargaSim.Checked = (p.CargaGeral == 1);
        rbCargaNao.Checked = !rbCargaSim.Checked;

        // Seções de seleção múltipla
        ApplySelected(select1, p.Ressonancia);
        ApplySelected(select2, p.PreOperatorio);
        ApplySelected(select3, p.Teleconsulta);
        ApplySelected(select4, p.ExamesUnicos);
    }


    private static void ApplySelected(HtmlSelect sel, IList<int> codes)
    {
        if (sel == null || codes == null || codes.Count == 0) return;
        var set = new HashSet<string>(StringComparer.InvariantCulture);
        for (int i = 0; i < codes.Count; i++)
            set.Add(codes[i].ToString(CultureInfo.InvariantCulture));
        for (int i = 0; i < sel.Items.Count; i++)
            sel.Items[i].Selected = set.Contains(sel.Items[i].Value);
    }

    // -------------------- Salvar (atualizar) --------------------
    protected void btnGravar_Click(object sender, EventArgs e)
    {
        int idPedido;
        if (!int.TryParse(hfPedidoId.Value, out idPedido) || idPedido <= 0)
        {
            AddPageError("ID do pedido inválido.");
            return;
        }

        int prontuario, codEsp; DateTime dtPedido;
        if (!TryValidateInputsForEdit(out prontuario, out dtPedido, out codEsp))
            return;

        string historicoTodosExames = JoinSelectedTexts(select1, select2, select3, select4);

        var cmd = new AtualizarPedidoCommand
        {
            Id = idPedido,
            Prontuario = prontuario,
            NomePaciente = (txbNomePaciente.Text ?? "").Trim().ToUpperInvariant(),
            DataPedido = dtPedido,
            CodEspecialidade = codEsp,
            Observacoes = txbOb.Text,
            
            Usuario = Session["login"] == null ? "desconhecido" : Session["login"].ToString(),

            CodigosPreOperatorio = GetSelectedCodes(select2),
            CodigosRessonancia = GetSelectedCodes(select1),
            CodigosTeleconsulta = GetSelectedCodes(select3),
            CodigosExamesUnicos = GetSelectedCodes(select4),

            ExamesPreOpTextoParaHistorico = historicoTodosExames,

            // NOVO: 1 = Sim, 0 = Não
            CargaGeral = rbCargaSim.Checked ? 1 : 0
        };

        try
        {
            _atualizarPedido.Handle(cmd);
            ShowSuccessModal(); // abre #myModal
        }
        catch (ApplicationException ex)
        {
            AddPageError(ex.Message.Replace("\n", "<br/>"));
        }
        catch (Exception ex)
        {
            AddPageError("Falha ao atualizar: " + ex.Message);
        }
    }


    private bool TryValidateInputsForEdit(out int prontuario, out DateTime dtPedido, out int codEsp)
    {
        prontuario = 0;
        dtPedido = default(DateTime);
        codEsp = 0;

        // Normalização
        txbProntuario.Text = (txbProntuario.Text ?? "").Trim();
        txbDtPedido.Text = (txbDtPedido.Text ?? "").Trim();
        txbNomePaciente.Text = (txbNomePaciente.Text ?? "").Trim();

        var errors = new List<string>();

        // Prontuário
        if (!TryGetIntPositive(txbProntuario.Text, out prontuario))
            errors.Add("Prontuário inválido.");

        // Data do pedido (pt-BR)
        if (!TryParseDatePtBr(txbDtPedido.Text, out dtPedido))
            errors.Add("Data do pedido inválida.");

        // Especialidade
        if (!TryGetIntPositive(ddlEspecialidade.SelectedValue, out codEsp))
            errors.Add("Selecione a especialidade.");

        // Pelo menos um exame
        if (!HasAnySelection(select2, select1, select3, select4))
            errors.Add("Selecione ao menos um item em Pré-operatório, Ressonância, Teleconsulta ou Exames Únicos.");

        // Nome do paciente
        if (string.IsNullOrEmpty(txbNomePaciente.Text))
            errors.Add("Informe o nome do paciente.");

        // Radios de Carga (Sim/Não)
        if (!rbCargaSim.Checked && !rbCargaNao.Checked)
            errors.Add("Informe se o formulário é relacionado a carga (Sim ou Não).");

        // Respeita validadores ASP.NET e lista de erros
        if (!Page.IsValid || errors.Count > 0)
        {
            foreach (var err in errors) AddPageError(err);
            return false;
        }

        return true;
    }


    private static IList<int> GetSelectedCodes(HtmlSelect sel)
    {
        var list = new List<int>();
        if (sel == null) return list;
        for (int i = 0; i < sel.Items.Count; i++)
        {
            ListItem it = sel.Items[i];
            if (!it.Selected) continue;
            int v; if (int.TryParse(it.Value, out v)) list.Add(v);
        }
        return list;
    }

    private static string JoinSelectedTexts(params HtmlSelect[] selects)
    {
        var parts = new List<string>();
        for (int s = 0; s < selects.Length; s++)
        {
            HtmlSelect sel = selects[s];
            for (int i = 0; i < sel.Items.Count; i++)
                if (sel.Items[i].Selected) parts.Add(sel.Items[i].Text);
        }
        return string.Join(", ", parts.ToArray());
    }

    // -------------------- Helpers (mesmo padrão do cadastro) --------------------
    private static void BindDropDown(ListControl ddl, IList<ListItemDto> data)
    {
        ddl.Items.Clear();
        ddl.Items.Add(new ListItem("Selecione...", ""));
        for (int i = 0; i < data.Count; i++)
            ddl.Items.Add(new ListItem(data[i].Text, data[i].Value.ToString(CultureInfo.InvariantCulture)));
        ddl.SelectedIndex = 0;
    }

    private static void BindHtmlSelect(HtmlSelect sel, IList<ListItemDto> data)
    {
        sel.Items.Clear();
        for (int i = 0; i < data.Count; i++)
            sel.Items.Add(new ListItem(data[i].Text, data[i].Value.ToString(CultureInfo.InvariantCulture)));
        RemoveDuplicateByText(sel.Items);
    }

    private static void RemoveDuplicateByText(ListItemCollection items)
    {
        var seen = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        for (int i = items.Count - 1; i >= 0; i--)
        {
            string t = (items[i].Text ?? "").Trim();
            if (t.Length == 0 || !seen.Add(t))
                items.RemoveAt(i);
        }
    }

    private static bool TryGetIntPositive(string input, out int value)
    {
        if (int.TryParse(input, out value) && value > 0) return true;
        value = 0; return false;
    }

    private static bool HasAnySelection(params HtmlSelect[] selects)
    {
        for (int i = 0; i < selects.Length; i++)
        {
            HtmlSelect s = selects[i];
            for (int j = 0; j < s.Items.Count; j++)
                if (s.Items[j].Selected) return true;
        }
        return false;
    }

    private static bool TryParseDatePtBr(string input, out DateTime result)
    {
        input = (input ?? "").Trim();
        string[] formats = new string[] { "d/M/yyyy", "dd/MM/yyyy" };
        return DateTime.TryParseExact(input, formats, new CultureInfo("pt-BR"),
                                      DateTimeStyles.None, out result);
    }

    private void AddPageError(string message)
    {
        var cv = new CustomValidator();
        cv.IsValid = false;
        cv.ErrorMessage = message;
        cv.ValidationGroup = "Salvar";
        cv.Display = ValidatorDisplay.None;
        Page.Validators.Add(cv);
    }

    private void ShowSuccessModal()
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "showOkModal",
            "var m = bootstrap.Modal.getOrCreateInstance(document.getElementById('myModal')); m.show();", true);
    }
}

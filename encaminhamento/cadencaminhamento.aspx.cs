using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Hspm.CadEncaminhamento.Domain;
using Hspm.CadEncaminhamento.Application;
using Hspm.CadEncaminhamento;

public partial class publico_cadencaminhamento : BasePage
{
    private readonly IPacienteGateway _pacientes;
    private readonly IEspecialidadeCatalog _especialidades;
    private readonly IExameCatalog _exames;
    private readonly IGravarPedidoHandler _gravarPedido;
    protected void cvCargaRel_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Validação servidor: exige que ao menos um dos radios esteja marcado
        args.IsValid = rbCargaSim.Checked || rbCargaNao.Checked;
    }
    public publico_cadencaminhamento()
    {
        _pacientes = CompositionRoot.Pacientes;
        _especialidades = CompositionRoot.Especialidades;
        _exames = CompositionRoot.Exames;
        _gravarPedido = CompositionRoot.GravarPedido;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        BindDropDown(ddlEspecialidade, _especialidades.Listar());
        BindHtmlSelect(select2, _exames.ListarPreOperatorio());
        BindHtmlSelect(select1, _exames.ListarRessonancia());
        BindHtmlSelect(select3, _exames.ListarTeleconsulta());
        BindHtmlSelect(select4, _exames.ListarExamesUnicos());
        rbCargaNao.Checked = true;  // padrão “Não”
        rbCargaSim.Checked = false;
        if (ddlEspecialidade.Items.Count == 0 || ddlEspecialidade.Items[0].Value != "")
            ddlEspecialidade.Items.Insert(0, new ListItem("Selecione...", ""));
        ddlEspecialidade.SelectedIndex = 0;
    }

    // -------------------- Bind Helpers --------------------

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

    // -------------------- Ações --------------------

    protected void btnPesquisapaciente_Click(object sender, EventArgs e)
    {
        txbNomePaciente.Text = "";

        int prontuario;
        if (!int.TryParse(txbProntuario.Text, out prontuario))
        {
            AddPageError("Prontuário inválido.");
            return;
        }

        PacienteDto p = _pacientes.ObterPorProntuario(prontuario);
        if (p == null || string.IsNullOrEmpty(p.Nome))
        {
            AddPageError("Paciente não encontrado.");
            return;
        }

        txbNomePaciente.Text = p.Nome.ToUpperInvariant();
    }

    protected void cvAlgumExame_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = HasAnySelection(select2, select1, select3, select4);
    }

    protected void btnGravar_Click(object sender, EventArgs e)
    {
        int prontuario;
        DateTime dtPedido;
        int codEsp;

        if (!TryValidateInputs(out prontuario, out dtPedido, out codEsp))
            return;

        string historicoTodosExames = JoinSelectedTexts(select1, select2, select3, select4);

        var cmd = new GravarPedidoCommand
        {
            Prontuario = prontuario,
            NomePaciente = txbNomePaciente.Text.Trim().ToUpperInvariant(),
            DataPedido = dtPedido,
            CodEspecialidade = codEsp,
            OutrasInformacoes = txbOb.Text,
            CargaGeral = rbCargaSim.Checked ? 1 : 0,
            Usuario = Session["login"] == null ? "desconhecido" : Session["login"].ToString(),
            ExamesPreOpTextoParaHistorico = historicoTodosExames,
            Exames = BuildExamesSelecionados()
        };

        try
        {
            int id = _gravarPedido.Handle(cmd);

            // 1) Limpa todos os campos (server-side)
            ClearAllFields();

            // 2) Reseta UI no cliente (bootstrap-select + flatpickr)
            RegisterClientResetScripts();

            // 3) Fecha todos os acordeões
            CloseAllAccordions();

            // 4) Mostra modal de sucesso (Bootstrap 5)
            ShowSuccessModal();
        }
        catch (ApplicationException ex)
        {
            AddPageError(ex.Message.Replace("\n", "<br/>"));
        }
        catch (Exception ex)
        {
            AddPageError("Falha inesperada: " + ex.Message);
        }
    }

    // -------------------- Validações/Build --------------------

    private bool TryValidateInputs(out int prontuario, out DateTime dtPedido, out int codEsp)
    {
        prontuario = 0;
        dtPedido = default(DateTime);
        codEsp = 0;

        // Normalização básica
        txbProntuario.Text = (txbProntuario.Text ?? "").Trim();
        txbDtPedido.Text = (txbDtPedido.Text ?? "").Trim();
        txbNomePaciente.Text = (txbNomePaciente.Text ?? "").Trim();

        // Se houver validações ASP.NET que já falharam, registramos uma msg geral
        if (!Page.IsValid)
            AddPageError("Existem campos obrigatórios não preenchidos ou inválidos.");

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

        // Ao menos um exame selecionado (Pré-op, Ressonância, Teleconsulta ou Exames Únicos)
        if (!HasAnySelection(select2, select1, select3, select4))
            errors.Add("Selecione pelo menos um item em Pré-operatório, Ressonância, Teleconsulta ou Exames Únicos.");

        // Nome do paciente deve estar preenchido após a busca
        if (string.IsNullOrEmpty(txbNomePaciente.Text))
            errors.Add("Pesquise o paciente para preencher o nome.");

        // Nova regra: radios de carga precisam ter uma escolha (Sim OU Não)
        // (Geralmente o padrão é 'Não', mas garantimos aqui.)
        if (!rbCargaSim.Checked && !rbCargaNao.Checked)
            errors.Add("Informe se o formulário é relacionado a carga (Sim ou Não).");

        if (!Page.IsValid || errors.Count > 0)
        {
            foreach (var err in errors) AddPageError(err);
            return false;
        }

        return true;
    }

    private IList<ExameSelecionado> BuildExamesSelecionados()
    {
        var list = new List<ExameSelecionado>();
        AddFromSelect(list, select2, "PreOp");
        AddFromSelect(list, select1, "Ressonancia");
        AddFromSelect(list, select3, "Teleconsulta");
        AddFromSelect(list, select4, "Unico");
        return list;
    }

    private static void AddFromSelect(IList<ExameSelecionado> list, HtmlSelect s, string grupo)
    {
        foreach (ListItem it in s.Items)
        {
            if (!it.Selected) continue;
            int code; int.TryParse(it.Value, out code);
            list.Add(new ExameSelecionado(code, it.Text, grupo));
        }
    }

    private static string JoinSelectedTexts(params HtmlSelect[] selects)
    {
        var parts = new List<string>();
        for (int i = 0; i < selects.Length; i++)
        {
            var s = selects[i];
            for (int j = 0; j < s.Items.Count; j++)
            {
                var it = s.Items[j];
                if (it.Selected) parts.Add(it.Text);
            }
        }
        return string.Join(", ", parts.ToArray());
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
            var s = selects[i];
            for (int j = 0; j < s.Items.Count; j++)
                if (s.Items[j].Selected) return true;
        }
        return false;
    }

    private static bool TryParseDatePtBr(string input, out DateTime result)
    {
        input = (input ?? "").Trim();
        string[] formats = new[] { "d/M/yyyy", "dd/MM/yyyy" };
        return DateTime.TryParseExact(
            input, formats, new CultureInfo("pt-BR"),
            DateTimeStyles.None, out result
        );
    }

    private void AddPageError(string message)
    {
        var cv = new CustomValidator
        {
            IsValid = false,
            ErrorMessage = message,
            ValidationGroup = "Salvar",
            Display = ValidatorDisplay.None
        };
        Page.Validators.Add(cv);
    }

    // -------------------- Pós-salvamento: limpar/atualizar UI --------------------

    private void ClearAllFields()
    {
        // TextBoxes
        txbProntuario.Text = string.Empty;
        txbNomePaciente.Text = string.Empty;
        txbDtPedido.Text = string.Empty;
        txbOb.Text = string.Empty;

        // RadioButtons de carga (padrão: "Não" marcado)
        rbCargaSim.Checked = false;
        rbCargaNao.Checked = true;

        // DropDownList de especialidade (volta para "Selecione...")
        ddlEspecialidade.ClearSelection();
        var first = ddlEspecialidade.Items.FindByValue("") ??
                    (ddlEspecialidade.Items.Count > 0 ? ddlEspecialidade.Items[0] : null);
        if (first != null) first.Selected = true;

        // HtmlSelect (remove seleções)
        ClearHtmlSelect(select1);
        ClearHtmlSelect(select2);
        ClearHtmlSelect(select3);
        ClearHtmlSelect(select4);
    }


    private static void ClearHtmlSelect(HtmlSelect sel)
    {
        if (sel == null) return;
        foreach (ListItem li in sel.Items) li.Selected = false;
    }

    private void RegisterClientResetScripts()
    {
        // DeselectAll + render (bootstrap-select) e limpar flatpickr
        var js = @"
(function(){
  if (window.jQuery){
    try {
      $('.selectpicker').each(function(){
        $(this).selectpicker('deselectAll');
        $(this).selectpicker('render');
      });
    } catch(e){}
  }
  var fpEl = document.getElementById('" + txbDtPedido.ClientID + @"');
  if (fpEl && fpEl._flatpickr){ fpEl._flatpickr.clear(); }
})();";
        ScriptManager.RegisterStartupScript(this, GetType(), "resetUiAfterSave", js, true);
    }

    private void CloseAllAccordions()
    {
        var js = @"
(function(){
  var acc = document.getElementById('accAtendimento');
  if(!acc) return;
  acc.querySelectorAll('.collapse.show').forEach(function(el){
    try { bootstrap.Collapse.getOrCreateInstance(el, { toggle:false }).hide(); } catch(e){}
  });
  try { window.scrollTo({ top: 0, behavior: 'smooth' }); } catch(e) { window.scrollTo(0,0); }
})();";
        ScriptManager.RegisterStartupScript(this, GetType(), "collapseAllAfterSave", js, true);
    }

    private void ShowSuccessModal()
    {
        var js = "var m = bootstrap.Modal.getOrCreateInstance(document.getElementById('myModal')); m.show();";
        ScriptManager.RegisterStartupScript(this, GetType(), "showOkModal", js, true);
    }
}

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

        if (ddlEspecialidade.Items.Count == 0 || ddlEspecialidade.Items[0].Value != "")
            ddlEspecialidade.Items.Insert(0, new ListItem("Selecione...", ""));
        ddlEspecialidade.SelectedIndex = 0;
    }

    private static void BindDropDown(ListControl ddl, IList<ListItemDto> data)
    {
        ddl.Items.Clear();
        ddl.Items.Add(new ListItem("Selecione...", ""));
        int i;
        for (i = 0; i < data.Count; i++)
            ddl.Items.Add(new ListItem(data[i].Text, data[i].Value.ToString(CultureInfo.InvariantCulture)));
        ddl.SelectedIndex = 0;
    }

    private static void BindHtmlSelect(HtmlSelect sel, IList<ListItemDto> data)
    {
        sel.Items.Clear();
        int i;
        for (i = 0; i < data.Count; i++)
            sel.Items.Add(new ListItem(data[i].Text, data[i].Value.ToString(CultureInfo.InvariantCulture)));
        RemoveDuplicateByText(sel.Items);
    }

    private static void RemoveDuplicateByText(ListItemCollection items)
    {
        System.Collections.Generic.HashSet<string> seen =
            new System.Collections.Generic.HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        int i;
        for (i = items.Count - 1; i >= 0; i--)
        {
            string t = (items[i].Text ?? "").Trim();
            if (t.Length == 0 || !seen.Add(t))
                items.RemoveAt(i);
        }
    }

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
        int prontuario, codEsp;
        DateTime dtPedido;

        if (!TryValidateInputs(out prontuario, out dtPedido, out codEsp))
            return;

        string historicoTodosExames = JoinSelectedTexts(select1, select2, select3, select4);

        GravarPedidoCommand cmd = new GravarPedidoCommand();
        cmd.Prontuario = prontuario;
        cmd.NomePaciente = txbNomePaciente.Text.Trim().ToUpperInvariant();
        cmd.DataPedido = dtPedido;
        cmd.CodEspecialidade = codEsp;
        cmd.OutrasInformacoes = txbOb.Text;
        cmd.Solicitante = (txbprofissional.Text ?? "").Trim().ToUpperInvariant();
        cmd.Usuario = Session["login"] == null ? "desconhecido" : Session["login"].ToString();
        cmd.ExamesPreOpTextoParaHistorico = historicoTodosExames;
        cmd.Exames = BuildExamesSelecionados();

        try
        {
            int id = _gravarPedido.Handle(cmd);

            ScriptManager.RegisterStartupScript(
                Page, GetType(), "ok",
                "$(function(){ $('#myModal').modal(); });",
                true
            );
        }
        catch (ApplicationException ex)
        {
            AddPageError(ex.Message.Replace("\n", "<br/>"));
        }
        catch
        {
            AddPageError("Ocorreu um erro ao gravar o pedido. Tente novamente.");
        }
    }

    private bool TryValidateInputs(out int prontuario, out DateTime dtPedido, out int codEsp)
    {
        prontuario = 0;
        dtPedido = default(DateTime);
        codEsp = 0;

        txbProntuario.Text = (txbProntuario.Text ?? "").Trim();
        txbDtPedido.Text = (txbDtPedido.Text ?? "").Trim();
        txbNomePaciente.Text = (txbNomePaciente.Text ?? "").Trim();
        txbprofissional.Text = (txbprofissional.Text ?? "").Trim();

        if (!Page.IsValid)
            AddPageError("Existem campos obrigatórios não preenchidos ou inválidos.");

        System.Collections.Generic.List<string> errors = new System.Collections.Generic.List<string>();

        if (!TryGetIntPositive(txbProntuario.Text, out prontuario))
            errors.Add("Prontuário inválido.");

        if (!TryParseDatePtBr(txbDtPedido.Text, out dtPedido))
            errors.Add("Data do pedido inválida.");

        if (!TryGetIntPositive(ddlEspecialidade.SelectedValue, out codEsp))
            errors.Add("Selecione a especialidade.");

        if (!HasAnySelection(select2, select1, select3, select4))
            errors.Add("Selecione pelo menos um item em Pré-operatório, Ressonância, Teleconsulta ou Exames Únicos.");

        if (string.IsNullOrEmpty(txbNomePaciente.Text))
            errors.Add("Pesquise o paciente para preencher o nome.");

        if (!Page.IsValid || errors.Count > 0)
        {
            int i;
            for (i = 0; i < errors.Count; i++) AddPageError(errors[i]);
            return false;
        }

        return true;
    }

    private System.Collections.Generic.IList<ExameSelecionado> BuildExamesSelecionados()
    {
        System.Collections.Generic.List<ExameSelecionado> list = new System.Collections.Generic.List<ExameSelecionado>();
        AddFromSelect(list, select2, "PreOp");
        AddFromSelect(list, select1, "Ressonancia");
        AddFromSelect(list, select3, "Teleconsulta");
        AddFromSelect(list, select4, "Unico");
        return list;
    }

    private static void AddFromSelect(System.Collections.Generic.IList<ExameSelecionado> list, HtmlSelect s, string grupo)
    {
        foreach (ListItem it in s.Items)
        {
            if (!it.Selected) continue;
            int code;
            int.TryParse(it.Value, out code);
            list.Add(new ExameSelecionado(code, it.Text, grupo));
        }
    }

    private static string JoinSelectedTexts(params HtmlSelect[] selects)
    {
        System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
        int i; int j;
        for (i = 0; i < selects.Length; i++)
        {
            HtmlSelect s = selects[i];
            for (j = 0; j < s.Items.Count; j++)
            {
                ListItem it = s.Items[j];
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
        int i; int j;
        for (i = 0; i < selects.Length; i++)
        {
            HtmlSelect s = selects[i];
            for (j = 0; j < s.Items.Count; j++)
            {
                if (s.Items[j].Selected) return true;
            }
        }
        return false;
    }

    private static bool TryParseDatePtBr(string input, out DateTime result)
    {
        input = (input ?? "").Trim();
        string[] formats = new string[] { "d/M/yyyy", "dd/MM/yyyy" };
        return DateTime.TryParseExact(
            input, formats, new CultureInfo("pt-BR"),
            DateTimeStyles.None, out result
        );
    }

    private void AddPageError(string message)
    {
        CustomValidator cv = new CustomValidator();
        cv.IsValid = false;
        cv.ErrorMessage = message;
        cv.ValidationGroup = "Salvar";
        cv.Display = ValidatorDisplay.None;
        Page.Validators.Add(cv);
    }
}

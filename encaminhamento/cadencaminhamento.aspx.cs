using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class publico_cadencaminhamento : BasePage
{
    // ============================ PAGE LOAD ============================
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        BindDropDown(ddlEspecialidade,
            EspecialidadeDAO.listaEspecialidade(), "descricao_espec", "cod_especialidade");

        BindHtmlSelect(select2, PreOperatorioDAO.listaExame(),
            "descricao_pre_operatorio", "cod_pre_operatorio");

        BindHtmlSelect(select1, RessonanciaDAO.listaRessonancia(),
            "descricao_ressonancia", "cod_ressonancia");

        BindHtmlSelect(select3, TeleConsultaDAO.listaExame(),
            "descricao_teleconsulta", "cod_teleconsulta");

        BindHtmlSelect(select4, ExamesUnicosDAO.listaExame(),
            "descricao_exames_unico", "cod_exames_unico");
    }

    // Presume que no ASPX o DropDownList tem AppendDataBoundItems="true"
    // e (opcionalmente) já contém <asp:ListItem Text="Selecione..." Value="" />
    private static void BindDropDown(ListControl ddl, object data, string textField, string valueField)
    {
        // NÃO limpar para preservar o placeholder existente no markup
        ddl.DataSource = data;
        ddl.DataTextField = textField;
        ddl.DataValueField = valueField;
        ddl.DataBind();

        // Garante que o placeholder exista e fique no topo/selecionado
        if (ddl.Items.Count == 0 || ddl.Items[0].Value != "")
            ddl.Items.Insert(0, new ListItem("Selecione...", ""));

        ddl.ClearSelection();
        ddl.Items[0].Selected = true;
    }

    private static void BindHtmlSelect(HtmlSelect sel, object data, string textField, string valueField)
    {
        sel.Items.Clear();
        sel.DataSource = data;
        sel.DataTextField = textField;
        sel.DataValueField = valueField;
        sel.DataBind();
        RemoveDuplicateByText(sel.Items);
    }

    // Remove itens duplicados por TEXTO (case-insensitive)
    private static void RemoveDuplicateByText(ListItemCollection items)
    {
        HashSet<string> seen = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        for (int i = items.Count - 1; i >= 0; i--)
        {
            string t = (items[i].Text ?? "").Trim();
            if (t.Length == 0 || !seen.Add(t))
                items.RemoveAt(i);
        }
    }

    // ======================== BUSCA PACIENTE ===========================
    protected void btnPesquisapaciente_Click(object sender, EventArgs e)
    {
        txbNomePaciente.Text = "";

        if (IsNullOrWhiteSpace(txbProntuario.Text))
        {
            AddPageError("Informe o prontuário.");
            return;
        }

        int prontuario;
        if (!int.TryParse(txbProntuario.Text, out prontuario))
        {
            AddPageError("Prontuário inválido.");
            return;
        }

        Paciente p = CarregaDadosPaciente(prontuario);
        if (p == null || IsNullOrWhiteSpace(p.Nm_nome))
        {
            AddPageError("Paciente não encontrado.");
            return;
        }

        txbNomePaciente.Text = (p.Nm_nome ?? "").ToUpperInvariant();
    }

    public Paciente CarregaDadosPaciente(int prontuario)
    {
        try
        {
            string uri = "http://10.48.21.64:5000/hspmsgh-api/pacientes/paciente/" + prontuario;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode != HttpStatusCode.OK) return null;
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<Paciente>(json);
                }
            }
        }
        catch { return null; }
    }

    // =========== CustomValidator: algum exame (qualquer grupo) =========
    protected void cvAlgumExame_ServerValidate(object source, ServerValidateEventArgs args)
    {
        List<HtmlSelect> grupos = GetExamSelects();
        args.IsValid = HasAnySelection(grupos);
    }

    private static bool HasAnySelection(IEnumerable<HtmlSelect> selects)
    {
        foreach (HtmlSelect s in selects)
        {
            foreach (ListItem it in s.Items)
                if (it.Selected) return true;
        }
        return false;
    }

    // Descobre todos os <select runat="server" data-exam="true">
    private List<HtmlSelect> GetExamSelects()
    {
        List<HtmlSelect> list = new List<HtmlSelect>();
        CollectExamSelects(this, list);
        return list;
    }

    private static void CollectExamSelects(Control root, List<HtmlSelect> acc)
    {
        for (int i = 0; i < root.Controls.Count; i++)
        {
            Control c = root.Controls[i];
            HtmlSelect sel = c as HtmlSelect;
            if (sel != null)
            {
                string flag = sel.Attributes["data-exam"];
                if (!string.IsNullOrEmpty(flag) && string.Equals(flag, "true", StringComparison.OrdinalIgnoreCase))
                    acc.Add(sel);
            }
            if (c.HasControls())
                CollectExamSelects(c, acc);
        }
    }

    // ============================== SALVAR =============================
    protected void btnGravar_Click(object sender, EventArgs e)
    {
        // espelha validações do front
        if (!Page.IsValid) return;

        int prontuario;
        if (!int.TryParse(txbProntuario.Text, out prontuario))
        {
            AddPageError("Prontuário inválido.");
            return;
        }

        if (IsNullOrWhiteSpace(txbNomePaciente.Text))
        {
            AddPageError("Pesquise o paciente para preencher o nome.");
            return;
        }

        DateTime dtPedido;
        if (!TryParseDatePtBr(txbDtPedido.Text, out dtPedido))
        {
            AddPageError("Data do pedido inválida.");
            return;
        }

        if (IsNullOrWhiteSpace(ddlEspecialidade.SelectedValue))
        {
            AddPageError("Selecione a especialidade.");
            return;
        }

        if (!HasAnySelection(GetExamSelects()))
        {
            AddPageError("Selecione pelo menos um item em Pré-operatório, Ressonância, Teleconsulta ou Exames Únicos.");
            return;
        }

        // Pedido (string com os pré-operatórios para histórico)
        string examesPreOpStr = JoinSelectedTexts(select2);

        Pedido pedido = new Pedido();
        pedido.prontuario = prontuario;
        pedido.nome_paciente = (txbNomePaciente.Text ?? "").Trim().ToUpperInvariant();
        pedido.data_pedido = dtPedido;
        pedido.cod_especialidade = Convert.ToInt32(ddlEspecialidade.SelectedValue);
        pedido.exames_solicitados = examesPreOpStr;
        pedido.outras_informacoes = txbOb.Text;
        pedido.solicitante = (txbprofissional.Text ?? "").ToUpperInvariant();
        pedido.usuario = Session["login"] != null ? Session["login"].ToString() : "desconhecido";

        int codPedido = PedidoDAO.GravaPedidoConsulta(
            pedido.prontuario, pedido.nome_paciente, pedido.data_pedido,
            pedido.cod_especialidade, pedido.exames_solicitados,
            pedido.outras_informacoes, pedido.solicitante, pedido.usuario);

        // Coleções selecionadas (sem LINQ)
        List<PreOperatorio> preOps = ToPreOps(BuildPairs(select2));
        List<Ressonancia> ressons = ToRessons(BuildPairs(select1));
        List<TeleConsulta> teles = ToTeles(BuildPairs(select3));
        List<ExameUnico> unicos = ToUnicos(BuildPairs(select4));

        // Grava relações
        ExamesUnicosDAO.GravaExamesPorPedidos(unicos, codPedido);
        TeleConsultaDAO.GravaExamesPorPedidos(teles, codPedido);
        PreOperatorioDAO.GravaExamesPorPedidos(preOps, codPedido);
        RessonanciaDAO.GravaRessonanciaPorPedidos(ressons, codPedido);

        // Modal de sucesso
        ScriptManager.RegisterStartupScript(Page, GetType(), "ok",
            "$(function(){ $('#myModal').modal(); });", true);
    }

    // ============================ HELPERS ==============================
    private static string JoinSelectedTexts(HtmlSelect s)
    {
        List<string> parts = new List<string>();
        foreach (ListItem it in s.Items)
            if (it.Selected) parts.Add(it.Text);
        return string.Join(", ", parts.ToArray());
    }

    // Converte HtmlSelect -> lista (codigo, texto)
    private static List<KeyValuePair<int, string>> BuildPairs(HtmlSelect s)
    {
        List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
        foreach (ListItem it in s.Items)
        {
            if (!it.Selected) continue;
            int code = SafeInt(it.Value);
            string text = it.Text;
            list.Add(new KeyValuePair<int, string>(code, text));
        }
        return list;
    }

    private static List<PreOperatorio> ToPreOps(List<KeyValuePair<int, string>> pairs)
    {
        List<PreOperatorio> list = new List<PreOperatorio>();
        for (int i = 0; i < pairs.Count; i++)
        {
            PreOperatorio po = new PreOperatorio();
            po.cod_pre_operatorio = pairs[i].Key;
            po.descricao_pre_operatorio = pairs[i].Value;
            list.Add(po);
        }
        return list;
    }

    private static List<Ressonancia> ToRessons(List<KeyValuePair<int, string>> pairs)
    {
        List<Ressonancia> list = new List<Ressonancia>();
        for (int i = 0; i < pairs.Count; i++)
        {
            Ressonancia r = new Ressonancia();
            r.cod_ressonancia = pairs[i].Key;
            r.descricao_ressonancia = pairs[i].Value;
            list.Add(r);
        }
        return list;
    }

    private static List<TeleConsulta> ToTeles(List<KeyValuePair<int, string>> pairs)
    {
        List<TeleConsulta> list = new List<TeleConsulta>();
        for (int i = 0; i < pairs.Count; i++)
        {
            TeleConsulta t = new TeleConsulta();
            t.cod_teleconsulta = pairs[i].Key;
            t.descricao_teleconsulta = pairs[i].Value;
            list.Add(t);
        }
        return list;
    }

    private static List<ExameUnico> ToUnicos(List<KeyValuePair<int, string>> pairs)
    {
        List<ExameUnico> list = new List<ExameUnico>();
        for (int i = 0; i < pairs.Count; i++)
        {
            ExameUnico u = new ExameUnico();
            u.cod_exames_unico = pairs[i].Key;
            u.descricao_exames_unico = pairs[i].Value;
            list.Add(u);
        }
        return list;
    }

    private static bool TryParseDatePtBr(string input, out DateTime result)
    {
        input = (input ?? "").Trim();
        string[] formats = new string[] { "d/M/yyyy", "dd/MM/yyyy" };
        return DateTime.TryParseExact(
            input, formats, CultureInfo.GetCultureInfo("pt-BR"),
            DateTimeStyles.None, out result);
    }

    private static int SafeInt(string val)
    {
        int n; int.TryParse(val, out n); return n;
    }

    private static bool IsNullOrWhiteSpace(string s)
    {
        return string.IsNullOrEmpty(s) || s.Trim().Length == 0;
    }

    // Adiciona erro ao ValidationSummary (ValidationGroup="Salvar")
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

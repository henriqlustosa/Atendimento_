using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class restrito_TiposExame : BasePage
{
    // Classe view-model para bind no Grid (substitui dynamic/anon type)
    private class ViewItem
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string StatusBadge { get; set; } // "A" ou "I"
        public string StatusTexto { get; set; } // "Ativo" ou "Inativo"
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // categoria padrão
            hfCategoria.Value = "exames_unico";
            SelecionaAba();
            BindGrid();
        }
    }

    // Alterna aba/categoria
    protected void Tab_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        hfCategoria.Value = btn.CommandArgument;
        SelecionaAba();
        LimpaMensagens();
        BindGrid();
    }

    private void SelecionaAba()
    {
        string cat = hfCategoria.Value;
        tabExamesUnico.CssClass = "nav-link" + (cat == "exames_unico" ? " active" : "");
        tabPreOp.CssClass = "nav-link" + (cat == "pre_operatorio" ? " active" : "");
        tabRessonancia.CssClass = "nav-link" + (cat == "ressonancia" ? " active" : "");
        tabTeleconsulta.CssClass = "nav-link" + (cat == "teleconsulta" ? " active" : "");
    }

    private void BindGrid()
    {
        // 1) Busca dados e projeta para a View
        TipoMeta meta = TipoMeta.FromCategoria(hfCategoria.Value);
        List<TipoRegistro> lista = TiposExameDAO.Listar(meta);

        List<ViewItem> view = new List<ViewItem>();
        foreach (TipoRegistro r in lista)
        {
            string badge = (r.StatusPadrao == "A") ? "A" : "I";
            string texto = (r.StatusPadrao == "A") ? "Ativo" : "Inativo";
            view.Add(new ViewItem { Id = r.Id, Descricao = r.Descricao, StatusBadge = badge, StatusTexto = texto });
        }

        // 2) Bind no Grid
        gvTipos.DataKeyNames = new string[] { "Id" }; // garantia extra
        gvTipos.ShowFooter = false;                   // evita <td colspan=...>
        gvTipos.AllowPaging = false;                  // paginação fica com o DataTables
        gvTipos.DataSource = view;
        gvTipos.DataBind();

        // 3) Força header dentro de <thead> (ajuda o DataTables a contar colunas)
        gvTipos.UseAccessibleHeader = true;
        if (gvTipos.HeaderRow != null)
            gvTipos.HeaderRow.TableSection = TableRowSection.TableHeader;

        // 4) Atualiza o UpdatePanel
        updGrid.Update();

        // 5) Re-inicializa DataTables no cliente após o async postback
        //    (safeInitDataTable() está no .aspx e já trata THEAD/colspan/mismatch)
        ScriptManager.RegisterStartupScript(
            this, GetType(),
            "reinitDT",
            "if(window.safeInitDataTable){ safeInitDataTable(); }",
            true
        );
    }


    protected void gvTipos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        int id = Convert.ToInt32(gvTipos.DataKeys[index].Value);
        TipoMeta meta = TipoMeta.FromCategoria(hfCategoria.Value);

        if (e.CommandName == "editRecord")
        {
            TipoRegistro reg = TiposExameDAO.Obter(meta, id);
            if (reg == null) { ShowError("Registro não encontrado."); return; }

            hfEditId.Value = reg.Id.ToString();
            txtDescricaoEdit.Text = reg.Descricao;
            ddlStatusEdit.SelectedValue = reg.StatusPadrao == "A" ? "A" : "I";
            ScriptManager.RegisterStartupScript(this, GetType(), "openEdit", "openEdit();", true);
        }
        else if (e.CommandName == "deleteRecord")
        {
            hfDelId.Value = id.ToString();
            litDelId.Text = id.ToString();
            ScriptManager.RegisterStartupScript(this, GetType(), "openDel", "openDel();", true);
        }
        BindGrid();
    }

    protected void btnSalvarNovo_Click(object sender, EventArgs e)
    {
        TipoMeta meta = TipoMeta.FromCategoria(hfCategoria.Value);
        try
        {
            string desc = SafeTrim(txtDescricaoNovo.Text);
            if (IsNullOrWhiteSpace35(desc)) { ShowError("Informe a descrição."); return; }

            string statusPadrao = ddlStatusNovo.SelectedValue; // "A" ou "I"
            TiposExameDAO.Inserir(meta, desc, statusPadrao);

            txtDescricaoNovo.Text = "";
            ddlStatusNovo.SelectedValue = "A";
            ShowOk("Registro incluído com sucesso.");
            BindGrid();
        }
        catch (Exception ex) { ShowError("Erro ao incluir: " + ex.Message); }
    }

    protected void btnSalvarEdit_Click(object sender, EventArgs e)
    {
        TipoMeta meta = TipoMeta.FromCategoria(hfCategoria.Value);
        try
        {
            int id = int.Parse(hfEditId.Value);
            string desc = SafeTrim(txtDescricaoEdit.Text);
            string statusPadrao = ddlStatusEdit.SelectedValue;

            TiposExameDAO.Atualizar(meta, id, desc, statusPadrao);
            ShowOk("Registro atualizado com sucesso.");
            BindGrid();
        }
        catch (Exception ex) { ShowError("Erro ao atualizar: " + ex.Message); }
    }

    protected void btnConfirmDel_Click(object sender, EventArgs e)
    {
        TipoMeta meta = TipoMeta.FromCategoria(hfCategoria.Value);
        try
        {
            int id = int.Parse(hfDelId.Value);
            TiposExameDAO.Excluir(meta, id);
            ShowOk("Registro excluído com sucesso.");
            BindGrid();
        }
        catch (Exception ex) { ShowError("Erro ao excluir: " + ex.Message); }
    }

    // utilidades (compatíveis com C# 3 / .NET 3.5)
    private static string SafeTrim(string s)
    {
        return (s == null) ? "" : s.Trim();
    }

    private static bool IsNullOrWhiteSpace35(string s)
    {
        if (s == null) return true;
        for (int i = 0; i < s.Length; i++)
        {
            if (!char.IsWhiteSpace(s[i])) return false;
        }
        return true;
    }

    private void ShowOk(string msg)
    {
        lblOk.Text = msg; lblErr.Text = "";
    }

    private void ShowError(string msg)
    {
        lblErr.Text = msg; lblOk.Text = "";
    }

    private void LimpaMensagens()
    {
        lblOk.Text = ""; lblErr.Text = "";
    }
}

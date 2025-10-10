using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class restrito_TiposExame : BasePage
{
    private class ViewItem
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string StatusBadge { get; set; }
        public string StatusTexto { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            hfCategoria.Value = "exames_unico"; // categoria padrão
            SelecionaAba();
            BindGrid();
        }
    }

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
        // NOVO: marca a aba Especialidade
        tabEspecialidade.CssClass = "nav-link" + (cat == "especialidade" ? " active" : "");
    }

    private void BindGrid()
    {
        TipoMeta meta = TipoMeta.FromCategoria(hfCategoria.Value);
        List<TipoRegistro> lista = TiposExameDAO.Listar(meta);

        var view = new List<ViewItem>();
        foreach (var r in lista)
        {
            string badge = (r.StatusPadrao == "A") ? "A" : "I";
            string texto = (r.StatusPadrao == "A") ? "Ativo" : "Inativo";
            view.Add(new ViewItem { Id = r.Id, Descricao = r.Descricao, StatusBadge = badge, StatusTexto = texto });
        }

        gvTipos.DataKeyNames = new[] { "Id" };
        gvTipos.ShowFooter = false;
        gvTipos.AllowPaging = false;
        gvTipos.DataSource = view;
        gvTipos.DataBind();

        gvTipos.UseAccessibleHeader = true;
        if (gvTipos.HeaderRow != null)
            gvTipos.HeaderRow.TableSection = TableRowSection.TableHeader;

        updGrid.Update();
        ScriptManager.RegisterStartupScript(this, GetType(), "reinitDT",
            "if(window.safeInitDataTable){ safeInitDataTable(); }", true);
    }

    protected void gvTipos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        int id = Convert.ToInt32(gvTipos.DataKeys[index].Value);
        TipoMeta meta = TipoMeta.FromCategoria(hfCategoria.Value);

        if (e.CommandName == "editRecord")
        {
            var reg = TiposExameDAO.Obter(meta, id);
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
        var meta = TipoMeta.FromCategoria(hfCategoria.Value);
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
        var meta = TipoMeta.FromCategoria(hfCategoria.Value);
        try
        {
            int id = int.Parse(hfEditId.Value);
            string desc = SafeTrim(txtDescricaoEdit.Text);
            string statusPadrao = ddlStatusEdit.SelectedValue;

            TiposExameDAO.Atualizar(meta, id, desc, statusPadrao);
            ShowOk("Registro atualizado com sucesso.");

            ScriptManager.RegisterStartupScript(this, GetType(), "hideEditModal", "$('#mdlEdit').modal('hide');", true);
            ClearEditForm();
            BindGrid();
        }
        catch (Exception ex) { ShowError("Erro ao atualizar: " + ex.Message); }
    }

    protected void btnConfirmDel_Click(object sender, EventArgs e)
    {
        var meta = TipoMeta.FromCategoria(hfCategoria.Value);
        try
        {
            int id = int.Parse(hfDelId.Value);
            TiposExameDAO.Excluir(meta, id);
            ShowOk("Registro excluído com sucesso.");
            BindGrid();
        }
        catch (Exception ex) { ShowError("Erro ao excluir: " + ex.Message); }
    }

    // utilidades
    private static string SafeTrim(string s) { return (s == null) ? "" : s.Trim(); }
    private static bool IsNullOrWhiteSpace35(string s)
    {
        if (s == null) return true; for (int i = 0; i < s.Length; i++) { if (!char.IsWhiteSpace(s[i])) return false; }
        return true;
    }
    private void ShowOk(string msg) { lblOk.Text = msg; lblErr.Text = ""; }
    private void ShowError(string msg) { lblErr.Text = msg; lblOk.Text = ""; }
    private void LimpaMensagens() { lblOk.Text = ""; lblErr.Text = ""; }
    private void ClearEditForm()
    {
        hfEditId.Value = ""; txtDescricaoEdit.Text = "";
        ddlStatusEdit.ClearSelection(); var li = ddlStatusEdit.Items.FindByValue("A"); if (li != null) li.Selected = true;
    }
}

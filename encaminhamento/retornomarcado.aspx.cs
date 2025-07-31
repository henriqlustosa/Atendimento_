using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI;
using System.Collections.Generic;

public partial class encaminhamento_retornomarcado : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
        //    // Supondo que o código do pedido vem por querystring
        //    if (!string.IsNullOrEmpty(Request.QueryString["cod"]))
        //    {
        //        int codPedido = int.Parse(Request.QueryString["cod"]);
        //        CarregarDados(codPedido);
        //    }

        //    CarregarEspecialidades(); // ddlEspecialidade
        //    CarregarSelects();        // select1 a select4
        //}
    }

    private void CarregarDados(int codPedido)
    {
        // Aqui você acessa o banco para buscar os dados do pedido
        //var pedido = PedidoDAO.ObterPedidoPorCodigo(codPedido);

        //if (pedido != null)
        //{
        //    txbProntuario.Text = pedido.Prontuario;
        //    txbNomePaciente.Text = pedido.NomePaciente;
        //    txbDtPedido.Text = pedido.DataPedido.ToString("dd/MM/yyyy");
        //    ddlEspecialidade.SelectedValue = pedido.CodEspecialidade.ToString();
        //    txbOb.Text = pedido.Observacoes;
        //    txbprofissional.Text = pedido.Solicitante;

        //    // Carrega selects múltiplos com os valores salvos
        //    PreencherSelectSelecionados(select1, pedido.CodigosRessonancia);
        //    PreencherSelectSelecionados(select2, pedido.CodigosPreOperatorio);
        //    PreencherSelectSelecionados(select3, pedido.CodigosTeleconsulta);
        //    PreencherSelectSelecionados(select4, pedido.CodigosExamesUnicos);
        //}
    }

    private void PreencherSelectSelecionados(ListControl select, List<int> selecionados)
    {
    //    foreach (ListItem item in select.Items)
    //    {
    //        item.Selected = selecionados.Contains(int.Parse(item.Value));
    //    }
    }

    private void CarregarEspecialidades()
    {
        //ddlEspecialidade.DataSource = EspecialidadeDAO.ListarTodas();
        //ddlEspecialidade.DataTextField = "Descricao";
        //ddlEspecialidade.DataValueField = "Codigo";
        //ddlEspecialidade.DataBind();
    }

    private void CarregarSelects()
    {
        // Supondo que você tem DAOs que retornam listas para cada categoria
        //select1.DataSource = RessonanciaDAO.Listar();
        //select2.DataSource = PreOperatorioDAO.Listar();
        //select3.DataSource = TeleconsultaDAO.Listar();
        //select4.DataSource = ExamesUnicosDAO.Listar();

        //foreach (var select in new[] { select1, select2, select3, select4 })
        //{
        //    select.DataTextField = "Descricao";
        //    select.DataValueField = "Codigo";
        //    select.DataBind();
        //}
    }

    protected void btnGrava_Click(object sender, EventArgs e)
    {
        //int codPedido = int.Parse(Request.QueryString["cod"]);

        //var pedido = new Pedido
        //{
        //    Codigo = codPedido,
        //    DataPedido = DateTime.Parse(txbDtPedido.Text),
        //    CodEspecialidade = int.Parse(ddlEspecialidade.SelectedValue),
        //    Observacoes = txbOb.Text,
        //    Solicitante = txbprofissional.Text,
        //    CodigosRessonancia = ObterSelecionados(select1),
        //    CodigosPreOperatorio = ObterSelecionados(select2),
        //    CodigosTeleconsulta = ObterSelecionados(select3),
        //    CodigosExamesUnicos = ObterSelecionados(select4)
        //};

        //PedidoDAO.Atualizar(pedido);

        //// Dispara o modal via script
        //ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "$('#myModal').modal('show');", true);
    }

    //private List<int> ObterSelecionados(ListControl select)
   // {
        //var lista = new List<int>();
        //foreach (ListItem item in select.Items)
        //{
        //    if (item.Selected)
        //        lista.Add(int.Parse(item.Value));
        //}
        //return lista;
   // }
}

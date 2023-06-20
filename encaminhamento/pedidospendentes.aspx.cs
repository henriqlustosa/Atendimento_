using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class encaminhamento_pedidospendentes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GridView1.DataSource = PedidoDAO.getListaPedidoConsultaPendente();
        GridView1.DataBind();
    }

    protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index;
        
        if (e.CommandName.Equals("editRecord"))
        {
            index = Convert.ToInt32(e.CommandArgument);

            int _id_pedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString()); //id da consulta
            GridViewRow row = GridView1.Rows[index];
            //string _status = row.Cells[7].Text;

            Response.Redirect("~/encaminhamento/retornomarcado.aspx?idpedido=" + _id_pedido +"");
        }
        if (e.CommandName.Equals("deleteRecord"))
        {
            index = Convert.ToInt32(e.CommandArgument);

            int _id_pedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString()); //id da consulta
            GridViewRow row = GridView1.Rows[index];
            
            PedidoDAO.deletePedidodeConsulta(_id_pedido);
            Response.Redirect("~/encaminhamento/pedidospendentes.aspx");
           
            //string _status = row.Cells[7].Text;
           
            
        }
        if (e.CommandName.Equals("fileRecord"))
        {
            String usuario = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();
            index = Convert.ToInt32(e.CommandArgument);

            int _id_pedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString()); //id da consulta
            GridViewRow row = GridView1.Rows[index];

            PedidoDAO.filePedidodeConsulta(_id_pedido);
            Response.Redirect("~/encaminhamento/pedidospendentes.aspx");

            //string _status = row.Cells[7].Text;


        }
    }
}
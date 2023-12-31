﻿using System;
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

public partial class encaminhamento_pedidosarquivados : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GridView1.DataSource = PedidoDAO.getListaPedidoConsultaArquivados();
        GridView1.DataBind();
    }

    protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index;
        
        if (e.CommandName.Equals("printRecord"))
        {
            index = Convert.ToInt32(e.CommandArgument);

            int _id_pedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString()); //id da consulta
            GridViewRow row = GridView1.Rows[index];
            //string _status = row.Cells[7].Text;

            // Response.Redirect("~/encaminhamento/pedidosarquivados.aspx" + _id_pedido);
             Response.Redirect("~/encaminhamento/pedidosarquivados.aspx");
        }
        if (e.CommandName.Equals("viewRecord"))
        {
            index = Convert.ToInt32(e.CommandArgument);

            int _id_pedido = Convert.ToInt32(GridView1.DataKeys[index].Value.ToString()); //id da consulta
            GridViewRow row = GridView1.Rows[index];
            //string _status = row.Cells[7].Text;

            //Response.Redirect("~/encaminhamento/retornomarcado.aspx?idpedido=" + _id_pedido + "");

            Response.Redirect("~/encaminhamento/arquivomarcado.aspx?idpedido=" + _id_pedido + "");
           
            //string _status = row.Cells[7].Text;
           
            
        }
       
    }
}
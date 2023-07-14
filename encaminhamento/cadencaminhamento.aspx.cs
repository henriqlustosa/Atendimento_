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
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading;

public partial class publico_cadencaminhamento : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlEspecialidade.DataSource = EspecialidadeDAO.listaEspecialidade();
            ddlEspecialidade.DataTextField = "descricao_espec";
            ddlEspecialidade.DataValueField = "cod_especialidade";
            ddlEspecialidade.DataBind();
            select2.DataSource = ExameDAO.listaExame();
            select2.DataTextField = "descricao_exame";
            select2.DataValueField = "cod_exame";
            select2.DataBind();
          
         

            // Get a reference to the ContentPlaceHolder
            // Get a reference to the master page

            select1.DataSource = RessonanciaDAO.listaRessonancia();
            select1.DataTextField = "descricao_ressonancia";
            select1.DataValueField = "cod_ressonancia";
            select1.DataBind();
        }
    }

    protected void btnPesquisapaciente_Click(object sender, EventArgs e)
    {
        int _prontuario = Convert.ToInt32(txbProntuario.Text);
        txbNomePaciente.Text = CarregaDadosPaciente(_prontuario).Nm_nome;
    }


    public Paciente CarregaDadosPaciente(int prontuario)
    {
        Paciente details = new Paciente();

        try
        {
            string URI = "http://10.48.21.64:5000/hspmsgh-api/pacientes/paciente/" + prontuario;
            WebRequest request = WebRequest.Create(URI);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(URI);
            // Sends the HttpWebRequest and waits for a response.
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var reader = new StreamReader(httpResponse.GetResponseStream());

                JsonSerializer json = new JsonSerializer();

                var objText = reader.ReadToEnd();

                details = JsonConvert.DeserializeObject<Paciente>(objText);
            }
        }
        catch (WebException ex)
        {
            string err = ex.Message;
        }
        catch (Exception ex1)
        {
            string err1 = ex1.Message;
        }
        return details;
    }

    protected void btnGravar_Click(object sender, EventArgs e)
    {
        int _cod_pedido = 0;
        List<Exame> exames = new List<Exame>();
        List<Ressonancia> ressonancias = new List<Ressonancia>();
       

        string _exames_solicitados = "";
        string _ressonancia_solicitados = "";

        for (int i = 0; i < select2.Items.Count; i++)
        {
            if (select2.Items[i].Selected == true)// getting selected value from CheckBox List  
            {
                _exames_solicitados += select2.Items[i].Text + ", "; // add selected Item text to the String .  
            }
        }
        if (_exames_solicitados != "")
        {
            _exames_solicitados = _exames_solicitados.Substring(0, _exames_solicitados.Length - 2); // Remove Last "," from the string .  
        }

        for (int i = 0; i < select1.Items.Count; i++)
        {
            if (select1.Items[i].Selected == true)// getting selected value from CheckBox List  
            {
                _ressonancia_solicitados += select1.Items[i].Text + ", "; // add selected Item text to the String .  
            }
        }
        if (_ressonancia_solicitados != "")
        {
            _ressonancia_solicitados = _ressonancia_solicitados.Substring(0, _ressonancia_solicitados.Length - 2); // Remove Last "," from the string .  
        }

        Pedido p = new Pedido();
        p.prontuario = Convert.ToInt32(txbProntuario.Text);
        p.nome_paciente = txbNomePaciente.Text.ToUpper();
        p.data_pedido = Convert.ToDateTime(txbDtPedido.Text);
        //p.data_cadastro = DateTime.Now;
        p.cod_especialidade = Convert.ToInt32(ddlEspecialidade.SelectedValue);
        p.exames_solicitados = _exames_solicitados;

        p.outras_informacoes = txbOb.Text;
        p.solicitante = txbprofissional.Text.ToUpper();
        p.usuario = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();

         _cod_pedido = PedidoDAO.GravaPedidoConsulta(p.prontuario,p.nome_paciente, p.data_pedido, p.cod_especialidade, p.exames_solicitados, p.outras_informacoes, p.solicitante, p.usuario);



        for (int i = 0; i < select2.Items.Count ; i++)
        {
            if (select2.Items[i].Selected)
            {
                Exame exm = new Exame();
                exm.descricao_exame = select2.Items[i].Text;
                exm.cod_exame = int.Parse(select2.Items[i].Value);
                exames.Add(exm);
            }
        }
        for (int i = 0; i < select1.Items.Count; i++)
        {
            if (select1.Items[i].Selected)
            {
                Ressonancia ress = new Ressonancia();
                ress.descricao_ressonancia = select1.Items[i].Text;
                ress.cod_ressonancia = int.Parse(select1.Items[i].Value);
                ressonancias.Add(ress);
            }
        }
        ExameDAO.GravaExamesPorPedidos(exames,_cod_pedido);
        RessonanciaDAO.GravaRessonanciaPorPedidos(ressonancias, _cod_pedido);
        //ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + mensagem + "');", true);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("$(document).ready(function(){");
        sb.Append("$('#myModal').modal();");
        sb.Append("});");
        ScriptManager.RegisterStartupScript(Page, this.Page.GetType(), "clientscript", sb.ToString(), true);

        //Response.Redirect("~/encaminhamento/cadencaminhamento.aspx");

    }
}
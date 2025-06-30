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
            // 1. Verifica se o usuário está logado (existe sessão)

            if (Session["login"] == null)

            {

                Response.Redirect("~/login.aspx"); // Redireciona se não estiver logado

                return;

            }



            // 2. Verifica se o perfil é diferente de "1" (Administrador)

            List<int> perfis = Session["perfis"] as List<int>;

            if (perfis == null || (!perfis.Contains(1) && !perfis.Contains(2)))

            {

                Response.Redirect("~/SemPermissao.aspx");

            }
            ddlEspecialidade.DataSource = EspecialidadeDAO.listaEspecialidade();
            ddlEspecialidade.DataTextField = "descricao_espec";
            ddlEspecialidade.DataValueField = "cod_especialidade";
            ddlEspecialidade.DataBind();




            select2.DataSource = PreOperatorioDAO.listaExame();
            select2.DataTextField = "descricao_pre_operatorio";
            select2.DataValueField = "cod_pre_operatorio";
            select2.DataBind();
          
         

            // Get a reference to the ContentPlaceHolder
            // Get a reference to the master page

            select1.DataSource = RessonanciaDAO.listaRessonancia();
            select1.DataTextField = "descricao_ressonancia";
            select1.DataValueField = "cod_ressonancia";
            select1.DataBind();

            select3.DataSource = TeleConsultaDAO.listaExame();
            select3.DataTextField = "descricao_teleconsulta";
            select3.DataValueField = "cod_teleconsulta";
            select3.DataBind();

            select4.DataSource = ExamesUnicosDAO.listaExame();
            select4.DataTextField = "descricao_exames_unico";
            select4.DataValueField = "cod_exames_unico";
            select4.DataBind();
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
        List<PreOperatorio> preoperatorios = new List<PreOperatorio>();
        List<Ressonancia> ressonancias = new List<Ressonancia>();
        List<ExameUnico> examesunicos = new List<ExameUnico>();
        List<TeleConsulta> teleconsultas = new List<TeleConsulta>();


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
                PreOperatorio preoperatorio = new PreOperatorio();
                preoperatorio.descricao_pre_operatorio = select2.Items[i].Text;
                preoperatorio.cod_pre_operatorio = int.Parse(select2.Items[i].Value);
                preoperatorios.Add(preoperatorio);
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


        for (int i = 0; i < select3.Items.Count; i++)
        {
            if (select3.Items[i].Selected)
            {
                TeleConsulta teleconsulta = new TeleConsulta();
                teleconsulta.descricao_teleconsulta = select3.Items[i].Text;
                teleconsulta.cod_teleconsulta = int.Parse(select3.Items[i].Value);
                teleconsultas.Add(teleconsulta);
            }
        }
        for (int i = 0; i < select4.Items.Count; i++)
        {
            if (select4.Items[i].Selected)
            {
                ExameUnico exameunico = new ExameUnico();
                exameunico.descricao_exames_unico = select4.Items[i].Text;
                exameunico.cod_exames_unico = int.Parse(select4.Items[i].Value);
                examesunicos.Add(exameunico);
            }
        }

        ExamesUnicosDAO.GravaExamesPorPedidos(examesunicos, _cod_pedido);
        TeleConsultaDAO.GravaExamesPorPedidos(teleconsultas, _cod_pedido);
        PreOperatorioDAO.GravaExamesPorPedidos(preoperatorios, _cod_pedido);
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